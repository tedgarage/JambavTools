using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jambav.Settings
{
    public class DeviceNameButton : MonoBehaviour
{

    [SerializeField] private Image buttonBgImage;
    [SerializeField] private TextMeshProUGUI buttonNameText;
    [SerializeField]private CanvasGroup selectionObject;
    private DeviceColor buttonColor;

   
   
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
                selectionObject.alpha = 0f;
                break;
            case ButtonState.Hovered:
                 selectionObject.alpha = 0.1f;
                break;
            case ButtonState.Selected:
                 selectionObject.alpha = 1f;
                break;
            case ButtonState.Disabled:
                selectionObject.alpha =0f;
                break;
        }
    }

   public void InitButton(DeviceColor _buttonColor,  Action _onClickAction)
    {
        buttonColor = _buttonColor; 
       
        GetComponentInChildren<TextMeshProUGUI>().text = _buttonColor.name;
        buttonBgImage.color  = _buttonColor.color;
        currentState = ButtonState.Normal;
        gameObject.name = _buttonColor.name + "Button";
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            _onClickAction?.Invoke();
        });
    }
}
}
