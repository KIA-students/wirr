using UnityEditor;
using UnityEngine;

namespace KIA.WiRR.Editor
{
    internal static class WiRRBranding
    {
        private const string PackageIconPath = "Packages/pl.prz.kia.wirr/icon.png";
        private static Texture2D icon;

        internal static Texture2D Icon
        {
            get
            {
                if (icon == null) icon = AssetDatabase.LoadAssetAtPath<Texture2D>(PackageIconPath);
                return icon;
            }
        }

        internal static GUIContent Title(string text) => new GUIContent(text, Icon);
    }
}
