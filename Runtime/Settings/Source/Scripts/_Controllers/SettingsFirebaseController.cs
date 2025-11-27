using UnityEngine;
using Jambav.FirebaseWrapper;
using System;
using Firebase.Auth;
using Jambav.Utilities;

namespace Jambav.Settings
{
    public class SettingsFirebaseController : Singleton<SettingsFirebaseController>
{
    // firebase Consts
    private const string dataPath_Admin = "/ADMIN_DATA/KEYS/";
    private const string expiredPath_Admin = "/EXPIRY_DATE";

    bool _isInit = false;


    public void FirebaseLogin(Action<bool> callback)
    {
        
        if (_isInit == true)
        {
            callback.Invoke(true);
            return;
        }
        FirebaseAuthWrapper.SignInAnonymously((FirebaseUser user) =>
        {
            _isInit = true;
            callback.Invoke(true);
        },
        (Exception ex) =>
        {
            _isInit = true;
            callback.Invoke(false);
        });
    }

    public void GetDateTimeFromEventCode(string _eventCode, Action<DateTime> callback, Action fallback)
    {

        FirebaseDatabaseWrapper.GetValue<string>(dataPath_Admin + _eventCode +expiredPath_Admin, (value) =>
        {
            // print("Path " + (dataPath_Admin + _eventCode + expiredPath_Admin));
            if (!string.IsNullOrEmpty(value))
            {
                // print(value);
                // var date = DateTime.Parse(value);
                // print(date);
                    
                if (TryParseDate(value, out DateTime expireDate))
                {
                    callback.Invoke(expireDate);
                }
                else
                {
                    fallback.Invoke();
                }
            }
            else
            {
                fallback.Invoke();
            }

        });
    }


    private bool TryParseDate(string input, out DateTime result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Try standard/ISO parsing first
        if (DateTime.TryParse(input, out DateTime dt))
        {
            result = dt;
            return true;
        }

        // Try numeric epoch parsing (seconds or milliseconds)
        if (long.TryParse(input, out long numeric))
        {
            try
            {
                // If the number looks like milliseconds (>= 13 digits), use milliseconds
                if (input.Length >= 13)
                {
                    result = DateTimeOffset.FromUnixTimeMilliseconds(numeric).LocalDateTime;
                }
                else
                {
                    result = DateTimeOffset.FromUnixTimeSeconds(numeric).LocalDateTime;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        return false;
    }
}
}

