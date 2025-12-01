using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Jambav.Utilities;

namespace Jambav.Settings
{
    public class SettingsInputController : Singleton<SettingsInputController>
{


    public Action OnSettingToggled;

    [Header("Restart Game")]
    private bool restartedCompleted = true;
    private int currentPressedButtonIndex = -1;

    // Game restart button Sequence
    private List<OVRInput.RawButton> buttonOrderVRSeqList = new List<OVRInput.RawButton> { OVRInput.RawButton.B, OVRInput.RawButton.A, OVRInput.RawButton.B, OVRInput.RawButton.A, OVRInput.RawButton.RHandTrigger };
    private List<KeyCode> buttonOrderEditorSeqList = new List<KeyCode> { KeyCode.T, KeyCode.Y, KeyCode.T, KeyCode.Y, KeyCode.Return };

    // Settings ToggleButtonSeq
    private List<OVRInput.RawButton> toggleButtonVRDevice = new List<OVRInput.RawButton> { OVRInput.RawButton.A,
                OVRInput.RawButton.RHandTrigger};
    private List<KeyCode> toggleButtonForEditor = new List<KeyCode> {    KeyCode.A,
                KeyCode.S, }; 


    private float waitingTimeForNextButtonPress;
    private readonly float defaultWaitingTimeForNextButtonPress = 2f;


    [Header("LongPress")]
    private bool longPress = true;
    private float clickDuration = 1;
    private bool thumpPressing = false;
    private float totalThumpDownTime = 0;
    private bool comboPressing = false;
    private float totalComboDownTime = 0f;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        RestartGameInput();
        if(SettingsManager.sharedInstance.canViewSettings)
            LongPressThumbAndTrigger();

    }

    private bool IsVR()
    {
        try
        {
            return OVRManager.isHmdPresent;
        }
        catch
        {
            return false;
        }
    }

    private void RestartGameInput()
    {
        if (!restartedCompleted) return;

            // Split logic between VR (OVRInput) and Editor/standalone (KeyCode/Input)
            if (IsVR())
            {
                // VR device input using OVRInput and buttonOrderVRSeqList
                if (currentPressedButtonIndex == -1)
                {
                    if (OVRInput.GetDown(buttonOrderVRSeqList[0]))
                    {
                        Debug.Log("Dennis --- VR Button Pressed -" + currentPressedButtonIndex);
                        currentPressedButtonIndex = 0;
                        waitingTimeForNextButtonPress = defaultWaitingTimeForNextButtonPress;
                    }
                }
                else
                {
                    waitingTimeForNextButtonPress -= Time.deltaTime;
                    if (waitingTimeForNextButtonPress < 0)
                    {
                        currentPressedButtonIndex = -1;
                        Debug.Log("Dennis --- Time done");
                    }

                    if (OVRInput.GetDown(buttonOrderVRSeqList[Mathf.Min(currentPressedButtonIndex + 1, buttonOrderVRSeqList.Count - 1)]))
                    {
                        currentPressedButtonIndex++;
                        waitingTimeForNextButtonPress = defaultWaitingTimeForNextButtonPress;

                        if (buttonOrderVRSeqList.Count - 1 == currentPressedButtonIndex)
                        {
                            StartCoroutine(RestartGame());
                            currentPressedButtonIndex = -1;
                        }
                    }
                    else if (OVRInput.GetDown(OVRInput.RawButton.Any))
                    {
                        // any other VR button - reset sequence
                        currentPressedButtonIndex = -1;
                    }
                }
            }
            else
            {
                // Editor / Standalone input using KeyCode list
                if (currentPressedButtonIndex == -1)
                {
                    if (Input.GetKeyDown(buttonOrderEditorSeqList[0]))
                    {
                        Debug.Log("Dennis --- Button Pressed -" + currentPressedButtonIndex);
                        currentPressedButtonIndex = 0;
                        waitingTimeForNextButtonPress = defaultWaitingTimeForNextButtonPress;
                    }
                }
                else
                {
                    waitingTimeForNextButtonPress -= Time.deltaTime;
                    if (waitingTimeForNextButtonPress < 0)
                    {
                        currentPressedButtonIndex = -1;
                        Debug.Log("Dennis --- Time done");
                    }

                    if (Input.GetKeyDown(buttonOrderEditorSeqList[Mathf.Min(currentPressedButtonIndex + 1, buttonOrderEditorSeqList.Count - 1)]))
                    {
                        currentPressedButtonIndex++;
                        waitingTimeForNextButtonPress = defaultWaitingTimeForNextButtonPress;

                        if (buttonOrderEditorSeqList.Count - 1 == currentPressedButtonIndex)
                        {
                            StartCoroutine(RestartGame());
                            currentPressedButtonIndex = -1;
                        }
                    }
                    else if (Input.anyKeyDown)
                    {
                        // wrong key - reset
                        currentPressedButtonIndex = -1;
                    }
                }
            }
        }


