using UnityEngine;
using TMPro;

public class TimedRoomUI : MonoBehaviour
{
    public static TimedRoomUI Instance;

    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI enemyText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show()
    {
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void UpdateUI(float time, int enemies)
    {
        timerText.text = "Time: " + Mathf.CeilToInt(time);
        enemyText.text = "Enemies: " + enemies;
    }
}
