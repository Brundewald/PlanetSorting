using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class Ufo : MonoBehaviour
    {
        [SerializeField] private float _movementDuration = 1f;
        private Vector3 _defaultPosition;

        public void Awake()
        {
            _defaultPosition = transform.position;
        }

        public void MoveToPosition(Vector3 position)
        {
            transform.DOComplete();
            transform.DOMove(position, _movementDuration);
        }

        public void ReturnToDefaultPosition()
        {
            transform.DOComplete();
            transform.DOMove(_defaultPosition, _movementDuration);
        }
    }
}