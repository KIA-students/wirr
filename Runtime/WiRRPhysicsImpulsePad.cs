using System.Collections.Generic;
using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Pole fizyczne nadające impuls obiektom Rigidbody wchodzącym w trigger.
    /// Może służyć jako wyrzutnia, odbijak albo pole siłowe.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRPhysicsImpulsePad : MonoBehaviour
    {
        [InspectorName("Lokalny kierunek impulsu")]
        [SerializeField] private Vector3 localDirection = new Vector3(0f, 1f, 0.35f);

        [Min(0f)]
        [InspectorName("Siła impulsu")]
        [SerializeField] private float impulse = 5.5f;

        [Min(0f)]
        [InspectorName("Losowy moment obrotowy")]
        [SerializeField] private float torqueImpulse = 0.6f;

        [Min(0f)]
        [InspectorName("Czas odnowienia [s]")]
        [SerializeField] private float cooldownSeconds = 0.35f;

        [InspectorName("Tryb siły")]
        [SerializeField] private ForceMode forceMode = ForceMode.Impulse;

        private readonly Dictionary<int, float> lastImpulseTime = new Dictionary<int, float>();

        public void Configure(Vector3 direction, float newImpulse, float cooldown = 0.35f)
        {
            localDirection = direction.sqrMagnitude > 0.0001f ? direction : Vector3.up;
            impulse = Mathf.Max(0f, newImpulse);
            cooldownSeconds = Mathf.Max(0f, cooldown);
        }

        private void Reset()
        {
            var box = GetComponent<BoxCollider>();
            if (box != null)
                box.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            ApplyImpulse(other.attachedRigidbody);
        }

        private void OnTriggerStay(Collider other)
        {
            ApplyImpulse(other.attachedRigidbody);
        }

        private void ApplyImpulse(Rigidbody body)
        {
            if (body == null || body.isKinematic || body.gameObject == gameObject)
                return;

            var id = body.GetInstanceID();
            var now = Time.time;
            if (lastImpulseTime.TryGetValue(id, out var previous) &&
                now - previous < cooldownSeconds)
            {
                return;
            }

            lastImpulseTime[id] = now;

            var direction = transform.TransformDirection(localDirection.normalized);
            body.AddForce(direction * impulse, forceMode);

            if (torqueImpulse > 0f)
            {
                var seed = id * 0.001f + now;
                var torque = new Vector3(
                    Mathf.PerlinNoise(seed, 0.1f) - 0.5f,
                    Mathf.PerlinNoise(seed, 0.5f) - 0.5f,
                    Mathf.PerlinNoise(seed, 0.9f) - 0.5f).normalized;
                body.AddTorque(torque * torqueImpulse, ForceMode.Impulse);
            }
        }
    }
}
