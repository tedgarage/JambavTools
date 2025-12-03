using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jambav.Settings
{
    public class DeviceNamePage : BaseSettingsPage
    {
        [SerializeField] List<DeviceColor> deviceColors;
        [SerializeField] Transform buttonHolder;
        [SerializeField] GameObject buttonPrefab;

        List<DeviceNameButton> deviceButtons;
        private int lastSelectedIndex = -1;
        private int selectedIndex = -1;


        void Awake()
        {

        }

        void init()
        {
            deviceButtons = new List<DeviceNameButton>();
            for (int i = 0; i < deviceColors.Count; i++)
            {
                int index = i;
                DeviceNameButton button = Instantiate(buttonPrefab, buttonHolder).GetComponent<DeviceNameButton>();
                deviceButtons.Add(button);
                button.InitButton(deviceColors[i], () =>
                {
                    ButtonPressed(index);
                });
            }
        }

        override public void SetUpPage()
        {

            string colorName = SettingsManager.sharedInstance.GetDeviceName();
            if (buttonHolder.childCount == 0)
            {
                init();
            }
            for (int i = 0; i < deviceButtons.Count; i++)
            {
                deviceButtons[i].currentState = ButtonState.Normal;
            }


            for (int i = 0; i < deviceColors.Count; i++)
            {
                if (deviceColors[i].name == colorName)
                {
                    lastSelectedIndex = i;
                    selectedIndex = i;
                    deviceButtons[i].currentState = ButtonState.Selected;
                    break;
                }
            }
        }

        private void ButtonPressed(int index)
        {
            if (selectedIndex != index)
            {
                if (selectedIndex != -1)
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
            if (lastSelectedIndex == selectedIndex)
            {
                ToastPanelHandler.sharedInstance.ShowToastMessage("Device name is not changed.");
                return;
            }
            else
            {
                bool itsFistTime = !PlayerPrefs.HasKey(CommonConstants.DeviceNameKey);
                PlayerPrefs.SetString(CommonConstants.DeviceNameKey, deviceColors[selectedIndex].name);
                PlayerPrefs.Save();
                if (itsFistTime)
                    SettingsManager.sharedInstance.DeviceNameFirstUpdated();
                lastSelectedIndex = selectedIndex;
                deviceButtons[selectedIndex].currentState = ButtonState.Selected;


                ToastPanelHandler.sharedInstance.ShowToastMessage("Device name is updated.");

            }
        }


    }
    [Serializable]
    public struct DeviceColor
    {
        public string name;
        public Color color;
    }
}