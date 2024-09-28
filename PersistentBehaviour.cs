using System;
using UnityEngine;

namespace UnityExtension
{
    public abstract class PersistentBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
