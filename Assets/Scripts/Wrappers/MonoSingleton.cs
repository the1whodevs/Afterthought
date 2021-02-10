using UnityEngine;

/// <summary>
/// Base class for MonoBehaviour singletons. Automatically checks and destroys duplicates!
/// </summary>
/// <typeparam name="T">The class that should be the singleton, i.e. GameManager etc.</typeparam>
/// <typeparam name="R">The way you want the singleton to handle the missing instance case, i.e
/// when no instance of the <typeparamref name="T"/> class can be found in the scene. 
/// Options are:\n
/// (1)ReportMissingInstanceCreate: Create a new gameobject, add the <typeparamref name="T"/>
/// class as a component and use that as the instance.\n
/// (2)ReportMissingInstanceLog: Debug.Log a message reporting that you have a missing <typeparamref name="T"/> instance.\n
/// (3)ReportMissingInstanceWarning: Debug.LogWarning a message reporting that you have a missing <typeparamref name="T"/> instance.\n
/// (4)ReportMissingInstanceError: Debug.LogError a message reporting that you have a missing <typeparamref name="T"/> instance.\n
/// (5)ReportMissingInstanceException: throw a System.Exception with a message reporting that you have a missing <typeparamref name="T"/> instance.</typeparam>
public class MonoSingleton<T, R> : MonoBehaviour where T: MonoBehaviour where R: ReportMissingInstance
{
    public static T Active
    {
        get
        {
            if (_instance) return _instance;

            _instance = FindObjectOfType<T>();

            if (_instance) return _instance;

            switch (reportType.ReportType())
            {
                case ReportMissingInstanceType.Error:
                    Debug.LogError($"Missing {nameof(T)} instance!");
                    break;

                case ReportMissingInstanceType.Log:
                    Debug.Log($"Missing {nameof(T)} instance!");
                    break;

                case ReportMissingInstanceType.Warning:
                    Debug.LogWarning($"Missing {nameof(T)} instance!");
                    break;

                case ReportMissingInstanceType.Create:
                    _instance = new GameObject(nameof(T)).AddComponent<T>();
                    return _instance;

                case ReportMissingInstanceType.Exception:
                    throw new System.Exception($"Missing {nameof(T)} instance");

                default:
                    break;
            }

            return null;
        }
    }

    private static T _instance;

    private static readonly R reportType;

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Debug.LogError($"Multiple {nameof(T)} instances in scene! Destroying...");
            Destroy(this);
        }
    }
}

public enum ReportMissingInstanceType { Log, Warning, Error, Exception, Create }

public abstract class ReportMissingInstance
{
    public abstract ReportMissingInstanceType ReportType();
}

public class ReportMissingInstanceCreate : ReportMissingInstance
{
    public override ReportMissingInstanceType ReportType()
    {
        return ReportMissingInstanceType.Create;
    }
}

public class ReportMissingInstanceLog : ReportMissingInstance
{
    public override ReportMissingInstanceType ReportType()
    {
        return ReportMissingInstanceType.Log;
    }
}

public class ReportMissingInstanceWarning : ReportMissingInstance
{
    public override ReportMissingInstanceType ReportType()
    {
        return ReportMissingInstanceType.Warning;
    }
}

public class ReportMissingInstanceError : ReportMissingInstance
{
    public override ReportMissingInstanceType ReportType()
    {
        return ReportMissingInstanceType.Error;
    }
}

public class ReportMissingInstanceException : ReportMissingInstance
{
    public override ReportMissingInstanceType ReportType()
    {
        return ReportMissingInstanceType.Exception;
    }
}