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