using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    [RequireComponent(typeof(RawImage))]  
    public class RawImageRenderTextureUtility : MonoBehaviour  
    {  
        [SerializeField] private string targetCameraTag;  
        private RawImage _rawImage;  
        private Camera _targetCamera;  
        private RectTransform _textureRectTransform;  
        private Camera _uiCamera;  
  
        public void Start()  
        {        _rawImage = GetComponent<RawImage>();  
            _textureRectTransform = _rawImage.transform as RectTransform;  
            var cameras = Camera.allCameras;  
            foreach (var camera in cameras)  
            {            if (!camera.tag.Equals(targetCameraTag))  
                    continue;  
                _targetCamera = camera;  
                break;  
            }  
            _targetCamera.targetTexture = _rawImage.texture as RenderTexture;  
            _uiCamera = Camera.main;  
        }    public Vector2 ScreenToRenderTexPoint(Vector2 screenPoint)  
        {        RectTransformUtility.ScreenPointToLocalPointInRectangle(_textureRectTransform, screenPoint, null, out Vector2 localPoint);  
            var rect = _textureRectTransform.rect;  
            Vector2 normalizedPoint = new Vector2((localPoint.x - rect.x) / rect.width - 0.5f, (localPoint.y - rect.y) / rect.height- 0.5f);  
  
            float orthoHeight = _targetCamera.orthographicSize * 2;  
            float orthoWidth = orthoHeight * _targetCamera.aspect;  
  
            Vector2 worldPoint = new Vector2(normalizedPoint.x * orthoWidth, normalizedPoint.y * orthoHeight);  
            return worldPoint + (Vector2)_targetCamera.transform.position;  
        }}
}