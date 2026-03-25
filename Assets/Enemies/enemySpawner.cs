using UnityEngine;
using System.Collections;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float respawnDelay = 3f;
    [SerializeField] private float spawnEffectDuration = 0.2f;

    public GameObject spawnedEnemy;
    private float respawnTimer = 0f;
    private SpriteRenderer spawnerRenderer;

    private void Awake()
    {
        spawnerRenderer = GetComponent<SpriteRenderer>();
        if (spawnerRenderer != null)
        {
            spawnerRenderer.enabled = false;
        }
    }
        void Start()
    {
        StartCoroutine(FlashSpawner());
    }

    private void Update()
    {
        if (spawnedEnemy == null)
        {
            respawnTimer += Time.deltaTime;

            if (respawnTimer >= respawnDelay)
            {
                StartCoroutine(FlashSpawner());
                respawnTimer = 0f;
            }
        }
        else
        {
            respawnTimer = 0f;
        }
    }

    private IEnumerator FlashSpawner()
    {
        if(enemyPrefab == null)
        {
            yield break;
        }

        if(spawnerRenderer != null)
        {
            spawnerRenderer.enabled = true;
        }

        yield return new WaitForSeconds(spawnEffectDuration);

        if(spawnerRenderer != null)
        {
            spawnerRenderer.enabled = false;
        }

        spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}
