using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
namespace Jambav.Extensions
{
    public static class Extensions
    {
        #region Color Extension Methods
        public static Color WithAlpha(this Color c, float alpha) => new Color(c.r, c.g, c.b, alpha);
        #endregion
        #region String Extension Methods
        public static Color Hex(this string hex, float alpha = 1)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            Color color = new Color(r / 255f, g / 255f, b / 255f);
            color.a = alpha;

            return color;
        }
        #endregion
        #region List Extension Methods
        public static List<T> Shuffle<T>(this List<T> list)
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
        
        #endregion

    }
}