using System;
using System.Collections.Generic;

namespace KIA.WiRR.Editor
{
    internal sealed class WiRRLabDefinition
    {
        public int Number { get; }
        public string Title { get; }
        public string SampleName { get; }
        public IReadOnlyList<string> Packages { get; }
        public IReadOnlyList<WiRRExternalSample> ExternalSamples { get; }

        public WiRRLabDefinition(int number, string title, string sampleName, string[] packages, WiRRExternalSample[] externalSamples)
        {
            Number = number;
            Title = title;
            SampleName = sampleName;
            Packages = packages ?? Array.Empty<string>();
            ExternalSamples = externalSamples ?? Array.Empty<WiRRExternalSample>();
        }
    }

    internal readonly struct WiRRExternalSample
    {
        public string PackageName { get; }
        public string PreferredSampleName { get; }
        public string FallbackSampleName { get; }

        public WiRRExternalSample(string packageName, string preferredSampleName, string fallbackSampleName = null)
        {
            PackageName = packageName;
            PreferredSampleName = preferredSampleName;
            FallbackSampleName = fallbackSampleName;
        }
    }

    internal static class WiRRLabCatalog
    {
        public const string RosTcpConnectorUrl =
            "https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.ros-tcp-connector#v0.7.1";

        private static readonly WiRRLabDefinition[] Labs =
        {
            new WiRRLabDefinition(1, "Środowisko XR i audyt urządzeń", "Lab 01 — Środowisko XR i audyt urządzeń",
                new[] { "com.unity.xr.management@4.7.0", "com.unity.xr.openxr@1.18.0", "com.unity.xr.interaction.toolkit@3.6.0", "com.unity.xr.hands@1.9.0" },
                new[] {
                    new WiRRExternalSample("com.unity.xr.interaction.toolkit", "Starter Assets"),
                    new WiRRExternalSample("com.unity.xr.interaction.toolkit", "XR Interaction Simulator", "XR Device Simulator")
                }),
            new WiRRLabDefinition(2, "Interakcja i manipulacja XR", "Lab 02 — Interakcja i manipulacja XR",
                new[] { "com.unity.xr.management@4.7.0", "com.unity.xr.openxr@1.18.0", "com.unity.xr.interaction.toolkit@3.6.0" },
                new[] {
                    new WiRRExternalSample("com.unity.xr.interaction.toolkit", "Starter Assets"),
                    new WiRRExternalSample("com.unity.xr.interaction.toolkit", "XR Interaction Simulator", "XR Device Simulator")
                }),
            new WiRRLabDefinition(3, "Rejestracja i kotwice AR", "Lab 03 — Rejestracja i kotwice AR",
                new[] { "com.unity.xr.management@4.7.0", "com.unity.xr.interaction.toolkit@3.6.0", "com.unity.xr.arfoundation@6.4.1", "com.unity.xr.arcore@6.4.1" },
                new[] {
                    new WiRRExternalSample("com.unity.xr.interaction.toolkit", "Starter Assets"),
                    new WiRRExternalSample("com.unity.xr.interaction.toolkit", "AR Starter Assets")
                }),
            new WiRRLabDefinition(4, "Rozumienie sceny i mieszanie rzeczywistości", "Lab 04 — Rozumienie sceny i MR",
                new[] { "com.unity.xr.management@4.7.0", "com.unity.xr.interaction.toolkit@3.6.0", "com.unity.xr.arfoundation@6.4.1", "com.unity.xr.arcore@6.4.1" },
                new[] {
                    new WiRRExternalSample("com.unity.xr.interaction.toolkit", "Starter Assets"),
                    new WiRRExternalSample("com.unity.xr.interaction.toolkit", "AR Starter Assets")
                }),
            new WiRRLabDefinition(5, "Optymalizacja modeli CAD", "Lab 05 — Optymalizacja modeli CAD", Array.Empty<string>(), Array.Empty<WiRRExternalSample>()),
            new WiRRLabDefinition(6, "Bliźniak cyfrowy ROS 2 + Gazebo", "Lab 06 — Bliźniak cyfrowy ROS 2 + Gazebo", new[] { RosTcpConnectorUrl }, Array.Empty<WiRRExternalSample>()),
            new WiRRLabDefinition(7, "Walidacja i testy akceptacyjne", "Lab 07 — Walidacja i testy akceptacyjne", new[] { "com.unity.test-framework@1.8.0" }, Array.Empty<WiRRExternalSample>())
        };

        public static WiRRLabDefinition Get(int labNumber)
        {
            if (labNumber < 1 || labNumber > Labs.Length)
                throw new ArgumentOutOfRangeException(nameof(labNumber));
            return Labs[labNumber - 1];
        }

        public static string[] GetPopupLabels()
        {
            var labels = new string[Labs.Length];
            for (var i = 0; i < Labs.Length; i++)
                labels[i] = $"Lab {Labs[i].Number:00} — {Labs[i].Title}";
            return labels;
        }
    }
}