    public void LongPressThumpButton()
    {
        if (!longPress)
            return;

        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            HandleLongPressMac(KeyCode.Space, ref thumpPressing, ref totalThumpDownTime, clickDuration, () =>
            {
                Debug.Log("thump long press (Mac test)");
            });
        #else
            HandleLongPress(OVRInput.RawButton.RThumbstick, ref thumpPressing, ref totalThumpDownTime, clickDuration, () =>
            {
                Debug.Log("thump long press");
            });
        #endif
    }

    public void LongPressThumbAndTrigger()
    {
        if (!longPress)
            return;

        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            HandleSimultaneousLongPressMac(
                toggleButtonForEditor[0],
               toggleButtonForEditor[1],
                ref comboPressing,
                ref totalComboDownTime,
                clickDuration,
                () => { OnSettingToggled?.Invoke(); });
        #else
            HandleSimultaneousLongPress(
                toggleButtonVRDevice[0],
                 toggleButtonVRDevice[1],
                ref comboPressing,
                ref totalComboDownTime,
                clickDuration,
                () => { OnSettingToggled?.Invoke(); });
        #endif
    }

    private bool HandleLongPress(OVRInput.RawButton button, ref bool isPressing, ref float totalDownTime, float duration, Action onLongPress)
    {
        if (OVRInput.GetDown(button))
        {
            totalDownTime = 0f;
            isPressing = true;
        }

        if (isPressing && OVRInput.Get(button))
        {
            totalDownTime += Time.deltaTime;
            if (totalDownTime >= duration)
            {
                isPressing = false;
                onLongPress?.Invoke();
                return true;
            }
        }

        if (isPressing && OVRInput.GetUp(button))
        {
            isPressing = false;
        }

        return false;
    }

    private bool HandleLongPressMac(KeyCode key, ref bool isPressing, ref float totalDownTime, float duration, Action onLongPress)
    {
        if (Input.GetKeyDown(key))
        {
            totalDownTime = 0f;
            isPressing = true;
        }

        if (isPressing && Input.GetKey(key))
        {
            totalDownTime += Time.deltaTime;
            if (totalDownTime >= duration)
            {
                isPressing = false;
                onLongPress?.Invoke();
                return true;
            }
        }

        if (isPressing && Input.GetKeyUp(key))
        {
            isPressing = false;
        }

        return false;
    }

    private bool HandleSimultaneousLongPress(OVRInput.RawButton buttonA, OVRInput.RawButton buttonB, ref bool isPressing, ref float totalDownTime, float duration, Action onLongPress)
    {
        if (!isPressing)
        {
            if (OVRInput.GetDown(buttonA) || OVRInput.GetDown(buttonB))
            {
                if (OVRInput.Get(buttonA) && OVRInput.Get(buttonB))
                {
                    totalDownTime = 0f;
                    isPressing = true;
                }
            }
            else if (OVRInput.Get(buttonA) && OVRInput.Get(buttonB))
            {
                totalDownTime = 0f;
                isPressing = true;
            }
        }

        if (isPressing)
        {
            if (OVRInput.Get(buttonA) && OVRInput.Get(buttonB))
            {
                totalDownTime += Time.deltaTime;
                if (totalDownTime >= duration)
                {
                    isPressing = false;
                    onLongPress?.Invoke();
                    return true;
                }
            }
            else
            {
                isPressing = false;
            }
        }

        if (OVRInput.GetUp(buttonA) || OVRInput.GetUp(buttonB))
        {
            isPressing = false;
        }

        return false;
    }

    private bool HandleSimultaneousLongPressMac(KeyCode keyA, KeyCode keyB, ref bool isPressing, ref float totalDownTime, float duration, Action onLongPress)
    {
        if (!isPressing)
        {
            if (Input.GetKeyDown(keyA) || Input.GetKeyDown(keyB))
            {
                if (Input.GetKey(keyA) && Input.GetKey(keyB))
                {
                    totalDownTime = 0f;
                    isPressing = true;
                    Debug.Log("Simultaneous press started");
                }
            }
            else if (Input.GetKey(keyA) && Input.GetKey(keyB))
            {
                totalDownTime = 0f;
                isPressing = true;
                Debug.Log("Both keys held");
            }
        }

        if (isPressing)
        {
            if (Input.GetKey(keyA) && Input.GetKey(keyB))
            {
                totalDownTime += Time.deltaTime;
                // Debug.Log($"Holding... {totalDownTime:F2}s / {duration}s");
                if (totalDownTime >= duration)
                {
                    isPressing = false;
                    onLongPress?.Invoke();
                    Debug.Log("Long press");
                    return true;
                }
            }
            else
            {
                isPressing = false;
                Debug.Log("Key released - reset");
            }
        }

        if (Input.GetKeyUp(keyA) || Input.GetKeyUp(keyB))
        {
            isPressing = false;
        }

        return false;
    }



    private void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }


    private IEnumerator RestartGame()
    {
        print("Restarting Game");
        restartedCompleted = false;
        SettingsManager.sharedInstance.OnRestartStart?.Invoke();
        yield return new WaitForSeconds(1f);
        ReloadScene();
        yield return new WaitForSeconds(2);
        restartedCompleted = true;
        
        if(SettingsManager.sharedInstance.DoseGameReadyToPlay())
        {
            SettingsManager.sharedInstance.GameReadyToPlay?.Invoke();
        }
    }
}
}
