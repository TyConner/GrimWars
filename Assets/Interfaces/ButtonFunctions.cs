using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;
#if UNITY_EDITOR
#endif

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField] private InputActionReference pause;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject winScreenUI;
    [SerializeField] private GameObject tutorialScreen;
    [SerializeField] private PlayerController player;

    bool isPaused = false;
    bool isGameOver = false;

    void OnEnable()
    {
        if (pause != null)
            pause.action.Enable();

        pause.action.performed += TogglePause;
    }

    void OnDisable()
    {
        if (pause != null)
            pause.action.performed -= TogglePause;
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (isGameOver || (pauseMenuUI != null && pauseMenuUI.activeInHierarchy))
            return;

        if (isPaused)
            Resume();
        else
            Pause();
    }


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        MusicManager.Instance.PlaySceneMusic();
        if (player != null)
            player.PlayResumeSFX();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        MusicManager.Instance.PauseMusic();
        if (player != null)
            player.PlayPauseSFX();
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        isGameOver = true;
        MusicManager.Instance.PlayMusic(MusicManager.Instance.loseMusic);
    }


    public void Restart()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("SampleScene");
    }

    public void ShowTutorial()
    {
        tutorialScreen.SetActive(true);
        Time.timeScale = 0f;
        MusicManager.Instance.PlayMusic(MusicManager.Instance.tutorialMusic);
    }

    public void HideTutorial()
    {
        tutorialScreen.SetActive(false);
        MusicManager.Instance.PopMusic();
        MusicManager.Instance.StopMusic();
    }

    public void LoadNextScene()
    {
        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        SceneManager.LoadScene(nextIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
    EditorApplication.isPlaying = false; 
#else
        Application.Quit(); 
#endif
    }
}
