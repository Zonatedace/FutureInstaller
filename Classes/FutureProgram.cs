using Microsoft.Win32;
using System;

public class FutureProgram
{
    public static string GetVersion()
    {
        string keyPath = @"SOFTWARE\WOW6432Node\Future P.O.S.";

        // Open the registry key
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
        {
            if (key != null)
            {
                // Read the value
                object versionValue = key.GetValue("Version");

                if (versionValue != null)
                {
                    // If the value exists, return it
                    return versionValue.ToString();
                }
                else
                {
                    // The value does not exist
                    return "Version Not Found";
                }
            }
            else
            {
                // The key does not exist
                return "Not Installed";
            }
        }
    }
}