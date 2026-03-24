using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoomData
{
    public GameObject prefab;
    public int weight = 1;
}

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Transform roomParent;
    [SerializeField] private RoomData[] roomPool;
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 7;

    int roomWidth = 20;
    int roomHeight = 12;

    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
    private int[,] roomGrid;
    private int roomCount;
    private bool generationComplete = false;
    private Vector2Int lastRoomIndex;

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;

        var initialRoom = Instantiate(startRoomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity, roomParent);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
        }
        else if (roomCount < minRooms)
        {
            Debug.Log("RoomCount was less than minimum room. Retrying.");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms");

            SpawnBossRoom();

            generationComplete = true;
        }
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomCount >= maxRooms)
        {
            return false;
        }

        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
        {
            return false;
        }

        if (CountAdjacentRooms(roomIndex) > 1)
        {
            return false;
        }

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(GetWeightedRandomRoom(), GetPositionFromGridIndex(roomIndex), Quaternion.identity, roomParent);
        newRoom.name = $"Room-{roomCount}";
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y);

        Room roomScript = newRoom.GetComponent<Room>();

        if (Random.value < 0.2f && roomCount > 2)
        {
            roomScript.isTimedRoom = true;
        }

        return true;
    }

    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            newRoomScript.OpenDoors(Vector2Int.left);
            leftRoomScript.OpenDoors(Vector2Int.right);
        }

        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            newRoomScript.OpenDoors(Vector2Int.right);
            rightRoomScript.OpenDoors(Vector2Int.left);
        }

        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            newRoomScript.OpenDoors(Vector2Int.down);
            bottomRoomScript.OpenDoors(Vector2Int.up);
        }

        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            newRoomScript.OpenDoors(Vector2Int.up);
            topRoomScript.OpenDoors(Vector2Int.down);
        }
    }

    public Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
        {
            return roomObject.GetComponent<Room>();
        }
        return null;
    }

    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++;
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++;
        if (y > 0 && roomGrid[x, y - 1] != 0) count++;
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++;

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2));
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; ++x)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight));
            }
        }
    }

    public Vector3 GetRoomPositionFromDoor(Room currentRoom, Vector2Int doorDirection)
    {
        Vector2Int targetIndex = currentRoom.RoomIndex + doorDirection;
        Room targetRoom = GetRoomScriptAt(targetIndex);

        if (targetRoom != null)
        {
            return targetRoom.transform.position;
        }
        else
        {
            Debug.LogWarning($"No room exists at {targetIndex}");
            return currentRoom.transform.position;
        }
    }

    private GameObject GetWeightedRandomRoom()
    {
        int totalWeight = 0;

        foreach (var room in roomPool)
            totalWeight += room.weight;

        int randomValue = Random.Range(0, totalWeight);

        foreach (var room in roomPool)
        {
            if (randomValue < room.weight)
                return room.prefab;

            randomValue -= room.weight;
        }

        return roomPool[0].prefab; // fallback
    }

    private void SpawnBossRoom()
    {
        Room oldRoom = GetRoomScriptAt(lastRoomIndex);
        if (oldRoom == null) return;

        Vector3 pos = oldRoom.transform.position;

        roomObjects.Remove(oldRoom.gameObject);
        Destroy(oldRoom.gameObject);

        GameObject bossRoom = Instantiate(bossRoomPrefab, pos, Quaternion.identity, roomParent);
        bossRoom.GetComponent<Room>().RoomIndex = lastRoomIndex;

        roomObjects.Add(bossRoom);

        OpenDoors(bossRoom, lastRoomIndex.x, lastRoomIndex.y);
    }
}