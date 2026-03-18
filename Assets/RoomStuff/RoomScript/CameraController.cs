using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform; // Or assign manually
    }

    public void MoveToRoom(Room room)
    {
        if (room != null)
        {
            // Move camera to room center
            cameraTransform.position = new Vector3(room.transform.position.x, room.transform.position.y, cameraTransform.position.z);
        }
    }
}
