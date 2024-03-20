using System;
using System.IO;
using System.Diagnostics;

namespace NonoSharp;

public class CrashHandler
{
    public static void Initialize() =>
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(handle);

    private static void handle(object sender, UnhandledExceptionEventArgs ueea)
    {
        if (ueea.IsTerminating)
        {
            string crashFile = $"{AppDomain.CurrentDomain.BaseDirectory}/CrashLog.txt";
            Exception exception = (Exception)ueea.ExceptionObject;
            using (StreamWriter writer = new(crashFile))
            {
                DateTime dateTime = DateTime.Now;
                writer.WriteLine($"--- nonoSharp Crash Log: {dateTime}");
                writer.WriteLine("nonoSharp has crashed. Here's some information about the crash:\n");
                writer.WriteLine(exception.ToString());
            }

            // This might be unsafe...
            // At least on linux
            // Because you can set .../crashllog.txt to be executable and make it have whatever code
            // (which would have to happen at quite a specific time and is extremely unlikely)
            Process proc = new()
            {
                StartInfo = new()
                {
                    UseShellExecute = true,
                    FileName = crashFile
                }
            };

            proc.Start();
            proc.WaitForExit();
        }
    }
}
