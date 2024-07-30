using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public class WindowsDefenderExceptionManager
{
    public async Task<string> ExecuteCommandAsync(string command)
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("powershell.exe", $"-Command \"{command}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
                string result = await process.StandardOutput.ReadToEndAsync();
                return result;
            }
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    public async Task<string> AddInstallFolderExceptionAsync(string path)
    {
        string command = $@"Add-MpPreference -ExclusionPath '{path}'";
        return await ExecuteCommandAsync(command);
    }

    public async Task<string> AddInstallerExceptionAsync(string processName)
    {
        string command = $@"Add-MpPreference -ExclusionProcess '{processName}.exe'";
        return await ExecuteCommandAsync(command);
    }

    public async Task<string> AddFirewallExceptionAsync(string processName)
    {
        string command = $@"Add-MpPreference -ExclusionProcess '{processName}.exe'";
        return await ExecuteCommandAsync(command);
    }

    public async Task<string> ChangeNetworkProfileAsync()
    {
        string command = $@"Set-NetConnectionProfile -NetworkCategory Private";
        return await ExecuteCommandAsync(command);
    }

    public async Task<List<(string command, string output)>> AddFirewallRulesAsync()
    {
        var firewallRules = new FirewallRule[]
        {
        new FirewallRule { Name = "SQL Port 1433", Protocol = "TCP", LocalPort = "1433" },
        new FirewallRule { Name = "SQL Browser UDP", Protocol = "UDP", LocalPort = "1434" },
        new FirewallRule { Name = "SQL Browser TCP", Protocol = "TCP", LocalPort = "1434" },
        new FirewallRule { Name = "BOW", Protocol = "TCP", LocalPort = "44356" },
        new FirewallRule { Name = "DBSetup", Protocol = "TCP", LocalPort = "49978" },
        new FirewallRule { Name = "Future POS X10", Protocol = "TCP", LocalPort = "7117" },
        new FirewallRule { Name = "Future POS Time", Protocol = "TCP", LocalPort = "7124" },
        new FirewallRule { Name = "Future POS Security", Protocol = "TCP", LocalPort = "7107" },
        new FirewallRule { Name = "Future POS Salenumber", Protocol = "TCP", LocalPort = "7106" },
        new FirewallRule { Name = "Future POS Restaurant Vision", Protocol = "TCP", LocalPort = "7123" },
        new FirewallRule { Name = "Future POS Printer Service 7127", Protocol = "TCP", LocalPort = "7127" },
        new FirewallRule { Name = "Future POS Printer Service 7128", Protocol = "TCP", LocalPort = "7128" },
        new FirewallRule { Name = "Future POS", Protocol = "TCP", LocalPort = "7104" },
        new FirewallRule { Name = "Future Ping", Protocol = "TCP", LocalPort = "7125" },
        new FirewallRule { Name = "Future Paydiant", Protocol = "TCP", LocalPort = "7115" },
        new FirewallRule { Name = "Future Net Install", Protocol = "TCP", LocalPort = "7108" },
        new FirewallRule { Name = "Future MobilePOS port2", Protocol = "UDP", LocalPort = "65001" },
        new FirewallRule { Name = "Future MobilePOS port1", Protocol = "UDP", LocalPort = "65000" },
        new FirewallRule { Name = "FuturePOS Loyalty", Protocol = "TCP", LocalPort = "7116" },
        new FirewallRule { Name = "FuturePOS HealthCheck Service 7130", Protocol = "TCP", LocalPort = "7130" },
        new FirewallRule { Name = "FuturePOS HealthCheck Service 7129", Protocol = "TCP", LocalPort = "7129" },
        new FirewallRule { Name = "FuturePOS Customer Number", Protocol = "TCP", LocalPort = "7118" },
        new FirewallRule { Name = "FuturePOS Loyalty 7114", Protocol = "TCP", LocalPort = "7114" },
        new FirewallRule { Name = "FuturePOS Scoreboard", Protocol = "TCP", LocalPort = "7113" },
        new FirewallRule { Name = "FuturePOS Credit Message", Protocol = "TCP", LocalPort = "7109" },
        new FirewallRule { Name = "FuturePOS Credit Message2", Protocol = "TCP", LocalPort = "7112" },
        new FirewallRule { Name = "FuturePOS Digital Signage", Protocol = "TCP", LocalPort = "7111" },
        new FirewallRule { Name = "FuturePOS Camera Control", Protocol = "TCP", LocalPort = "7110" },
        new FirewallRule { Name = "FuturePOS Enterprise Update", Protocol = "TCP", LocalPort = "7105" },
        new FirewallRule { Name = "FuturePOS TLOG", Protocol = "TCP", LocalPort = "7103" },
        new FirewallRule { Name = "FuturePOS PMS", Protocol = "TCP", LocalPort = "7102" },
        new FirewallRule { Name = "FuturePOS CallerID", Protocol = "TCP", LocalPort = "7101" },
        new FirewallRule { Name = "FuturePOS Camera", Protocol = "TCP", LocalPort = "7100" },
        new FirewallRule { Name = "SSL", Protocol = "TCP", LocalPort = "443" },
        new FirewallRule { Name = "SQL DEBUG/RPC", Protocol = "TCP", LocalPort = "135" },
        new FirewallRule { Name = "HTTP", Protocol = "TCP", LocalPort = "80" },
        new FirewallRule { Name = "Shift4 Command Center", Protocol = "TCP", LocalPort = "18028" },
        new FirewallRule { Name = "ICE1 6456", Protocol = "TCP", LocalPort = "6456" },
        new FirewallRule { Name = "ICE1 6457", Protocol = "TCP", LocalPort = "6457" },
        new FirewallRule { Name = "UTG2Svc.exe", Program = @"C:\Shift4\UTG2\UTG2Svc.exe" },
        new FirewallRule { Name = "FPOS.EXE", Program = @"C:\FPOS\Bin\fpos.exe" },
        new FirewallRule { Name = "FPOSVirtualHost", Program = @"C:\FPOS\Bin\FPOSVirtualHost.exe" },
        new FirewallRule { Name = "CallerIDDotCom", Program = @"C:\FPOS\Bin\CallerIDDotCom.exe" },
        new FirewallRule { Name = "SQLSERV.EXE", Program = @"C:\Program Files\Microsoft SQL Server\MSSQL12.CESSQL\MSSQL\Binn\sqlservr.exe" }
        };

        List<Task<(string command, string output)>> tasks = new List<Task<(string command, string output)>>();

        foreach (var rule in firewallRules)
        {
            tasks.Add(AddFirewallRuleAsync(rule));
            
        }

        var results = await Task.WhenAll(tasks);
        return new List<(string command, string output)>(results);
    }

    private async Task<(string command, string output)> AddFirewallRuleAsync(FirewallRule rule)
    {
        string checkCommand = $@"if (Get-NetFirewallRule -DisplayName '{rule.Name}' -ErrorAction SilentlyContinue) {{ $true }} else {{ $false }}";
        if (!await RuleExistsAsync(checkCommand))
        {
            string command;
            if (rule.Program != null)
            {
                command = $@"netsh advfirewall firewall add rule name='{rule.Name}' dir=in action=allow program='{rule.Program}'";
            }
            else
            {
                command = $@"netsh advfirewall firewall add rule name='{rule.Name}' dir=in action=allow protocol={rule.Protocol} localport={rule.LocalPort}";
            }
            string output = await ExecuteCommandAsync(command);
            return (command, output);
        }
        return (checkCommand, "Rule already exists");
    }


    private async Task<bool> RuleExistsAsync(string command)
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("powershell.exe", $"-Command \"{command}\"")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(processStartInfo))
            {
                string result = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();
                return result.Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return false;
        }
    }
}

public class FirewallRule
{
    public string Name { get; set; }
    public string Protocol { get; set; }
    public string LocalPort { get; set; }
    public string Program { get; set; }
}
