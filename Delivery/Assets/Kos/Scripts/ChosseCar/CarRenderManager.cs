using UnityEngine;
using System;
using System.Collections;

namespace ArcadeBridge
{
    public class CarRenderManager : MonoBehaviour
    {
        [Header("Render Settings")]
        [SerializeField] private Camera renderCameraPrefab;
        [SerializeField] private Vector2Int textureSize = new Vector2Int(1080, 1080);
        [SerializeField] private LayerMask renderLayer = 1 << 10;

        private Camera _renderCamera;
        private Transform _renderRoot;

        private void Awake()
        {
            SetupRenderEnvironment();
        }

        private void SetupRenderEnvironment()
        {
            _renderRoot = new GameObject("CarRenderRoot").transform;

            if (renderCameraPrefab != null)
            {
                _renderCamera = Instantiate(renderCameraPrefab, _renderRoot);
            }
            else
            {
                GameObject camObj = new GameObject("RenderCamera");
                _renderCamera = camObj.AddComponent<Camera>();
                _renderCamera.transform.SetParent(_renderRoot);
                _renderCamera.clearFlags = CameraClearFlags.SolidColor;
                _renderCamera.backgroundColor = new Color(0, 0, 0, 0);
                _renderCamera.cullingMask = renderLayer;
                _renderCamera.orthographic = false;
                _renderCamera.enabled = false;
            }
        }

        public IEnumerator RenderCarImage(GameObject carPrefab, Action<Texture> callback)
        {
            GameObject carInstance = Instantiate(carPrefab, _renderRoot);
            SetLayerRecursive(carInstance, LayerMaskToLayer(renderLayer));

            carInstance.transform.rotation = Quaternion.Euler(0f, 210f, 0f);

            Bounds bounds = CalculateBounds(carInstance);
            float distance = bounds.extents.magnitude * 2f;

            _renderCamera.transform.position = bounds.center + new Vector3(0, 0, -distance);
            _renderCamera.transform.LookAt(bounds.center);
            _renderCamera.fieldOfView = 10f;

            RenderTexture rt = new RenderTexture(textureSize.x, textureSize.y, 24);
            rt.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm;
            rt.Create();

            _renderCamera.targetTexture = rt;
            _renderCamera.Render();

            yield return new WaitForEndOfFrame();

            callback?.Invoke(rt);

            Destroy(carInstance);
        }

        public void DisposeRenderCamera()
        {
            if (_renderCamera != null)
            {
                Destroy(_renderCamera.gameObject);
                _renderCamera = null;
            }

            if (_renderRoot != null)
            {
                Destroy(_renderRoot.gameObject);
                _renderRoot = null;
            }
        }

        private Bounds CalculateBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (Renderer rend in renderers)
            {
                bounds.Encapsulate(rend.bounds);
            }
            return bounds;
        }

        private void SetLayerRecursive(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }

        private int LayerMaskToLayer(LayerMask mask)
        {
            int value = mask.value;
            for (int i = 0; i < 32; i++)
            {
                if ((value & (1 << i)) != 0)
                    return i;
            }
            return 0;
        }
    }
}



