using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Commen methods for all projects
/// </summary>

namespace Jambav.Utilities
{
    public static class CommonMethods
    {

        public static List<int> GetShuffledIndeiesArray(int count = 0)
        {
            List<int> shuffleArray = new List<int>();
            for (int i = 0; i < count; i++)
            {
                shuffleArray.Add(i);
            }

            var count1 = shuffleArray.Count;
            var last = count - 1;
            for (var i = 1; i < last; ++i) //It was 0;
            {
                var r = UnityEngine.Random.Range(i, count1);
                var tmp = shuffleArray[i];
                shuffleArray[i] = shuffleArray[r];
                shuffleArray[r] = tmp;

            }
            return shuffleArray;
        }
        public static List<T> ShuffleList<T>(List<T> list)
        {
            var count1 = list.Count;
            var last = list.Count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count1);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;

            }
            return list;
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

        public static Color HexToColor(string hex, float alpha = 1)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            Color color = new Color(r / 255f, g / 255f, b / 255f);
            color.a = alpha;

            return color;
        }
    }
}

