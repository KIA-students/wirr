using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Znacznik sceny przygotowanej przez pakiet WiRR.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRSceneMarker : MonoBehaviour
    {
        [Range(1, 7)]
        [InspectorName("Numer laboratorium")]
        [Tooltip("Numer laboratorium, do którego należy ta scena.")]
        [SerializeField] private int labNumber = 1;

        [Tooltip("Opcjonalny identyfikator zespołu, np. numery indeksów.")]
        [InspectorName("Identyfikator zespołu")]
        [SerializeField] private string teamId = string.Empty;

        public int LabNumber
        {
            get => labNumber;
            set => labNumber = Mathf.Clamp(value, 1, 7);
        }

        public string TeamId
        {
            get => teamId;
            set => teamId = value ?? string.Empty;
        }
    }
}
