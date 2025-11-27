using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#else
#if FIREBASE_DATABASE_AVAILABLE
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
#endif
#endif

namespace Jambav.FirebaseWrapper
{
    public class FirebaseDatabaseWrapper
    {
#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    public static extern void GetJSON(string path, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void GetDataSnapshot(string path, Action<string> callback, Action emptyFallback);

    [DllImport("__Internal")]
    public static extern void PostJSON(string path, string value, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void PushJSON(string path, string value, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void UpdateJSON(string path, string value, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void DeleteJSON(string path, Action<string> callback);

#elif FIREBASE_DATABASE_AVAILABLE

        private static DatabaseReference _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        private static Dictionary<string, ListenOnValueChangeData> ListenOnValueChangeDict = new Dictionary<string, ListenOnValueChangeData>();
        private static Dictionary<string, ListenOnChildChangeData> ListenOnChildAddedDict = new Dictionary<string, ListenOnChildChangeData>();
        private static Dictionary<string, ListenOnValueChangeWithContraintsData> ListenOnValueChangeWithContraintsDict = new Dictionary<string, ListenOnValueChangeWithContraintsData>();

        public static void GetJSON(string path, Action<string> callback)
        {
            try
            {
                _databaseRef.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        callback.Invoke("Network Issue");
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        if (snapshot == null)
                        {
                            callback.Invoke(null);
                            return;
                        }
                        if (snapshot.ChildrenCount == 0)
                        {
                            callback.Invoke("");
                            return;
                        }
                        callback.Invoke(snapshot.GetRawJsonValue());
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        public static void GetValue<T>(string path, Action<T> callback)
        {
            // var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();



            _databaseRef.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    callback.Invoke((T)(object)null);
                    if (typeof(T) == typeof(string))
                        callback?.Invoke((T)(object)"");
                    if (typeof(T) == typeof(int))
                        callback?.Invoke((T)(object)-1);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string dataValue = snapshot.Value.ToString();
                        Debug.Log("Data retrieved: " + dataValue);
                        if (snapshot == null)
                        {
                            callback.Invoke((T)(object)null);
                            return;
                        }

                        callback.Invoke((T)snapshot.GetValue(false));
                    }
                    else
                    {
                         callback.Invoke((T)(object)null);
                        if (typeof(T) == typeof(string))
                            callback?.Invoke((T)(object)"");
                        if (typeof(T) == typeof(int))
                            callback?.Invoke((T)(object)-1);
                        Debug.Log("No data found at specified path.");
                    }
                }
            });
            // _databaseRef.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
            // {
            //     if (task.IsFaulted)
            //     {
            //         callback.Invoke((T)(object)null);
            //         if (typeof(T) == typeof(string))
            //             callback?.Invoke((T)(object)"");
            //         if (typeof(T) == typeof(int))
            //             callback?.Invoke((T)(object)-1);
            //     }
            //     else if (task.IsCompleted)
            //     {
            //         DataSnapshot snapshot = task.Result;

            //         if (snapshot == null)
            //         {
            //             callback.Invoke((T)(object)null);
            //             return;
            //         }

            //         callback.Invoke((T)snapshot.GetValue(false));
            //     }
            // }, taskScheduler);
        }

        public static void GetDataSnapshot(string path, Action<string> callback, Action emptyFallback = null)
        {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _databaseRef.Child(path).GetValueAsync().ContinueWith((Task<DataSnapshot> task) =>
            {
                if (task.IsFaulted)
                {
                    callback.Invoke("Network Issue");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot == null)
                    {
                        callback.Invoke("No Data Found");
                        return;
                    }

                    if (snapshot.ChildrenCount == 0)
                    {
                        emptyFallback.Invoke();
                        return;
                    }

                    IEnumerable<DataSnapshot> games = snapshot.Children;

                    foreach (DataSnapshot dataSnapshot in games)
                    {
                        callback.Invoke(dataSnapshot.GetRawJsonValue());
                    }
                }
            }, taskScheduler);
        }

        public static void GetDataSnapshot(string path, Action<DataSnapshot> callback, Action emptyFallback = null)
        {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _databaseRef.Child(path).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    callback.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot == null)
                    {
                        callback.Invoke(null);
                        return;
                    }

