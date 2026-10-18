using System;

namespace KIA.WiRR
{
    [Serializable]
    public sealed class RobotState
    {
        public string[] JointNames;
        public double[] Positions;
        public double ReceivedAtSeconds;

        public RobotState(string[] jointNames, double[] positions, double receivedAtSeconds)
        {
            JointNames = jointNames ?? Array.Empty<string>();
            Positions = positions ?? Array.Empty<double>();
            ReceivedAtSeconds = receivedAtSeconds;
        }
    }
}
