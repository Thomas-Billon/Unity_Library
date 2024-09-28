using System;
using UnityEngine;

namespace UnityExtension
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _instanceLock = new object();
        private static bool _isApplicationQuit = false;

        public static T Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null && _isApplicationQuit == false)
                    {
                        _instance = FindFirstObjectByType<T>();
                        if (_instance == null)
                        {
                            GameObject gameObject = new GameObject(typeof(T).ToString(), typeof(T));
                            _instance = gameObject.GetComponent<T>();
                            DontDestroyOnLoad(gameObject);
                        }
                    }
                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = gameObject.GetComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance.GetInstanceID() != GetInstanceID())
            {
                Destroy(gameObject);
                throw new Exception($"Duplicate singleton: Instance of {typeof(T)} already exists, removing {name}");
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _isApplicationQuit = true;
        }
    }
}
