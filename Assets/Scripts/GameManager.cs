using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            
            DatabaseManager.Instance.Initialized += InstanceOnInitialized;
        }

        private void InstanceOnInitialized()
        {
            
        }
    }
}