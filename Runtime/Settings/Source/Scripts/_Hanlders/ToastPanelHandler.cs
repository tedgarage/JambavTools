using UnityEngine.UI;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Jambav.Utilities;

namespace Jambav.Settings
{
    public class ToastPanelHandler : Singleton<ToastPanelHandler>
{
    [SerializeField] private Transform toastParent;
    [SerializeField] private CanvasGroup toastCanvasGroup;
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private Image successImage;
    [SerializeField] private Image errorImage;


    private Sequence toastSequence;
    private void SetToastMessage(string message)
    {
        toastText.text = message;

    }   
    
    public void ShowToastMessage(string message, bool isError = false)
    {
        print("Showing toast: " + message);
        print("isError: " + isError);
        if (toastSequence != null && toastSequence.IsActive())
        {
            toastSequence.Complete();
            toastSequence.Kill();
            
        }
        toastCanvasGroup.alpha = 0.0f;
        SetToastMessage(message);
        toastParent.gameObject.SetActive(true);
        errorImage.gameObject.SetActive(isError);
        successImage.gameObject.SetActive(!isError);
        toastSequence = DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() => { toastCanvasGroup.alpha=1.0f; }).AppendInterval(2f).OnComplete(() =>
        {
            HideToastMessage();
        });
    }
    private void HideToastMessage()
    {
        toastParent.gameObject.SetActive(false);
    }
}
}