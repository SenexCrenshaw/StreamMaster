using System.Diagnostics;
using System.Reflection;

using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Helpers;

/// <summary>
/// Helper class for directory management, including creation, renaming, and deletion of directories.
/// </summary>
public static class DirectoryHelper
{
    private static bool setupDirectories = false;

    /// <summary>
    /// Logs messages to both console and debug output.
    /// </summary>
    /// <param name="format">The message format string.</param>
    /// <param name="args">The arguments to format.</param>
    private static void Log(string format, params object[] args)
    {
        string message = string.Format(format, args);
        Console.WriteLine(message);
        Debug.WriteLine(message);
    }

    /// <summary>
    /// Renames a directory from the old name to the new name.
    /// </summary>
    /// <param name="oldName">The current name of the directory.</param>
    /// <param name="newName">The new name for the directory.</param>
    public static void RenameDirectory(string oldName, string newName)
    {
        try
        {
            if (Directory.Exists(oldName))
            {
                Directory.Move(oldName, newName);
            }
        }
        catch (Exception ex)
        {
            Log($"Failed to rename directory: {oldName} to {newName}. Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates necessary application directories based on configuration, and optionally forces recreation.
    /// </summary>
    /// <param name="alwaysRun">Indicates whether the directory setup should run even if it has been run previously.</param>
    public static void CreateApplicationDirectories(bool alwaysRun = false)
    {
        if (setupDirectories && !alwaysRun)
        {
            return;
        }
        setupDirectories = true;

        Log($"Using settings file {BuildInfo.SettingsFile}");

        Type targetType = typeof(BuildInfo);

        IEnumerable<string?> fieldPaths = targetType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(f => (f.IsDefined(typeof(CreateDirAttribute), false) || f.Name.EndsWith("Folder")) && f.FieldType == typeof(string))
                        .Select(f => (string?)f.GetValue(null));

        IEnumerable<string?> propertyPaths = targetType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(p => (p.IsDefined(typeof(CreateDirAttribute), false) || p.Name.EndsWith("Folder")) && p.PropertyType == typeof(string))
                            .Select(p => (string?)p.GetValue(null));

        IEnumerable<string?> paths = fieldPaths.Concat(propertyPaths);
        Log("Creating Directories");

        foreach (string? newPath in paths)
        {
            if (!string.IsNullOrEmpty(newPath) && !Directory.Exists(newPath))
            {
                try
                {
                    Directory.CreateDirectory(newPath);
                }
                catch (Exception ex)
                {
                    Log($"Failed to create directory: {newPath}. Exception: {ex.Message}");
                }
            }
        }
        //CreateSubDirs(BuildInfo.SDImagesFolder);
        //CreateSubDirs(BuildInfo.SDStationLogosFolder);
        CreateSubDirs(BuildInfo.LogoFolder);
        CreateSubDirs(BuildInfo.ProgramLogoFolder);
        CreateSubDirs(BuildInfo.CustomLogoFolder);
    }

    /// <summary>
    /// Creates subdirectories with names 0-9 and a-f in the given folder.
    /// </summary>
    /// <param name="folder">The folder where subdirectories will be created.</param>
    private static void CreateSubDirs(string folder)
    {
        for (char c = '0'; c <= '9'; c++)
        {
            string subdirectoryPath = Path.Combine(folder, c.ToString());
            if (!Directory.Exists(subdirectoryPath))
            {
                Directory.CreateDirectory(subdirectoryPath);
            }
        }

        for (char c = 'a'; c <= 'f'; c++)
        {
            string subdirectoryPath = Path.Combine(folder, c.ToString());
            if (!Directory.Exists(subdirectoryPath))
            {
                Directory.CreateDirectory(subdirectoryPath);
            }
        }
    }

    /// <summary>
    /// Deletes a directory and its contents.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to delete.</param>
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
            Log($"An error occurred while deleting directory: {directoryPath}. Exception: {ex.Message}");
        }
    }

    /// <summary>
    /// Empties the contents of a directory, deleting all files and subdirectories.
    /// </summary>
    /// <param name="directoryPath">The path of the directory to empty.</param>
    public static void EmptyDirectory(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Log($"Attempted to empty a non-existing directory: {directoryPath}");
                return;
            }

            DirectoryInfo directoryInfo = new(directoryPath);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                    Log($"File deleted: {file.FullName}");
                }
                catch (Exception ex)
                {
                    Log($"Failed to delete file: {file.FullName}. Exception: {ex.Message}");
                }
            }

            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                    Log($"Directory deleted: {dir.FullName}");
                }
                catch (Exception ex)
                {
                    Log($"Failed to delete directory: {dir.FullName}. Exception: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Log($"Operation cancelled while emptying directory: {directoryPath}");
            throw;
        }
        catch (Exception ex)
        {
            Log($"An unexpected error occurred while emptying directory: {directoryPath}. Exception: {ex.Message}");
        }
    }
}
