using System;
using System.Collections.Generic;
using Jambav.Utilities;
using TMPro;
using UnityEngine;

namespace Jambav.Settings
{
    public class SettingsUIController : Singleton<SettingsUIController>
{
    [SerializeField] private Transform panel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Transform revertButton;
    [SerializeField] private Transform saveButton;
    [SerializeField] private TextMeshProUGUI titleText;

    private BaseSettingsPage[] pages;
    private MenuButton[] buttons;

    private BaseSettingsPage currentPage;

    void Start()
    {
        InitSettings();
    }

    public void InitSettings()
    {
        panel.gameObject.SetActive(false);
        var values = Enum.GetValues(typeof(SettingOption));
        int count = values.Length;
        pages = new BaseSettingsPage[count];
        buttons = new MenuButton[count];

        for (int i = 0; i < count; i++)
        {
            var opt = (SettingOption)values.GetValue(i);
            string optionName = opt.ToString();

            int index = i;
            MenuButton button = Instantiate(buttonPrefab, buttonsParent).GetComponent<MenuButton>();
            buttons[index] = button;

            button.InitButton(optionName, () => { 
                OnSettingButtonClicked(opt);
                
            });

            button.currentState = ButtonState.Disabled;
            if(opt ==  SettingOption.Language)
            {
                button.gameObject.SetActive(SettingsManager.sharedInstance.DoseLocalizationAvailable());
            }
            
        }

        foreach (BaseSettingsPage page in contentParent.GetComponentsInChildren<BaseSettingsPage>(true))
        {
            if (Enum.TryParse<SettingOption>(page.name, out var parsed))
            {
                page.ShowActionButtons = OnBasePageEdited;
                page.HideActionButtons = HideActionButtons;

                pages[(int)parsed] = page;
            }
            else
            {
                Debug.LogWarning($"SettingsUIController: Couldn't map page '{page.name}' to a SettingOption enum value.");
            }
        }
    }
    private void EnableButtons(List<SettingOption> _options)
    {
        var values = Enum.GetValues(typeof(SettingOption));
        int count = values.Length;
        MenuButton menuButton;
        for (int i = 0; i < count; i++)
        {
            menuButton = buttons[i];
            if (_options.Contains((SettingOption)values.GetValue(i)))
            {
                menuButton.currentState = ButtonState.Normal;
            }
            else
            {
                menuButton.currentState = ButtonState.Disabled;
            } 
        }
        OnSettingButtonClicked(_options[0]);
    }

    private void OnSettingButtonClicked(SettingOption enumValue)
    {
        titleText.text = enumValue.ToString().Replace("_", " ");
        int index = (int)enumValue;
        print("current index " + index);

        if (pages != null)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i]?.HideView();
            }
        }
        currentPage = pages[index];
        currentPage.SetUpPage();
        currentPage.ShowView();
        HideActionButtons();
        if (buttons != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].currentState == ButtonState.Disabled)
                    continue;   
                    
                buttons[i].currentState = ButtonState.Normal;
            }
        }

        buttons[index].currentState = ButtonState.Selected;
    }

    internal void HideSettingsPopUp()
    {
        panel.gameObject.SetActive(false);
    }

    internal void OpenEventCodePage()
    {
        print((int)SettingOption.Event_Code);
        EventCodePage eventCodePage = pages[(int)SettingOption.Event_Code] as EventCodePage;
        eventCodePage.SetUpPage();

        EnableButtons(new List<SettingOption> { SettingOption.Event_Code });
        panel.gameObject.SetActive(true);
    }
     internal void OpenDeviceNamePage()
    {
       DeviceNamePage eventCodePage = pages[(int)SettingOption.Device_Color] as DeviceNamePage;
        eventCodePage.SetUpPage();

        EnableButtons(new List<SettingOption> { SettingOption.Device_Color });
        panel.gameObject.SetActive(true);
    }

    private void OnBasePageEdited(bool showRevert)
    {
        saveButton.gameObject.SetActive(true);
        revertButton.gameObject.SetActive(showRevert);
    }
   private void HideActionButtons()
    {
        saveButton.gameObject.SetActive(false);
        revertButton.gameObject.SetActive(false);
   }
    public void SaveButtonAction()
    {
        if (currentPage == null)
            return;
        currentPage.SaveActionDone();
        
    }
    public void RevertButtonActon()
    {
        if (currentPage == null)
            return;
        currentPage.RevertActionDone();
    }
    internal void OpenSettings()
    {
        EnableButtons(new List<SettingOption> {  SettingOption.Event_Code,SettingOption.Device_Color, SettingOption.Shortcuts, SettingOption.Language });
        panel.gameObject.SetActive(true);
    }
    internal void OpenAllSettings()
    {
        EnableButtons(new List<SettingOption> { SettingOption.Device_Color, SettingOption.Event_Code, SettingOption.Shortcuts, SettingOption.Language });
        panel.gameObject.SetActive(true);
    }
}

    public enum SettingOption
    {
        Event_Code,
        Device_Color,
        Shortcuts,
        Language,
        // Utility_Settings
    }
}