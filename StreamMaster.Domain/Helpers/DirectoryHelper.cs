using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Configuration;

using System.Diagnostics;
using System.Reflection;

namespace StreamMaster.Domain.Helpers;

public static class DirectoryHelper
{
    private static bool setupDirectories = false;
    private static void Log(string format, params object[] args)
    {
                string message = string.Format(format, args);
        Console.WriteLine(message);
        Debug.WriteLine(message);
    }

    public static void RenameDirectory(string oldName, string newName)
    {
        if (Directory.Exists(oldName))
        {
            Directory.Move(oldName, newName);
        }
    }

    public static void CreateApplicationDirectories(bool alwaysRun = false)
    {
        if (setupDirectories && !alwaysRun)
        {
            return;
        }
        setupDirectories = true;

        Log($"Using settings file {BuildInfo.SettingsFile}");

        Type targetType = typeof(BuildInfo);

        // Get fields marked with [CreateDir] or named "*Folder"
        IEnumerable<string?> fieldPaths = targetType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(f => (f.IsDefined(typeof(CreateDirAttribute), false) || f.Name.EndsWith("Folder")) && f.FieldType == typeof(string))
                        .Select(f => (string)f.GetValue(null));

        // Get properties marked with [CreateDir] or named "*Folder"
        IEnumerable<string?> propertyPaths = targetType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(p => (p.IsDefined(typeof(CreateDirAttribute), false) || p.Name.EndsWith("Folder")) && p.PropertyType == typeof(string))
                            .Select(p => (string)p.GetValue(null));

        // Combine paths from fields and properties
        IEnumerable<string?> paths = fieldPaths.Concat(propertyPaths);
        Log("Checking Directories:");
        foreach (string? newPath in paths)
        {
            string? path = newPath;

            Log($"Directory: {path}");
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);

                }
                catch (Exception ex)
                {
                    Log($"Failed to create directory: {path} {ex.InnerException}");
                    //throw;
                }
            }
        }

        for (char c = '0'; c <= '9'; c++)
        {
            string subdirectoryName = c.ToString();
            string subdirectoryPath = Path.Combine(BuildInfo.SDImagesFolder, subdirectoryName);

            Log($"Directory: {subdirectoryPath}");
            // Create the subdirectory if it doesn't exist
            if (!Directory.Exists(subdirectoryPath))
            {
                Directory.CreateDirectory(subdirectoryPath);
            }
        }

        for (char c = 'a'; c <= 'f'; c++)
        {
            string subdirectoryName = c.ToString();
            string subdirectoryPath = Path.Combine(BuildInfo.SDImagesFolder, subdirectoryName);
            Log($"Directory: {subdirectoryPath}");
            // Create the subdirectory if it doesn't exist
            if (!Directory.Exists(subdirectoryPath))
            {
                Directory.CreateDirectory(subdirectoryPath);
            }
        }
    }

    public static void DeleteDirectory(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            Directory.Delete(directoryPath, true);
        }
        catch (OperationCanceledException)
        {
            Log("Operation was canceled.");
        }
        catch (Exception ex)
        {
            Log($"An error occurred: {ex.Message}");

        }
    }

    public static void EmptyDirectory(string directoryPath)
    {
        try
        {
            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                Log("Attempted to empty a non-existing directory: {DirectoryPath}", directoryPath);
                return;
            }

            DirectoryInfo directoryInfo = new(directoryPath);

            // Delete all files
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                    Log("File deleted: {FilePath}", file.FullName);
                }
                catch (Exception ex)
                {
                    Log("Failed to delete file: {FilePath} {ex}", file.FullName, ex.InnerException);
                }
            }

            // Delete all directories
            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                try
                {
                    dir.Delete(true); // true to remove directories, subdirectories, and files
                    Log("Directory deleted: {DirectoryPath}", dir.FullName);
                }
                catch (Exception ex)
                {
                    Log("Failed to delete directory: {DirectoryPath} {ex}", dir.FullName, ex.InnerException);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Log("Operation cancelled while emptying directory: {DirectoryPath}", directoryPath);
            throw; // Propagate the cancellation exception
        }
        catch (Exception ex)
        {
            Log("An unexpected error occurred while emptying directory: {DirectoryPath} {ex}", directoryPath, ex.InnerException);
        }
    }
}
