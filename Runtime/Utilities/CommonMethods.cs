using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Common methods for all projects
/// </summary>

namespace Jambav.Utilities
{
    public static class CommonMethods
    {
        #region  UNITY_RELATED_METHODS
        // Example Path: "ProductSprites/Zoho_{0}"
        public static T GetResource<T>(string _path, string _resourceName) where T : Object
        {
            return Resources.Load<T>(string.Format(_path, _resourceName));
        }
        public static Transform GetObjectForReuse(GameObject _objectPrefab, Transform _collectorObject, Transform _holderObject)
        {
            Transform returnObj;
            if (_collectorObject.childCount == 0)
            {
                returnObj = Object.Instantiate(_objectPrefab, _holderObject).transform;
            }
            else
            {
                returnObj = _collectorObject.GetChild(0);
                returnObj.SetParent(_holderObject);
                returnObj.gameObject.SetActive(true);
            }
            return returnObj;
        }
        public static void DespawnObject(Transform _object, Transform _collectorObject)
        {
            _object.SetParent(_collectorObject);
            _object.localPosition = Vector3.zero;
        }

        public static bool IsLandscapeActive()
        {
#if UNITY_IOS || UNITY_ANDROID
            return Screen.width > Screen.height;
#else
            return true;
#endif
        }

        public static void OpenUrl(string url)
        {
            url = System.Uri.EscapeUriString(url);
            Application.OpenURL(url);
        }

        #endregion

        #region  COMMON METHODS
        public static List<int> GetShuffledIndiesList(int count = 0)
        {
            List<int> shuffleArray = new List<int>();
            for (int i = 0; i < count; i++)
            {
                shuffleArray.Add(i);
            }

            var count1 = shuffleArray.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) //It was 0;
            {
                var r = UnityEngine.Random.Range(i, count1);
                var tmp = shuffleArray[i];
                shuffleArray[i] = shuffleArray[r];
                shuffleArray[r] = tmp;

            }
            return shuffleArray;
        }
    
        
        #endregion
    }

}

