using System;
using System.IO;
using UnityEngine;

public class ConsoleLogger : MonoBehaviour
{
    private string logFilePath;

    private void Awake()
    {
        // Set the file path for the log
        logFilePath = Path.Combine(Application.persistentDataPath, "ConsoleLog.txt");

        // Clear any existing log file
        File.WriteAllText(logFilePath, string.Empty);

        // Subscribe to the log message received event
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the log message received event
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Format the log message
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{type}] {logString}\n";

        // Append the log message to the file
        File.AppendAllText(logFilePath, logEntry);

        // If there is a stack trace, log it for errors or exceptions
        if (!string.IsNullOrEmpty(stackTrace) && (type == LogType.Error || type == LogType.Exception))
        {
            File.AppendAllText(logFilePath, $"{stackTrace}\n");
        }
    }
}
