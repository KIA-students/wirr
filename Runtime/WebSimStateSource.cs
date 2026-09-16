using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace KIA.WiRR
{
    public sealed class WebSimStateSource : MonoBehaviour, IRobotStateSource
    {
        [SerializeField] private string backendWebSocketUrl = "";
        [SerializeField] private string sessionCode = "TEAM01";
        [SerializeField] private string robotId = "rrbot";
        [SerializeField] private bool connectOnStart;
        [SerializeField, Min(0.25f)] private float staleAfterSeconds = 1.5f;
        [SerializeField, Min(5f)] private float heartbeatSeconds = 30f;

        private ClientWebSocket socket;
        private CancellationTokenSource cancellation;
        private readonly ConcurrentQueue<RobotState> stateQueue = new ConcurrentQueue<RobotState>();
        private readonly ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
        private double lastStateRealtime;
        private double nextHeartbeatRealtime;

        public bool IsConnected => socket != null && socket.State == WebSocketState.Open;
        public string SessionCode => sessionCode;
        public string BackendWebSocketUrl => backendWebSocketUrl;
        public string RobotId => robotId;
        public bool IsStale => IsConnected && Time.realtimeSinceStartupAsDouble - lastStateRealtime > staleAfterSeconds;
        public event Action<RobotState> StateReceived;
        public event Action<string> StatusReceived;

        private async void Start()
        {
            if (connectOnStart && !string.IsNullOrWhiteSpace(backendWebSocketUrl))
                await ConnectAsync();
        }

        private void Update()
        {
            while (stateQueue.TryDequeue(out var state))
            {
                lastStateRealtime = Time.realtimeSinceStartupAsDouble;
                StateReceived?.Invoke(state);
            }

            while (logQueue.TryDequeue(out var status))
            {
                StatusReceived?.Invoke(status);
                Debug.Log($"[WiRR WebSim] {status}");
            }

            if (IsConnected && Time.realtimeSinceStartupAsDouble >= nextHeartbeatRealtime)
            {
                nextHeartbeatRealtime = Time.realtimeSinceStartupAsDouble + heartbeatSeconds;
                _ = PublishControlAsync("ping", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            }
        }

        private async void OnDestroy()
        {
            await DisconnectAsync();
        }

        public void Configure(string websocketUrl, string session, string robot)
        {
            backendWebSocketUrl = websocketUrl?.Trim() ?? string.Empty;
            sessionCode = NormalizeSession(session);
            robotId = string.IsNullOrWhiteSpace(robot) ? "rrbot" : robot.Trim();
        }

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;
            if (!Uri.TryCreate(backendWebSocketUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "ws" && uri.Scheme != "wss"))
                throw new InvalidOperationException("WebSim wymaga poprawnego adresu ws:// lub wss://.");

            await DisconnectAsync();
            sessionCode = NormalizeSession(sessionCode);
            cancellation = new CancellationTokenSource();
            var newSocket = new ClientWebSocket();
            await newSocket.ConnectAsync(uri, cancellation.Token);
            socket = newSocket;

            await SubscribeAsync($"/wirr/{sessionCode}/joint_states", "sensor_msgs/msg/JointState", 30);
            await SubscribeAsync($"/wirr/{sessionCode}/status", "std_msgs/msg/String", 0);
            await SubscribeAsync("/wirr/status", "std_msgs/msg/String", 0);
            await SubscribeAsync("/clock", "rosgraph_msgs/msg/Clock", 100);
            await PublishControlAsync("create");
            await PublishControlAsync("ping", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            nextHeartbeatRealtime = Time.realtimeSinceStartupAsDouble + heartbeatSeconds;
            _ = ReceiveLoopAsync(newSocket, cancellation.Token);
            logQueue.Enqueue($"CONNECTED {uri} session={sessionCode} robot={robotId}");
        }

        public async Task DisconnectAsync()
        {
            var current = socket;
            socket = null;
            cancellation?.Cancel();
            cancellation?.Dispose();
            cancellation = null;

            if (current == null)
                return;
            try
            {
                if (current.State == WebSocketState.Open || current.State == WebSocketState.CloseReceived)
                    await current.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unity disconnect", CancellationToken.None);
            }
            catch (WebSocketException)
            {
                // Connection may already be gone; disposal below is sufficient.
            }
            catch (ObjectDisposedException)
            {
                // Safe during scene teardown.
            }
            current.Dispose();
        }

        public Task SendMotionAsync(string motionName)
        {
            var name = string.IsNullOrWhiteSpace(motionName) ? "A" : motionName.Trim().ToUpperInvariant();
            return PublishStringAsync($"/wirr/{sessionCode}/command", $"motion:{name}");
        }

        public Task SendHomeAsync() => PublishStringAsync($"/wirr/{sessionCode}/command", "home");
        public Task SendResetAsync() => PublishStringAsync($"/wirr/{sessionCode}/command", "reset");
        public Task DestroySessionAsync() => PublishControlAsync("destroy");

        private async Task ReceiveLoopAsync(ClientWebSocket receiveSocket, CancellationToken token)
        {
            var buffer = new byte[64 * 1024];
            var builder = new StringBuilder();
            try
            {
                while (!token.IsCancellationRequested && receiveSocket.State == WebSocketState.Open)
                {
                    builder.Clear();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await receiveSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                        if (result.MessageType == WebSocketMessageType.Close)
                            return;
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    } while (!result.EndOfMessage);

                    ProcessEnvelope(builder.ToString());
                }
            }
            catch (OperationCanceledException)
            {
                // Expected during disconnect.
            }
            catch (ObjectDisposedException)
            {
                // Expected when a scene is closed while receiving.
            }
            catch (Exception exception)
            {
                logQueue.Enqueue("ERROR " + exception.Message);
            }
        }

        private void ProcessEnvelope(string json)
        {
            RosbridgeEnvelope envelope;
            try
            {
                envelope = JsonUtility.FromJson<RosbridgeEnvelope>(json);
            }
            catch (ArgumentException)
            {
                return;
            }

            if (envelope == null || envelope.op != "publish" || envelope.msg == null)
                return;

            if (envelope.topic == $"/wirr/{sessionCode}/joint_states" && envelope.msg.position != null)
            {
                stateQueue.Enqueue(new RobotState(
                    envelope.msg.name ?? Array.Empty<string>(),
                    envelope.msg.position,
                    Time.realtimeSinceStartupAsDouble));
            }
            else if ((envelope.topic == $"/wirr/{sessionCode}/status" || envelope.topic == "/wirr/status") &&
                     !string.IsNullOrEmpty(envelope.msg.data))
            {
                logQueue.Enqueue(envelope.msg.data);
            }
        }

        private Task PublishControlAsync(string action, long clientTime = 0)
        {
            var payload = JsonUtility.ToJson(new ControlPayload
            {
                session = sessionCode,
                robot = robotId,
                action = action,
                clientTime = clientTime
            });
            return PublishStringAsync("/wirr/control", payload);
        }

        private Task PublishStringAsync(string topic, string value)
        {
            var escaped = EscapeJson(value ?? string.Empty);
            return SendAsync($"{{\"op\":\"publish\",\"topic\":\"{topic}\",\"type\":\"std_msgs/msg/String\",\"msg\":{{\"data\":\"{escaped}\"}}}}");
        }

        private Task SubscribeAsync(string topic, string type, int throttleRate)
        {
            return SendAsync($"{{\"op\":\"subscribe\",\"topic\":\"{topic}\",\"type\":\"{type}\",\"throttle_rate\":{throttleRate},\"queue_length\":1}}");
        }

        private async Task SendAsync(string json)
        {
            var current = socket;
            var tokenSource = cancellation;
            if (current == null || current.State != WebSocketState.Open || tokenSource == null)
                throw new InvalidOperationException("WebSim nie jest połączony.");
            var bytes = Encoding.UTF8.GetBytes(json);
            await current.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, tokenSource.Token);
        }

        private static string NormalizeSession(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "TEAM01";
            var source = value.Trim().ToUpperInvariant();
            var builder = new StringBuilder();
            foreach (var character in source)
            {
                if ((character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9') || character == '_' || character == '-')
                    builder.Append(character);
            }
            if (builder.Length < 3)
                builder.Append("001");
            if (builder.Length > 12)
                builder.Length = 12;
            return builder.ToString();
        }

        private static string EscapeJson(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        [Serializable]
        private sealed class RosbridgeEnvelope
        {
            public string op;
            public string topic;
            public RosMessage msg;
        }

        [Serializable]
        private sealed class RosMessage
        {
            public string[] name;
            public double[] position;
            public string data;
        }

        [Serializable]
        private sealed class ControlPayload
        {
            public string session;
            public string robot;
            public string action;
            public long clientTime;
        }
    }
}
