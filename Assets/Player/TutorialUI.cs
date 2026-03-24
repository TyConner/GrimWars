using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{  
    [SerializeField] private GameObject tutorialScreen;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text pageText;
    [SerializeField] private Transform screenContainer;

    private GameObject[] tutorialScreens;
    private int currentIndex = 0;

    

    void Awake()
    {
        int count = screenContainer.childCount;
        tutorialScreens = new GameObject[count];
        MusicManager.Instance.PlayMusic(MusicManager.Instance.tutorialMusic);

        for (int i = 0; i < count; i++)
        {
            tutorialScreens[i] = screenContainer.GetChild(i).gameObject;
        }
    }

    void OnEnable()
    {
        currentIndex = 0;
        UpdateScreen();
    }

    public void Next()
    {
        if (currentIndex < tutorialScreens.Length - 1)
        {
            currentIndex++;
            UpdateScreen();
        }
    }

    public void Previous()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateScreen();
        }
    }

    public void BackToPause()
    {
        tutorialScreen.SetActive(false);
        pauseMenuUI.SetActive(true);
        MusicManager.Instance.StopMusic();
    }

    private void UpdateScreen()
    {
        for (int i = 0; i < tutorialScreens.Length; i++)
        {
            tutorialScreens[i].SetActive(i == currentIndex);
        }

        pageText.text = (currentIndex + 1) + "/" + tutorialScreens.Length;
        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < tutorialScreens.Length - 1;
    }
}
