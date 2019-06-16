using System;
using System.IO;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    private string path;

    #region Singleton
    public static Debugger instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        path = Application.persistentDataPath + "/anonima.log";
        Debug.Log(path);
    }
    #endregion

    public void WriteLog(string entry)
    {
        try
        {
            if (DataContainer.instance.isDebugging)
            {
                string timeStamp = DateTime.Now.ToString();
                string logEntry = timeStamp + "   -   " + entry;
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(logEntry);
                }
            }
            Debug.Log(entry);
        }
        catch (Exception e)
        {
            Debug.LogError("Debugger log exception: " + e.Source + e.Message);
        }
    }
}
