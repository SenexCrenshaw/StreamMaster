using System.Diagnostics;

namespace StreamMaster.Domain.Helpers;

public static class ProcessHelper
{
    public static bool KillProcessByName(string processName)
    {
        try
        {
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while killing process '{processName}': {ex.Message}");
            return false;
        }
    }

    public static bool KillProcessById(int processId)
    {
        if (processId < 1024)
        {
            return false;
        }
        try
        {
            Process process = Process.GetProcessById(processId);
            process.Kill();
            return true;
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"Process with ID {processId} not found.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while killing process with ID {processId}: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> KillProcessByNameAsync(string processName)
    {
        try
        {
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                process.Kill();
                await Task.Delay(100); // Optional delay for cleanup
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while killing process '{processName}': {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> KillProcessByIdAsync(int processId)
    {
        try
        {
            Process process = Process.GetProcessById(processId);
            process.Kill();
            await Task.Delay(100); // Optional delay for cleanup
            return true;
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"Process with ID {processId} not found.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while killing process with ID {processId}: {ex.Message}");
            return false;
        }
    }
}
