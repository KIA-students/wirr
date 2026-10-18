using System;

namespace KIA.WiRR
{
    public interface IRobotStateSource
    {
        bool IsConnected { get; }
        string SessionCode { get; }
        event Action<RobotState> StateReceived;
    }
}
