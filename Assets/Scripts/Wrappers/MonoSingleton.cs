using UnityEngine;

/// <summary>
/// Base class for MonoBehaviour singletons. Automatically checks and destroys duplicates!
/// </summary>
/// <typeparam name="T">The class that should be the singleton, i.e. GameManager etc.</typeparam>
public class MonoSingleton<T> : MonoBehaviour where T: MonoBehaviour
{
    public static T Active
    {
        get
        {
            if (_instance) return _instance;

            _instance = FindObjectOfType<T>();

            if (_instance) return _instance;

            return null;
        }
    }

    private static T _instance;
}