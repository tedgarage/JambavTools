using System;
using Jambav.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jambav.Settings
{
    public class MenuButton : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Image buttonBgImage;
   
    public ButtonState currentState {  get => _state; set
        {
            _state = value;
            OnStateChanged();
        }   
    }
    private ButtonState _state = ButtonState.Normal;
    private void OnStateChanged()
    {
        switch (_state)
        {
            case ButtonState.Normal:
                 canvasGroup.alpha = 1f;
                buttonBgImage.color = Color.clear;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true; 
                break;
            case ButtonState.Hovered:
                canvasGroup.alpha = 1f;
                buttonBgImage.color = CommonConstants.SettingsPrimaryColor.WithAlpha(0.2f);
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true; 
                break;
            case ButtonState.Selected:
                 canvasGroup.alpha = 1f;
                buttonBgImage.color = CommonConstants.SettingsPrimaryColor;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                break;
            case ButtonState.Disabled:
                canvasGroup.alpha = 0.1f;
                buttonBgImage.color = Color.clear;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                break;
        }
    }

   public void InitButton(string buttonName, Action onClickAction)
    {
        canvasGroup = GetComponent<CanvasGroup>();
        buttonBgImage = GetComponent<Image>();
        GetComponentInChildren<TextMeshProUGUI>().text = buttonName.Replace("_", " ");
        transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Icons/{buttonName}_Icon");
        currentState = ButtonState.Normal;
        gameObject.name = buttonName + "Button";
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            onClickAction?.Invoke();
        });
    }
   
}
 public enum ButtonState
    {
        Normal,
        Hovered,
        Selected,
        Disabled,
    }
}