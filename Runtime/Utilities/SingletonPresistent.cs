using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class of singleton Presistent
/// This class, don't destroy on load
/// </summary>

namespace Jambav.Utilities
{

    public class SingletonPresistent<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T sharedInstance { get; private set; }
        protected virtual void Awake()
        {
            if (sharedInstance == null)
            {
                sharedInstance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

    }
}