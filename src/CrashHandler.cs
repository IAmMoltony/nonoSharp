using System;
using System.IO;

namespace NonoSharp;

public static class CrashHandler
{
    public static void Initialize(Action exit) =>
        AppDomain.CurrentDomain.UnhandledException += (sender, ueea) => handle(sender, ueea, exit);

    private static void handle(object sender, UnhandledExceptionEventArgs ueea, Action exit)
    {
        if (ueea.IsTerminating)
        {
            string crashFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashLog.txt");
            Exception exception = (Exception)ueea.ExceptionObject;
            using StreamWriter writer = new(crashFile);
            DateTime dateTime = DateTime.Now;
            writer.WriteLine($"--- nonoSharp Crash Log: {dateTime}");
            writer.WriteLine("nonoSharp has crashed. Here's some information about the crash:\n");
            writer.WriteLine(exception.ToString());

            exit();

#if !DEBUG // Not executed on debug builds because the error is visible in the console (and in VS if we're using it)
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
#endif
        }
    }
}
