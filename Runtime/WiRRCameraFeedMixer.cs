using System.Collections;
using UnityEngine;

namespace KIA.WiRR
{
    /// <summary>
    /// Displays a real camera/external texture on a virtual surface. On PC/Android it
    /// can use WebCamTexture; on XR devices a platform passthrough/camera provider can
    /// call SetExternalTexture without coupling WiRR Runtime to one vendor SDK.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WiRRCameraFeedMixer : MonoBehaviour
    {
        [InspectorName("Powierzchnia obrazu")]
        [SerializeField] private Renderer targetRenderer;

        [InspectorName("Właściwość tekstury")]
        [SerializeField] private string textureProperty = "_BaseMap";

        [InspectorName("Uruchom kamerę automatycznie")]
        [SerializeField] private bool autoStartWebCamera;

        [InspectorName("Preferowana szerokość")]
        [SerializeField] private int requestedWidth = 1280;

        [InspectorName("Preferowana wysokość")]
        [SerializeField] private int requestedHeight = 720;

        [InspectorName("Preferowane FPS")]
        [SerializeField] private int requestedFps = 30;

        private readonly MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        private WebCamTexture webCamera;
        private Texture externalTexture;
        private Texture fallbackTexture;
        private bool mirrorX;
        private bool mirrorY;

        public bool HasLiveTexture =>
            externalTexture != null || (webCamera != null && webCamera.isPlaying);

        public void Configure(Renderer renderer, bool startWebCamera = false)
        {
            targetRenderer = renderer;
            autoStartWebCamera = startWebCamera;
            CaptureFallback();
            ApplyTexture();
        }

        public void SetExternalTexture(Texture texture)
        {
            externalTexture = texture;
            ApplyTexture();
        }

        public void ClearExternalTexture()
        {
            externalTexture = null;
            ApplyTexture();
        }

        public void SetMirror(bool horizontal, bool vertical)
        {
            mirrorX = horizontal;
            mirrorY = vertical;
            ApplyTexture();
        }

        public void StartWebCamera()
        {
            if (webCamera != null && webCamera.isPlaying)
                return;

            StartCoroutine(StartWebCameraRoutine());
        }

        public void StopWebCamera()
        {
            if (webCamera != null && webCamera.isPlaying)
                webCamera.Stop();
            ApplyTexture();
        }

        private void Awake()
        {
            CaptureFallback();
        }

        private void Start()
        {
            if (autoStartWebCamera)
                StartWebCamera();
            else
                ApplyTexture();
        }

        private void OnDestroy()
        {
            if (webCamera != null)
            {
                if (webCamera.isPlaying)
                    webCamera.Stop();
                Destroy(webCamera);
            }
        }

        private IEnumerator StartWebCameraRoutine()
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogWarning("[WiRR] Brak zgody na użycie kamery.");
                yield break;
            }

            var devices = WebCamTexture.devices;
            if (devices == null || devices.Length == 0)
            {
                Debug.LogWarning("[WiRR] Nie znaleziono kamery dostępnej przez WebCamTexture.");
                yield break;
            }

            if (webCamera != null)
            {
                if (webCamera.isPlaying)
                    webCamera.Stop();
                Destroy(webCamera);
            }

            webCamera = new WebCamTexture(
                devices[0].name,
                Mathf.Max(320, requestedWidth),
                Mathf.Max(240, requestedHeight),
                Mathf.Max(1, requestedFps));
            webCamera.Play();
            ApplyTexture();
        }

        private void CaptureFallback()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponentInChildren<Renderer>();

            if (targetRenderer == null || targetRenderer.sharedMaterial == null)
                return;

            if (targetRenderer.sharedMaterial.HasProperty(textureProperty))
                fallbackTexture = targetRenderer.sharedMaterial.GetTexture(textureProperty);
        }

        private void ApplyTexture()
        {
            if (targetRenderer == null)
                return;

            var texture = externalTexture != null
                ? externalTexture
                : webCamera != null && webCamera.isPlaying
                    ? webCamera
                    : fallbackTexture;

            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetTexture(textureProperty, texture);

            var scaleX = mirrorX ? -1f : 1f;
            var scaleY = mirrorY ? -1f : 1f;
            var offsetX = mirrorX ? 1f : 0f;
            var offsetY = mirrorY ? 1f : 0f;
            propertyBlock.SetVector(textureProperty + "_ST", new Vector4(scaleX, scaleY, offsetX, offsetY));

            targetRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