                    if (snapshot.ChildrenCount == 0)
                    {
                        callback.Invoke(null);
                        return;
                    }

                    callback.Invoke(snapshot);
                }
            }, taskScheduler);
        }

        public static void GetDataSnapshotWithConstraints(string path, string orderBy, int limit, Action<DataSnapshot> callback)
        {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _databaseRef.Child(path).OrderByChild(orderBy).LimitToLast(limit).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    callback.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot == null)
                    {
                        callback.Invoke(null);
                        return;
                    }

                    if (snapshot.ChildrenCount == 0)
                    {
                        callback.Invoke(snapshot);
                        return;
                    }

                    callback.Invoke(snapshot);
                }
            }, taskScheduler);
        }


        public static void PostJSON(string path, string json, Action<string> callback)
        {
            FirebaseDatabase.DefaultInstance.GetReference(path).SetRawJsonValueAsync(json).ContinueWithOnMainThread((Task task) =>
            {
                if (task.IsFaulted)
                {
                    callback?.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    string successString = "Success: " + json + " was posted to " + path;
                    callback?.Invoke(successString);
                }
            });
        }

        public static void PostValue<T>(string path, T value, Action<string> callback)
        {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            FirebaseDatabase.DefaultInstance.GetReference(path).SetValueAsync(value).ContinueWith((Task task) =>
            {
                if (task.IsFaulted)
                {
                    callback?.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    string successString = "Success: " + value + " was posted to " + path;
                    callback?.Invoke(successString);
                }
            }, taskScheduler);
        }

        public static void UpdateJSON(string path, Dictionary<string, object> value, Action<string> callback)
        {
            FirebaseDatabase.DefaultInstance.GetReference(path).UpdateChildrenAsync(value).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    callback?.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    string successString = "Success: " + value + " was UPDATED to " + path;
                    callback?.Invoke(successString);
                }
            });
        }

        public static void DeleteJSON(string path, Action<string> callback)
        {
            _databaseRef.Child(path).RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error : " + task.ToString());
                    callback.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    callback.Invoke("Successfully Deleted!");
                }
            });
        }

        public static void PushJSON(string path, string json, Action<string> callback, Action<string> fallback)
        {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            DatabaseReference pushRef = FirebaseDatabase.DefaultInstance.GetReference(path).Push();
            pushRef.SetRawJsonValueAsync(json).ContinueWith((Task task) =>
            {
                if (task.IsFaulted)
                {
                    fallback?.Invoke("Something went wrong in push JSON");
                }
                else if (task.IsCompleted)
                {
                    callback?.Invoke(pushRef.Key);
                }
            }, taskScheduler);
        }


        public static void PerformTransaction(string __path, int __addingValue, Action<int> __callBack, Action<string> __fallBack)
        {
            try
            {
                DatabaseReference scoreRef = _databaseRef.Child(__path);

                scoreRef.RunTransaction(mutableData =>
                {
                    if (mutableData.Value != null && mutableData.Value is long currentValue)
                    {
                        mutableData.Value = currentValue + __addingValue;
                    }
                    else
                    {
                        mutableData.Value = __addingValue;
                    }

                    return TransactionResult.Success(mutableData);
                }).ContinueWithOnMainThread(task =>
                {
                    if (task.Exception != null)
                    {
                        __fallBack?.Invoke(task.Exception.Message);
                    }
                    else if (task.IsCompleted)
                    {
                        if (task.Result.Exists)
                        {
                            int updatedValue = (int)(long)task.Result.Value;
                            __callBack?.Invoke(updatedValue);
                        }
                        else
                        {
                            __fallBack?.Invoke("Data doesn't exist at the specified path.");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                __fallBack?.Invoke(ex.Message);
            }
        }


        #region LISTEN/UNLISTEN METHODS

        public static void AddListenerForValueChanged(string path, Action<object, ValueChangedEventArgs> callback)
        {
            if (ListenOnValueChangeDict.ContainsKey(path))
            {
                ListenOnValueChangeData data = ListenOnValueChangeDict[path];
                data.pathRef.ValueChanged -= data.callback.Invoke;
                ListenOnValueChangeDict.Remove(path);
                Debug.LogWarning("Lister already exit for same path");
            }

            DatabaseReference pathRef = FirebaseDatabase.DefaultInstance.GetReference(path);

            pathRef.ValueChanged += callback.Invoke;

            ListenOnValueChangeDict.Add(path, new ListenOnValueChangeData(pathRef, callback));
        }
        public static void RemoveListenerForValueChanged(string path)
        {
            if (!ListenOnValueChangeDict.ContainsKey(path))
            {
                Debug.LogWarning("Lister not available!");
                return;
            }
            ListenOnValueChangeData data = ListenOnValueChangeDict[path];
            data.pathRef.ValueChanged -= data.callback.Invoke;
            ListenOnValueChangeDict.Remove(path);
        }

        public static void AddListenerForValueChangedWithConstraints(string path, string orderBy, int limit, Action<object, ValueChangedEventArgs> callback)
        {
            if (ListenOnValueChangeWithContraintsDict.ContainsKey(path))
            {
                ListenOnValueChangeWithContraintsData data = ListenOnValueChangeWithContraintsDict[path];
                data.query.ValueChanged -= data.callback.Invoke;
                ListenOnValueChangeWithContraintsDict.Remove(path);
                Debug.LogWarning("Lister already exit for same path");
            }

            Query query = FirebaseDatabase.DefaultInstance.GetReference(path).OrderByChild(orderBy).LimitToLast(limit);

            query.ValueChanged += callback.Invoke;

            ListenOnValueChangeWithContraintsDict.Add(path, new ListenOnValueChangeWithContraintsData(query, callback));
        }
        public static void RemoveListenerForContraints(string path)
        {
            if (!ListenOnValueChangeWithContraintsDict.ContainsKey(path))
            {
                Debug.LogWarning("Lister not available!");
                return;
            }

            ListenOnValueChangeWithContraintsDict[path].query.ValueChanged -= ListenOnValueChangeWithContraintsDict[path].callback.Invoke;
        }

        public static void AddListenerForChildAdded(string path, Action<object, ChildChangedEventArgs> callback)
        {
            if (ListenOnChildAddedDict.ContainsKey(path))
            {
                ListenOnChildChangeData data = ListenOnChildAddedDict[path];
                data.pathRef.ChildAdded -= data.callback.Invoke;
                ListenOnChildAddedDict.Remove(path);
                Debug.LogWarning("Lister already exit for same path");
            }

            DatabaseReference pathRef = FirebaseDatabase.DefaultInstance.GetReference(path);

            pathRef.ChildAdded += callback.Invoke;

            ListenOnChildAddedDict.Add(path, new ListenOnChildChangeData(pathRef, callback));
        }

        public static void RemoveListenerForChildAdded(string path)
        {
            if (!ListenOnChildAddedDict.ContainsKey(path))
            {
                Debug.LogWarning("Lister not available!");
                return;
            }

            ListenOnChildChangeData data = ListenOnChildAddedDict[path];
            data.pathRef.ChildAdded -= data.callback.Invoke;
            ListenOnChildAddedDict.Remove(path);
        }

        #endregion
#endif
    }

#if !UNITY_WEBGL || UNITY_EDITOR
#if FIREBASE_DATABASE_AVAILABLE
    public struct ListenOnValueChangeData
    {
        public DatabaseReference pathRef;
        public Action<object, ValueChangedEventArgs> callback;

        public ListenOnValueChangeData(DatabaseReference __pathRef, Action<object, ValueChangedEventArgs> __callback)
        {
            pathRef = __pathRef;
            callback = __callback;
        }
    }
    public struct ListenOnChildChangeData
    {
        public DatabaseReference pathRef;
        public Action<object, ChildChangedEventArgs> callback;

        public ListenOnChildChangeData(DatabaseReference __pathRef, Action<object, ChildChangedEventArgs> __callback)
        {
            pathRef = __pathRef;
            callback = __callback;
        }
    }
    public struct ListenOnValueChangeWithContraintsData
    {
        public Query query;
        public Action<object, ValueChangedEventArgs> callback;

        public ListenOnValueChangeWithContraintsData(Query __pathRef, Action<object, ValueChangedEventArgs> __callback)
        {
            query = __pathRef;
            callback = __callback;
        }
    }
#endif
#endif
}