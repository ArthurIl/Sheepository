using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        public Transform target;
        [SerializeField] Vector3 cameraLocalPos;
        [SerializeField] Vector3 localTargetLookAtPosition;

        [SerializeField] float positionLerpSpeed = 0.1f;
        [SerializeField] float lookLerpSpeed = 0.1f;

        Vector3 wantedPos;
        Quaternion wantedRotation;

        private void Update()
        {
            wantedPos = target.TransformPoint(cameraLocalPos);
            wantedPos.y = cameraLocalPos.y + target.position.y;

            transform.position = Vector3.Lerp(transform.position, wantedPos, positionLerpSpeed);

            wantedRotation = Quaternion.LookRotation(target.TransformPoint(localTargetLookAtPosition) - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, lookLerpSpeed);
        }
    }
}