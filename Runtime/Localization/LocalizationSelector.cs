using System.Collections;
using UnityEngine;
#if LOCALIZATION_AVAILABLE
using UnityEngine.Localization.Settings;
#endif


namespace Jambav.Localization
{
    #if LOCALIZATION_AVAILABLE
    public class LocalizationSelector : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        #region PLAYER PREFS

        private string localeKey = "LocaleKey";
        public int GetSelectedLocale()
        {
            int id = PlayerPrefs.GetInt(localeKey, 0);
            return id;
        }

        private void SaveLocale(int id)
        {
            PlayerPrefs.SetInt(localeKey, id);
            PlayerPrefs.Save();
        }

        #endregion

        #region EXTERNAL METHODS

        public void LoadLocale()
        {
            int id = GetSelectedLocale();
            ChangeLocale(id);
        }

        public void ChangeLocale(int localeId)
        {
             StartCoroutine(SetLocale(localeId));
        }

        #endregion

        private IEnumerator SetLocale(int localeId)
        {
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
            SaveLocale(localeId);
        }
        public string[] GetLocalNames()
        {
            string[] names = new string[LocalizationSettings.AvailableLocales.Locales.Count];
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                names[i] = LocalizationSettings.AvailableLocales.Locales[i].name;
            }
            return names;
        }

    }
    #endif
}