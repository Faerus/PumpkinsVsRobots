using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public abstract class InstanceMonoBehaviour<T> : MonoBehaviour 
    where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                string gameObjectName = typeof(T).Name;
                gameObjectName = Regex.Replace(gameObjectName, "[A-Z]", " $0").Trim();
                _instance = GameObject.Find(gameObjectName).GetComponent<T>();
            }

            return _instance;
        }
    }
}
