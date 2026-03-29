using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [HideInInspector] public Room currentRoom;
    [HideInInspector] public RoomManager roomManager;

    public Vector2Int doorDirection;

    float hOffset = 7f;
    float vOffset = 3f;
    [SerializeField] private float teleportCooldown = 0.2f;

    private static bool isTeleporting = false;

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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting)
        {
            return;
        }

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        Vector3 targetPosition = roomManager.GetRoomPositionFromDoor(currentRoom, doorDirection);

        if (doorDirection.x == -1)
        {
            targetPosition += Vector3.right * hOffset;
        }
        else if (doorDirection.x == 1)
        {
            targetPosition += Vector3.left * hOffset;
        }
        else if (doorDirection.y == 1)
        {
            targetPosition += Vector3.down * vOffset;
        }
        else if (doorDirection.y == -1)
        {
            targetPosition += Vector3.up * vOffset;
        }

        isTeleporting = true;
        other.transform.position = targetPosition;

        Room targetRoom = roomManager.GetRoomScriptAt(currentRoom.RoomIndex + doorDirection);

        if (targetRoom != null && Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(
                targetRoom.transform.position.x,
                targetRoom.transform.position.y,
                Camera.main.transform.position.z
            );
        }

        StartCoroutine(ResetTeleportLock());
    }

    private IEnumerator ResetTeleportLock()
    {
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }
}