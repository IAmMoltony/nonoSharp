using System;
using System.IO;
using System.Diagnostics;

namespace NonoSharp;

public class CrashHandler
{
    public static void Initialize()
    {
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(_handle);
    }

    private static void _handle(object sender, UnhandledExceptionEventArgs ueea)
    {
        if (ueea.IsTerminating)
        {
            string crashFile = AppDomain.CurrentDomain.BaseDirectory + "/CrashLog.txt";
            Exception exception = (Exception)ueea.ExceptionObject;
            using (StreamWriter writer = new(crashFile))
            {
                DateTime dateTime = new();
                writer.WriteLine($"--- nonoSharp Crash Log: {dateTime.ToString()}");
                writer.WriteLine("nonoSharp has crashed. Here's some information about the crash:\n");
                writer.WriteLine(exception.ToString());
            }

            Process proc = new();
            proc.StartInfo = new()
            {
                UseShellExecute = true,
                FileName = crashFile
            };

            proc.Start();
            proc.WaitForExit();
        }
    }
}
