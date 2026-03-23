using UnityEngine;
using System.Collections.Generic;
public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    
    [Header("Timed Room")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject rewardPrefab;

   
    public bool isTimedRoom = false;
    public bool challengeCompleted = false;
    private bool challengeActive = false;
    public float challengeTime = 60f;
    private float timer;
    public int enemiesRemaining;
    private PlayerController player;

    public Vector2Int RoomIndex { get; set; }

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public void OpenDoors(Vector2Int direction)
    {
        if (direction == Vector2Int.up){
            topDoor.SetActive(true);
        }

        if (direction == Vector2Int.down)
        {
            bottomDoor.SetActive(true);
        }

        if (direction == Vector2Int.right)
        {
            rightDoor.SetActive(true);
        }

        if (direction == Vector2Int.left)
        {
            leftDoor.SetActive(true);
        }
    }

    public void StartChallenge()
    {
        if (!isTimedRoom || challengeActive) return;

        challengeActive = true;
        timer = challengeTime;

        SpawnEnemies();
        LockDoors(true);

        TimedRoomUI.Instance.Show();

        MusicManager.Instance.PlayMusic(MusicManager.Instance.timedRoomMusic);
    }

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (!challengeActive) return;

        timer -= Time.deltaTime;
        TimedRoomUI.Instance.UpdateUI(timer, enemiesRemaining);

        if (enemiesRemaining <= 0)
        {
            CompleteChallenge(true);
        }
        else if (timer <= 0)
        {
            CompleteChallenge(false);
        }
    }

    void SpawnEnemies()
    {
        int count = Random.Range(10, 16);
        enemiesRemaining = count;

        for (int i = 0; i < count; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
            spawnedEnemies.Add(enemy);

            enemy.GetComponent<EnemyController>().OnDeath += OnEnemyKilled;
        }
    }

    void OnEnemyKilled()
    {
        enemiesRemaining--;
    }

    void CompleteChallenge(bool success)
    {
        challengeActive = false;
        challengeCompleted = success;

        LockDoors(false);
        TimedRoomUI.Instance.Hide();

        if (success)
        {
            Instantiate(rewardPrefab, transform.position, Quaternion.identity);
        }

        MusicManager.Instance.PopMusic();
    }

    void LockDoors(bool locked)
    {
        topDoor.GetComponent<Collider2D>().enabled = !locked;
        bottomDoor.GetComponent<Collider2D>().enabled = !locked;
        leftDoor.GetComponent<Collider2D>().enabled = !locked;
        rightDoor.GetComponent<Collider2D>().enabled = !locked;
    }
}
