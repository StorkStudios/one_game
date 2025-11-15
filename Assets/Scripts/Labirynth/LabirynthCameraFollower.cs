using UnityEngine;

public class LabirynthCameraFollower : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private void Update()
    {
        Vector3 playerPosition = LabirynthPlayerController.Instance.transform.position;

        playerPosition.z = transform.position.z;

        transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
    }
}
