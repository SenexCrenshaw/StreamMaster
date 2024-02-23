using Microsoft.Extensions.Logging;

namespace StreamMaster.Domain.Helpers;

public class DirectoryHelper
{
    /// <summary>
    /// Deletes all files within the specified directory.
    /// </summary>
    /// <param name="directoryPath">The path to the directory whose files should be deleted.</param>
    /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static void DeleteAllFiles(string directoryPath, ILogger logger)
    {
        try
        {
            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' was not found.");
            }

            Directory.Delete(directoryPath, true);

            //// Get all files in the directory
            //string[] files = Directory.GetFiles(directoryPath);
            //foreach (string file in files)
            //{

            //    try
            //    {
            //        // Delete the file
            //        File.Delete(file);
            //    }
            //    catch (Exception ex)
            //    {
            //        // Log the error (assuming a logger is available)
            //        logger.LogError(ex, "Failed to delete file {FileName}.", file);
            //        //Console.WriteLine($"Failed to delete file {file}. Exception: {ex.Message}");
            //        // Consider whether to rethrow, handle the exception, or continue based on your use case
            //    }
            //}
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
}
