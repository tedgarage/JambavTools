using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Jambav.Utilities
{
    public class InternetConnectivityChecker : MonoBehaviour
    {
        private const string DEFAULT_CONNECTIVITY_URL = "https://jambav.com/alive.html";
        private const float DEFAULT_TIMEOUT = 5f;

        private static InternetConnectivityChecker _instance;
        private bool _isConnected = false;
        private Coroutine _currentCheckCoroutine;

     
        public static InternetConnectivityChecker Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("InternetConnectivityChecker");
                    _instance = go.AddComponent<InternetConnectivityChecker>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }


        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #region Quick Check Methods

        public static bool HasInternetQuick()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public static NetworkReachability GetNetworkType()
        {
            return Application.internetReachability;
        }

        #endregion

     
    #region Static Convenience Methods
    public static void CheckInternetConnection(Action<bool> callback, string url = null, float timeout = DEFAULT_TIMEOUT)
    {
        if (Instance._currentCheckCoroutine != null)
        {
            Instance.StopCoroutine(Instance._currentCheckCoroutine);
        }
        
        string checkUrl = string.IsNullOrEmpty(url) ? DEFAULT_CONNECTIVITY_URL : url;
        Instance._currentCheckCoroutine = Instance.StartCoroutine(Instance.CheckConnectionCoroutine(callback, checkUrl, timeout));
    }


        #endregion

        #region Coroutines

        private IEnumerator CheckConnectionCoroutine(Action<bool> callback, string url, float timeout)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.timeout = (int)timeout;

                yield return webRequest.SendWebRequest();

                bool isSuccess = webRequest.result == UnityWebRequest.Result.Success;
                _isConnected = isSuccess;

                if (isSuccess)
                {
                    Debug.Log($"[InternetConnectivityChecker] Connection available - URL: {url}");
                }
                else
                {
                    Debug.LogWarning($"[InternetConnectivityChecker] No connection - Error: {webRequest.error}");
                }

                callback?.Invoke(isSuccess);
            }

            _currentCheckCoroutine = null;
        }

        #endregion

      
       
        private void OnDestroy()
        {
            if (_currentCheckCoroutine != null)
            {
                StopCoroutine(_currentCheckCoroutine);
            }
        }
    }
}

