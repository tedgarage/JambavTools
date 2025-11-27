using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jambav.Settings
{
    public class LanguageButton : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI buttonNameText;
    [SerializeField] private CanvasGroup selectionObject;
    
    private int index;

   
   
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

   public void InitButton(string _language,int _index,  Action _onClickAction)
    {
       index = _index;
       
        GetComponentInChildren<TextMeshProUGUI>().text = _language;
        currentState = ButtonState.Normal;
        gameObject.name =_language ;
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            _onClickAction?.Invoke();
        });
    }
}
}
