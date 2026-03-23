using UnityEngine;

public class Door : MonoBehaviour
{
    [HideInInspector] public Room currentRoom;
    [HideInInspector] public RoomManager roomManager;
    
    [SerializeField] GameObject TimedRoomIndicator;

    public Vector2Int doorDirection;

    private void Start()
    {
        if (currentRoom == null)
        {
            currentRoom = GetComponentInParent<Room>();
        }

        if (roomManager == null)
        {
            roomManager = FindFirstObjectByType<RoomManager>();
        }

        Room targetRoom = roomManager.GetRoomScriptAt(currentRoom.RoomIndex + doorDirection);

        if (targetRoom != null && targetRoom.isTimedRoom)
        {
            TimedRoomIndicator.SetActive(true);
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

        if (currentRoom.isTimedRoom && !currentRoom.challengeCompleted)
        {
            return;
        }

        if (targetRoom.isTimedRoom)
        {
            targetRoom.StartChallenge();
        }
    }
}
