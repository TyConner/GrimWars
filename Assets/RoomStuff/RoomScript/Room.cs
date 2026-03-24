using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [Header("TimedRoom Settings")]
    [SerializeField] private GameObject rewardPrefab;
    public bool isTimedRoom = false;
    public bool challengeCompleted = false;
    private bool challengeActive = false;
    public float challengeTime = 60f;
    private float timer;
    public int enemiesRemaining;

    [Header("Enemy Spawners")]
    [SerializeField] private enemySpawner[] spawners;

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

        TimedRoomUI.Instance.Show();

        MusicManager.Instance.PlayMusic(MusicManager.Instance.timedRoomMusic);

        foreach (var spawner in spawners)
        {
            if (spawner != null)
                spawner.gameObject.SetActive(true);
        }

        StartCoroutine(ChallengeTimer());
   
        TimedRoomUI.Instance?.Show();
        TimedRoomUI.Instance?.UpdateUI(timer, GetActiveEnemyCount());
        MusicManager.Instance?.PlayChallengeMusic();
    }

    private IEnumerator ChallengeTimer()
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            TimedRoomUI.Instance?.UpdateUI(timer, GetActiveEnemyCount());

            if (GetActiveEnemyCount() <= 0)
            {
                CompleteChallenge();
                yield break;
            }

            yield return null;
        }
        FailChallenge();
    }

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void OnEnemyKilled()
    {
        enemiesRemaining--;
    }

    private void CompleteChallenge()
    {
        challengeActive = false;
        challengeCompleted = true;

        UnlockDoors();
        TimedRoomUI.Instance.Hide();

        if (rewardPrefab != null)
            Instantiate(rewardPrefab, transform.position, Quaternion.identity);

        MusicManager.Instance?.StopChallengeMusicAndRestorePrevious();

        TimedRoomUI.Instance?.Hide();
    }

    private void FailChallenge()
    {
        challengeActive = false;

        UnlockDoors();

        MusicManager.Instance?.StopChallengeMusicAndRestorePrevious();

        TimedRoomUI.Instance?.Hide();
    }

    public void OpenDoor(Vector2Int direction)
    { 
        if (direction == Vector2Int.up) topDoor?.SetActive(true);
        if (direction == Vector2Int.down) bottomDoor?.SetActive(true);
        if (direction == Vector2Int.left) leftDoor?.SetActive(true);
        if (direction == Vector2Int.right) rightDoor?.SetActive(true);
    }

    private void UnlockDoors()
    {
        topDoor?.SetActive(true);
        bottomDoor?.SetActive(true);
        leftDoor?.SetActive(true);
        rightDoor?.SetActive(true);
    }

    private int GetActiveEnemyCount()
    {
        int count = 0;
        foreach (var spawner in spawners)
        {
            if (spawner != null && spawner.spawnedEnemy != null)
                count++;
        }
        return count;
    }

}
