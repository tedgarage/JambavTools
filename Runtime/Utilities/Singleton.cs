using UnityEngine;

/// <summary>
/// base class of singleton 
/// </summary>

namespace Jambav.Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T sharedInstance { get; private set; }

        protected virtual void Awake()
        {
            if (sharedInstance == null)
            {
                sharedInstance = this as T;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}

