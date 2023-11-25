using UnityEngine;

namespace TendedTarsier
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float _lerpSpeed = 1.0f;

        private void Update()
        {
            if (_target == null)
            {
                return;
            }

            transform.position = Vector3.Lerp(transform.position, _target.position, _lerpSpeed * Time.deltaTime);
        }
    }
}