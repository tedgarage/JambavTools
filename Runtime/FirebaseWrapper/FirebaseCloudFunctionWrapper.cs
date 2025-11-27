using System;
using UnityEngine;

#if FIREBASE_FUNCTIONS_AVAILABLE
using Firebase.Extensions;
using Firebase.Functions;

    

namespace Jambav.FirebaseWrapper
{
public class FirebaseCloudFunctionWrapper
{
    public static void FirebaseCallable(string functionName, Action<string> __callback, Action<string> __fallBack = null)
    {
        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;
        HttpsCallableReference reference = functions.GetHttpsCallable(functionName);
        try
        {
            reference.CallAsync().ContinueWith((response) =>
            {
                __callback?.Invoke(JsonUtility.ToJson(response.Result.Data));
            });
        }
        catch (Exception err)
        {
            __fallBack?.Invoke(err.Message);
        }
    }

    public static void FirebaseHttpsCallable(string functionName, string data, Action<string> __callback, Action<string> __fallBack = null)
    {
        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;
        HttpsCallableReference reference = functions.GetHttpsCallable(functionName);
        try
        {
            reference.CallAsync(data).ContinueWithOnMainThread(response =>
            {
                __callback?.Invoke(response.Result.Data.ToString());
            });
        }
        catch (Exception err)
        {
            __fallBack?.Invoke(err.ToString());
        }
    }

    public static void FirebaseHttpsCallableWithOutMainThread(string functionName, string data, Action<string> __callback, Action<string> __fallBack = null)
    {
        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;
        HttpsCallableReference reference = functions.GetHttpsCallable(functionName);
        try
        {
            reference.CallAsync(data).ContinueWith((response) =>
            {
                __callback?.Invoke(response.Result.Data.ToString());
            });
        }
        catch (Exception err)
        {
            __fallBack?.Invoke(err.ToString());
        }
    }

    public static void FirebaseApiCall(string __url, Action<string> __callback, Action<Exception> __fallBack = null)
    {
        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;
        HttpsCallableReference reference = functions.GetHttpsCallableFromURL(new Uri(__url));
        try
        {
            reference.CallAsync().ContinueWith((response) =>
            {
                __callback?.Invoke(response.Result.Data.ToString());
            });
        }
        catch (Exception err)
        {
            __fallBack?.Invoke(err);
        }
    }

}
}
#endif
