using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jambav.Settings
{
    public class MessagePanelHandler : MonoBehaviour
{
    public Action OnActionButtonClicked;
    [SerializeField] public Transform messageParent;
    [SerializeField] public TextMeshProUGUI messageText;
    [SerializeField] public TextMeshProUGUI messageTitle;
    [SerializeField] public Button actionButton;

    public void SetErrorMessage(string title, string message, string actionButtonText = "OK", Action action = null)
    {
        messageTitle.text = title;
        messageText.text = message;
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = actionButtonText;
        OnActionButtonClicked = action;
        ShowErrorMessage();

    }
    public void ShowErrorMessage()
    {
        messageParent.gameObject.SetActive(true);
    }
    public void HideErrorMessage()
    {
        if (messageParent != null && messageParent.gameObject != null)
        {
            messageParent.gameObject.SetActive(false);
        }
    }
    public void ActionButtonClicked()
    {
        // hide the panel first, then invoke the configured action (if any)
        if (messageParent != null && messageParent.gameObject != null)
        {
            messageParent.gameObject.SetActive(false);
        }

        OnActionButtonClicked?.Invoke();
    }
}
}