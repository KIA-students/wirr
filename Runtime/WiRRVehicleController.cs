using UnityEngine;
using UnityEngine.InputSystem;

namespace KIA.WiRR
{
    /// <summary>
    /// Prosty kontroler pojazdu typu arcade oparty na Rigidbody.
    /// Domyślnie obsługuje klawiaturę i gamepad, ale może też przyjmować sterowanie
    /// z zewnętrznego interfejsu XR przez SetExternalInput().
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public sealed class WiRRVehicleController : MonoBehaviour
    {
        [Min(0f)]
        [InspectorName("Przyspieszenie")]
        [SerializeField] private float acceleration = 8f;

        [Min(0f)]
        [InspectorName("Przyspieszenie wsteczne")]
        [SerializeField] private float reverseAcceleration = 5f;

        [Min(0.1f)]
        [InspectorName("Maksymalna prędkość [m/s]")]
        [SerializeField] private float maxSpeed = 7f;

        [Min(0f)]
        [InspectorName("Prędkość skrętu [deg/s]")]
        [SerializeField] private float steeringDegreesPerSecond = 95f;

        [Range(0f, 1f)]
        [InspectorName("Przyczepność boczna")]
        [SerializeField] private float lateralGrip = 0.75f;

        [Range(0f, 1f)]
        [InspectorName("Siła hamowania")]
        [SerializeField] private float brakeStrength = 0.82f;

        [InspectorName("Sterowanie lokalne")]
        [Tooltip("WASD/strzałki, spacja = hamulec. Gamepad: lewy drążek + lewy trigger.")]
        [SerializeField] private bool readLocalInput = true;

        private Rigidbody body;
        private WiRRProceduralAudio engineAudio;
        private Vector2 externalDriveInput;
        private float externalBrake;
        private bool externalInputActive;
        private Vector2 localDriveInput;
        private float localBrake;

        public float Speed => body != null ? body.linearVelocity.magnitude : 0f;
        public float NormalizedSpeed => Mathf.Clamp01(Speed / Mathf.Max(0.1f, maxSpeed));

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            engineAudio = GetComponent<WiRRProceduralAudio>();

            body.centerOfMass += new Vector3(0f, -0.18f, 0f);
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void Update()
        {
            if (readLocalInput && !externalInputActive)
                ReadInput();

            if (engineAudio != null)
                engineAudio.SetNormalizedLoad(NormalizedSpeed);
        }

        private void FixedUpdate()
        {
            if (body == null)
                return;

            var drive = externalInputActive ? externalDriveInput : localDriveInput;
            var brake = externalInputActive ? externalBrake : localBrake;

            ApplyDrive(drive.y);
            ApplySteering(drive.x);
            ApplyLateralGrip();
            ApplyBrake(brake);
            LimitSpeed();
        }

        public void SetExternalInput(Vector2 steeringAndThrottle, float brake = 0f)
        {
            externalInputActive = true;
            externalDriveInput = Vector2.ClampMagnitude(steeringAndThrottle, 1f);
            externalBrake = Mathf.Clamp01(brake);
        }

        public void ClearExternalInput()
        {
            externalInputActive = false;
            externalDriveInput = Vector2.zero;
            externalBrake = 0f;
        }

        public void Configure(
            float newAcceleration,
            float newMaxSpeed,
            float newSteeringDegreesPerSecond)
        {
            acceleration = Mathf.Max(0f, newAcceleration);
            maxSpeed = Mathf.Max(0.1f, newMaxSpeed);
            steeringDegreesPerSecond = Mathf.Max(0f, newSteeringDegreesPerSecond);
        }

        private void ReadInput()
        {
            var steering = 0f;
            var throttle = 0f;
            var brake = 0f;

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) steering -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) steering += 1f;
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) throttle += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) throttle -= 1f;
                if (keyboard.spaceKey.isPressed) brake = 1f;
            }

            var gamepad = Gamepad.current;
            if (gamepad != null)
            {
                var stick = gamepad.leftStick.ReadValue();
                if (Mathf.Abs(stick.x) > Mathf.Abs(steering))
                    steering = stick.x;
                if (Mathf.Abs(stick.y) > Mathf.Abs(throttle))
                    throttle = stick.y;
                brake = Mathf.Max(brake, gamepad.leftTrigger.ReadValue());
            }

            localDriveInput = Vector2.ClampMagnitude(new Vector2(steering, throttle), 1f);
            localBrake = Mathf.Clamp01(brake);
        }

        private void ApplyDrive(float throttle)
        {
            if (Mathf.Abs(throttle) < 0.01f)
                return;

            var forwardSpeed = Vector3.Dot(body.linearVelocity, transform.forward);
            var sameDirectionAsVelocity =
                Mathf.Abs(forwardSpeed) < 0.05f ||
                Mathf.Sign(forwardSpeed) == Mathf.Sign(throttle);

            if (sameDirectionAsVelocity && Mathf.Abs(forwardSpeed) >= maxSpeed)
                return;

            var force = throttle >= 0f ? acceleration : reverseAcceleration;
            body.AddForce(transform.forward * (throttle * force), ForceMode.Acceleration);
        }

        private void ApplySteering(float steering)
        {
            if (Mathf.Abs(steering) < 0.01f)
                return;

            var planarVelocity = Vector3.ProjectOnPlane(body.linearVelocity, transform.up);
            var speedFactor = Mathf.Clamp01(planarVelocity.magnitude / 0.8f);
            if (speedFactor <= 0f)
                return;

            var forwardSpeed = Vector3.Dot(planarVelocity, transform.forward);
            var direction = forwardSpeed < -0.05f ? -1f : 1f;
            var degrees =
                steering * steeringDegreesPerSecond * speedFactor * direction * Time.fixedDeltaTime;

            body.MoveRotation(body.rotation * Quaternion.Euler(0f, degrees, 0f));
        }

        private void ApplyLateralGrip()
        {
            var lateralSpeed = Vector3.Dot(body.linearVelocity, transform.right);
            if (Mathf.Abs(lateralSpeed) < 0.001f)
                return;

            body.AddForce(
                -transform.right * lateralSpeed * lateralGrip,
                ForceMode.VelocityChange);
        }

        private void ApplyBrake(float brake)
        {
            if (brake <= 0.001f)
                return;

            var velocity = body.linearVelocity;
            var horizontal = Vector3.ProjectOnPlane(velocity, transform.up);
            var vertical = velocity - horizontal;
            var factor = Mathf.Lerp(1f, brakeStrength, brake);
            body.linearVelocity = horizontal * factor + vertical;
        }

        private void LimitSpeed()
        {
            var velocity = body.linearVelocity;
            var vertical = Vector3.Project(velocity, transform.up);
            var horizontal = velocity - vertical;

            if (horizontal.magnitude <= maxSpeed)
                return;

            body.linearVelocity = horizontal.normalized * maxSpeed + vertical;
        }
    }
}
