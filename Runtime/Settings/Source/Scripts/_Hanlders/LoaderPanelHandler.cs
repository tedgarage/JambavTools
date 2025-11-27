using Jambav.Utilities;
using NUnit.Framework;
using TMPro;
using UnityEngine;

namespace Jambav.Settings
{
    public class LoaderPanelHandler : Singleton<LoaderPanelHandler>
    {
        [SerializeField] private Transform loadingPanel;
        [SerializeField] private TextMeshProUGUI loadingText;

        public void ShowLoadingPanel(LoadingType loadingType)
        {
            loadingText.text = GetLoadingText(loadingType);
            loadingPanel.gameObject.SetActive(true);

        }

        public void HideLoadingPanel()
        {
            loadingPanel.gameObject.SetActive(false);
        }

        private string GetLoadingText(LoadingType type)
        {
            switch (type)
            {
                case LoadingType.Checking:
                    return "Checking...";
                case LoadingType.Loading:
                    return "Loading...";
                case LoadingType.Sending:
                    return "Sending...";
                default:
                    return "Loading...";
            }
        }

       
    }
     public enum LoadingType
        {
            Checking,
            Loading,
            Sending,
        }
}