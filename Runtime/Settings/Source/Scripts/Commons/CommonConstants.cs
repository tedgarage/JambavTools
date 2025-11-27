using UnityEngine;

namespace Jambav.Settings
{
    public static class CommonConstants
{
    public static Color SettingsPrimaryColor => new Color(19 / 255f, 91 / 255f, 230 / 255f, 1);
    public const string EventCodeKey = "EventCodeKey";
    public const string ExpireDateKey = "ExpireDateKey";
    public const string DeviceNameKey = "DeviceNameKey";
    public const string BundleNameKey = "BundleNameKey";
    public const string gameplayDataPathKey = "gameplayDataPathKey";
    public const string LanguageKey = "LanguageKey";
    public const string TutorialEnableKey = "TutorialEnableKey";
    public const string GameplayDataPathFormat = "/GAMEPLAY_DATA/{0}/";
}
}