using Microsoft.Extensions.Logging;

namespace StreamMaster.Domain.Helpers;

public class DirectoryHelper
{

    public static void DeleteDirectory(string directoryPath, ILogger logger)
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
            logger.LogError("Operation was canceled.");
        }
        catch (Exception ex)
        {
            // Log or handle the global error
            logger.LogError($"An error occurred: {ex.Message}");
            // Depending on the severity of the function, you might want to rethrow or handle the exception gracefully
        }
    }


    public static void EmptyDirectory(string directoryPath, ILogger logger)
    {
        try
        {
            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                logger.LogWarning("Attempted to empty a non-existing directory: {DirectoryPath}", directoryPath);
                return;
            }

            DirectoryInfo directoryInfo = new(directoryPath);

            // Delete all files
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                    logger.LogInformation("File deleted: {FilePath}", file.FullName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to delete file: {FilePath}", file.FullName);
                }
            }

            // Delete all directories
            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                try
                {
                    dir.Delete(true); // true to remove directories, subdirectories, and files
                    logger.LogInformation("Directory deleted: {DirectoryPath}", dir.FullName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to delete directory: {DirectoryPath}", dir.FullName);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation cancelled while emptying directory: {DirectoryPath}", directoryPath);
            throw; // Propagate the cancellation exception
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while emptying directory: {DirectoryPath}", directoryPath);
        }
    }
}
