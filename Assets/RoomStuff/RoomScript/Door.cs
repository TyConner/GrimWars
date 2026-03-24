using UnityEngine;

public class Door : MonoBehaviour
{
    [HideInInspector] public Room currentRoom;
    [HideInInspector] public RoomManager roomManager;
    public Vector2Int doorDirection;

    private void Start()
    {
        if (currentRoom == null)
        {
            currentRoom = GetComponentInParent<Room>();
        }

        if (roomManager == null)
        {
            roomManager = FindObjectOfType<RoomManager>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.GetComponent<PlayerController>())
        {
            return;
        }
        Vector3 targetPosition = roomManager.GetRoomPositionFromDoor(currentRoom, doorDirection);
        other.transform.position = targetPosition;

        Room targetRoom = roomManager.GetRoomScriptAt(currentRoom.RoomIndex + doorDirection);

        if (targetRoom != null)
        {
            Camera.main.transform.position = new Vector3(targetRoom.transform.position.x, targetRoom.transform.position.y, Camera.main.transform.position.z);
        }
    }
}
