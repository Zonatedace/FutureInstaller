using System;
using System.Diagnostics;

public class WindowsDefenderExceptionManager
{
    public void AddException(string path)
    {
        string command = $@"powershell -Command ""Add-MpPreference -ExclusionPath '{path}'""";
        ExecuteCommand(command);
    }



    private void ExecuteCommand(string command)
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c " + command);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            using (Process process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
                string result = process.StandardOutput.ReadToEnd();
                Console.WriteLine(result);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}

