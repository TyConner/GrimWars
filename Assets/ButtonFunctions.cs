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
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        isGameOver = true;
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


    public void Quit()
    {
#if UNITY_EDITOR
    EditorApplication.isPlaying = false; 
#else
        Application.Quit(); 
#endif
    }
}
