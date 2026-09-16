using System;
using System.Collections.Generic;
using UnityEngine;

namespace KIA.WiRR
{
    public sealed class WiRRRobotRig : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour stateSourceBehaviour;
        [SerializeField] private string[] jointNames = { "joint1", "joint2" };
        [SerializeField] private Transform[] jointTransforms = Array.Empty<Transform>();
        [SerializeField] private Vector3 localRotationAxis = Vector3.up;

        private IRobotStateSource stateSource;
        private Quaternion[] baseRotations = Array.Empty<Quaternion>();
        private readonly Dictionary<string, int> sourceIndices = new Dictionary<string, int>();

        public string[] JointNames => jointNames;

        private void Awake()
        {
            BindSource();
            baseRotations = new Quaternion[jointTransforms.Length];
            for (var i = 0; i < jointTransforms.Length; i++)
                baseRotations[i] = jointTransforms[i] != null ? jointTransforms[i].localRotation : Quaternion.identity;
        }

        private void OnEnable()
        {
            BindSource();
            if (stateSource != null)
                stateSource.StateReceived += ApplyState;
        }

        private void OnDisable()
        {
            if (stateSource != null)
                stateSource.StateReceived -= ApplyState;
        }

        public void Configure(MonoBehaviour source, string[] names, Transform[] transforms, Vector3 axis)
        {
            if (stateSource != null)
                stateSource.StateReceived -= ApplyState;

            stateSourceBehaviour = source;
            jointNames = names ?? Array.Empty<string>();
            jointTransforms = transforms ?? Array.Empty<Transform>();
            localRotationAxis = axis.sqrMagnitude > 0f ? axis.normalized : Vector3.up;
            baseRotations = new Quaternion[jointTransforms.Length];
            for (var i = 0; i < jointTransforms.Length; i++)
                baseRotations[i] = jointTransforms[i] != null ? jointTransforms[i].localRotation : Quaternion.identity;

            BindSource();
            if (isActiveAndEnabled && stateSource != null)
                stateSource.StateReceived += ApplyState;
        }

        private void BindSource()
        {
            stateSource = stateSourceBehaviour as IRobotStateSource;
        }

        private void ApplyState(RobotState state)
        {
            if (state == null || state.JointNames == null || state.Positions == null)
                return;

            sourceIndices.Clear();
            for (var i = 0; i < state.JointNames.Length; i++)
                sourceIndices[state.JointNames[i]] = i;

            var count = Mathf.Min(jointNames.Length, jointTransforms.Length);
            for (var i = 0; i < count; i++)
            {
                if (jointTransforms[i] == null || !sourceIndices.TryGetValue(jointNames[i], out var sourceIndex))
                    continue;
                if (sourceIndex < 0 || sourceIndex >= state.Positions.Length)
                    continue;

                var degrees = (float)(state.Positions[sourceIndex] * Mathf.Rad2Deg);
                jointTransforms[i].localRotation = baseRotations[i] * Quaternion.AngleAxis(degrees, localRotationAxis);
            }
        }
    }
}
