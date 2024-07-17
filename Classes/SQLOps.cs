using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Runtime.InteropServices;

public class SQLOps
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern int SHGetKnownFolderPath(ref Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

    public static string GetDownloadFolderPath()
    {
       
        Guid downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B"); // Downloads folder GUID
        IntPtr outPath;
        if (SHGetKnownFolderPath(ref downloads, 0, IntPtr.Zero, out outPath) == 0)
        {
            string path = Marshal.PtrToStringUni(outPath);
            Marshal.FreeCoTaskMem(outPath);
            return path;
        }
        return String.Empty; // or throw an exception, depending on your use case
    }

    public void InstallSqlServerIfNotPresent()
    {
        if (!IsSqlServerInstalled())
        {
            var path = GetDownloadFolderPath();
            var sqlServerInstallerPath = path;
            var cumulativeUpdateInstallerPath = path;

            WebClient webClient = new WebClient();

            Console.WriteLine("Downloading SQL Server 2014...");
            webClient.DownloadFile("https://URL_TO_SQL_SERVER_2014_INSTALLER", sqlServerInstallerPath);
            Console.WriteLine("Downloading Cumulative Update 4...");
            webClient.DownloadFile("https://URL_TO_CU4_INSTALLER", cumulativeUpdateInstallerPath);

            Console.WriteLine("Installing SQL Server 2014...");
            Process.Start(sqlServerInstallerPath, "/quiet /install");  // Assuming `/quiet /install` are the silent install arguments
            Console.WriteLine("Installing Cumulative Update 4...");
            Process.Start(cumulativeUpdateInstallerPath, "/quiet /install"); // Again, assuming the silent arguments
        }
        else
        {
            Console.WriteLine("SQL Server is already installed.");
        }
    }

    private bool IsSqlServerInstalled()
    {
        try
        {
            Server server = new Server();
            return server.Information.Version != null;
        }
        catch (ConnectionFailureException)
        {
            return false;
        }
    }
}