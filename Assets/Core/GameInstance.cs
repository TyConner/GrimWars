using System.Collections;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance { get; private set; }

    [SerializeField] Camera cam;
    [SerializeField] GameObject player;

    PlayerController ply;

    Vector3 camStartLocalPos;
    Coroutine shakeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (player != null)
        {
            ply = player.GetComponent<PlayerController>();
        }

        if (cam != null)
        {
            camStartLocalPos = cam.transform.localPosition;
        }
    }

    public PlayerController GetPlayer()
    {
        return ply;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        if (cam == null) return;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            cam.transform.localPosition = camStartLocalPos;
        }

        shakeRoutine = StartCoroutine(CameraShake(duration, magnitude));
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        float timeLeft = duration;
        camStartLocalPos = cam.transform.localPosition;

        while (timeLeft > 0f)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            cam.transform.localPosition = camStartLocalPos + new Vector3(offset.x, offset.y, 0f);

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = camStartLocalPos;
        shakeRoutine = null;
    }
}