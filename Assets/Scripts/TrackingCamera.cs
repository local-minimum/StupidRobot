using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrackingCamera : MonoBehaviour
{
    [SerializeField]
    Transform trackTarget;

    [SerializeField, Range(0, 1)]
    float attack = 0.4f;

    [SerializeField]
    Vector3 offset;

    private void LateUpdate()
    {
        var optimalPosition = trackTarget.position + offset;

        transform.position = Vector3.Lerp(transform.position, optimalPosition, attack);
    }
}
