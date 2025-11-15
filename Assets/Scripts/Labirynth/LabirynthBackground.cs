using UnityEngine;

public class LabirynthBackground : MonoBehaviour
{
    [SerializeField]
    private float scalar;

    private void LateUpdate()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition *= (1 + scalar);
        cameraPosition.z = transform.position.z;
        transform.position = cameraPosition;
    }
}
