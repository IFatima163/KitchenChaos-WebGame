using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyboardMoveUpKeyText;
    [SerializeField] private TextMeshProUGUI keyboardMoveDownKeyText;
    [SerializeField] private TextMeshProUGUI keyboardMoveLeftKeyText;
    [SerializeField] private TextMeshProUGUI keyboardMoveRightKeyText;
    [SerializeField] private TextMeshProUGUI keyboardInteractKeyText;
    [SerializeField] private TextMeshProUGUI keyboardInteractAltKeyText;
    [SerializeField] private TextMeshProUGUI keyboardPauseKeyText;
    [SerializeField] private TextMeshProUGUI gamepadMoveKeyText;
    [SerializeField] private TextMeshProUGUI gamepadInteractKeyText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAltKeyText;
    [SerializeField] private TextMeshProUGUI gamepadPauseKeyText;

    private void Start()
    {
        GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        
        UpdateVisual();
    }

    private void GameInput_OnBindingRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Hide(); //Hide tutorial if game starting
        }
    }

    private void UpdateVisual()
    {
        keyboardMoveUpKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        keyboardMoveDownKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        keyboardMoveLeftKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        keyboardMoveRightKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        keyboardInteractKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        keyboardInteractAltKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        keyboardPauseKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamepadInteractKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        gamepadInteractAltKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
        gamepadPauseKeyText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
