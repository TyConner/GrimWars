using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class ButtonTutorial : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI wText;
    [SerializeField] private TextMeshProUGUI aText;
    [SerializeField] private TextMeshProUGUI sText;
    [SerializeField] private TextMeshProUGUI dText;
    [SerializeField] private TextMeshProUGUI qText;

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialScreen; // parent panel of the tutorial UI

    private bool wPressed, aPressed, sPressed, dPressed, qPressed;
    private Coroutine deactivateCoroutine;

    void Start()
    {
        if (tutorialScreen == null)
            tutorialScreen = this.gameObject; // default to this object
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!wPressed && keyboard.wKey.wasPressedThisFrame)
        {
            wPressed = true;
            wText.color = Color.green;
        }

        if (!aPressed && keyboard.aKey.wasPressedThisFrame)
        {
            aPressed = true;
            aText.color = Color.green;
        }

        if (!sPressed && keyboard.sKey.wasPressedThisFrame)
        {
            sPressed = true;
            sText.color = Color.green;
        }

        if (!dPressed && keyboard.dKey.wasPressedThisFrame)
        {
            dPressed = true;
            dText.color = Color.green;
        }

        if (!qPressed && keyboard.qKey.wasPressedThisFrame)
        {
            qPressed = true;
            qText.color = Color.green;
        }

        // Start the 30-second timer only after all keys have been pressed
        if (AllKeysPressed() && deactivateCoroutine == null)
        {
            deactivateCoroutine = StartCoroutine(DeactivateAfterDelay(1f));
        }
    }

    private bool AllKeysPressed()
    {
        return wPressed && aPressed && sPressed && dPressed && qPressed;
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tutorialScreen.SetActive(false);
    }
}