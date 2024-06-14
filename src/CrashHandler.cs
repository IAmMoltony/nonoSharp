using System;
using System.IO;
using System.Diagnostics;

namespace NonoSharp;

public static class CrashHandler
{
    public static void Initialize() =>
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(handle);

    private static void handle(object sender, UnhandledExceptionEventArgs ueea)
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

            NonoSharpGame.Close();

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
