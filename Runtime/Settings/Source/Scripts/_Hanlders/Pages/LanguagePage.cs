using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Jambav.Settings
{
    public class LanguagePage : BaseSettingsPage
{

    [SerializeField] Transform buttonHolder;
    [SerializeField] GameObject buttonPrefab;

    List<LanguageButton> deviceButtons;
    private int lastSelectedIndex = -1;
    private int selectedIndex = -1;
    string[] languages;


    void Awake()
    {

    }

    void init()
    {
        languages = SettingsManager.sharedInstance.LocalizationSelector.GetLocalNames();
        deviceButtons = new List<LanguageButton>();
        for (int i = 0; i < languages.Length; i++)
        {
            int index = i;
            LanguageButton button = Instantiate(buttonPrefab, buttonHolder).GetComponent<LanguageButton>();
            deviceButtons.Add(button);
            button.InitButton(languages[index],i, () =>
            {
                ButtonPressed(index);
            });
        }
    }

     public override void SetUpPage()
    {
       if (buttonHolder.childCount == 0)
        {
            init();
        }
        for (int i = 0; i < deviceButtons.Count; i++)
        {
            deviceButtons[i].currentState = ButtonState.Normal;
        }

       int currentIndex = SettingsManager.sharedInstance.LocalizationSelector.GetSelectedLocale(); 
        for (int i = 0; i < languages.Length; i++)
        {
            if (currentIndex == i)
            {
                lastSelectedIndex = i;
                selectedIndex = i;
                deviceButtons[i].currentState = ButtonState.Selected;
            }
            else
            {
                deviceButtons[i].currentState = ButtonState.Normal;
            }
        }
         HideActionButtons.Invoke();
    }

    private void ButtonPressed(int index)
    {
        if (selectedIndex != index)
        {
            if(selectedIndex != -1)
                deviceButtons[selectedIndex].currentState = ButtonState.Normal;
            deviceButtons[index].currentState = ButtonState.Selected;
            selectedIndex = index;
            ShowActionButtons.Invoke(false);

        }
       

    }

    public override void RevertActionDone()
    {
        for (int i = 0; i < deviceButtons.Count; i++)
        {
            deviceButtons[i].currentState = ButtonState.Normal;
        }
        if (lastSelectedIndex != -1)
        {
            deviceButtons[lastSelectedIndex].currentState = ButtonState.Selected;
        }
    }

    public override void SaveActionDone()
    {
        SettingsManager.sharedInstance.LocalizationSelector.ChangeLocale(selectedIndex);
         
        ToastPanelHandler.sharedInstance.ShowToastMessage("Language has been changed.");
    }
}
}