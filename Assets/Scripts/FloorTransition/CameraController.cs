using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float transitionTime = 1.0f; // カメラの移動にかける時間

    public void MoveCamera(Vector3 newCameraPosition)
    {
        StartCoroutine(SmoothMoveCamera(newCameraPosition));
    }

    IEnumerator SmoothMoveCamera(Vector3 newCameraPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < transitionTime)
        {
            transform.position = Vector3.Lerp(startingPosition, newCameraPosition, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = newCameraPosition;
    }
}
