using System;
using UnityEngine;

namespace Jambav.Settings
{
    public abstract class BaseSettingsPage : MonoBehaviour
{
    public Action<bool> ShowActionButtons;
    public Action HideActionButtons;

    [SerializeField] protected Transform panelTransform;
    


    public abstract void SaveActionDone();
    public abstract void RevertActionDone();
    public abstract void SetUpPage();
    
    public  void ShowView()
    {
        panelTransform.gameObject.SetActive(true);
    }
    public  void HideView()
    {
        panelTransform.gameObject.SetActive(false);
    }
}
}