using UnityEngine;

public class xpLogger : xpTURN.Common.ILog
{
    public void Debug(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void Info(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void Warn(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    public void Error(string message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public void Fatal(string message)
    {
        UnityEngine.Debug.LogAssertion(message);
    }
}
