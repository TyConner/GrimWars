using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] TextMeshPro playText;
    [SerializeField] TextMeshPro quitText;

    [SerializeField] InputActionReference navigateAction;
    [SerializeField] InputActionReference submitAction;

    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color highlightedColor = Color.yellow;

    int selectedIndex = 0;
    float navigateCooldown = 0.15f;
    float navigateTimer = 0f;

    void OnEnable()
    {
        if (navigateAction != null)
        {
            navigateAction.action.Enable();
        }

        if (submitAction != null)
        {
            submitAction.action.Enable();
            submitAction.action.performed += OnSubmit;
        }

        UpdateVisuals();
    }

    void OnDisable()
    {
        if (submitAction != null)
        {
            submitAction.action.performed -= OnSubmit;
            submitAction.action.Disable();
        }

        if (navigateAction != null)
        {
            navigateAction.action.Disable();
        }
    }

    void Update()
    {
        if (navigateAction == null)
        {
            return;
        }

        if (navigateTimer > 0f)
        {
            navigateTimer -= Time.unscaledDeltaTime;
        }

        Vector2 navigateInput = navigateAction.action.ReadValue<Vector2>();

        if (navigateTimer <= 0f)
        {
            if (navigateInput.y > 0.5f)
            {
                MoveSelection(-1);
            }
            else if (navigateInput.y < -0.5f)
            {
                MoveSelection(1);
            }
        }
    }

    void MoveSelection(int direction)
    {
        selectedIndex += direction;

        if (selectedIndex < 0)
        {
            selectedIndex = 1;
        }
        else if (selectedIndex > 1)
        {
            selectedIndex = 0;
        }

        navigateTimer = navigateCooldown;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (playText != null)
        {
            playText.color = selectedIndex == 0 ? highlightedColor : normalColor;
        }

        if (quitText != null)
        {
            quitText.color = selectedIndex == 1 ? highlightedColor : normalColor;
        }
    }

    void OnSubmit(InputAction.CallbackContext context)
    {
        if (selectedIndex == 0)
        {
            SceneManager.LoadScene("Grimoire Selection");
        }
        else if (selectedIndex == 1)
        {
            Debug.Log("Quit selected");
            Application.Quit();
        }
    }
}