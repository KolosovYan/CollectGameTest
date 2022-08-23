using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _cameraOffset;

    private void Update()
    {
        transform.position = new Vector3(_cameraTarget.position.x, transform.position.y, _cameraTarget.position.z - _cameraOffset);
    }
}
