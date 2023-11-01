using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;

using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI.Services;

public class SDTokenFileHandler : ISDTokenFileHandler
{
    private readonly string SD_TOKEN_FILENAME = Path.Combine(BuildInfo.AppDataFolder, "sd_token.json");
    private readonly ILogger logger;

    public SDTokenFileHandler(ILogger logger)
    {
        this.logger = logger;
    }

    public string? LoadToken()
    {
        try
        {
            if (!File.Exists(SD_TOKEN_FILENAME))
            {
                return null;
            }

            string jsonString = File.ReadAllText(SD_TOKEN_FILENAME);
            SDTokenFile? result = JsonSerializer.Deserialize<SDTokenFile>(jsonString);
            return result?.Token;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to load token from file. Error: {Error}", ex.Message);
            return null;
        }
    }

    public void SaveToken(string token)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(new SDTokenFile { Token = token, TokenDateTime = DateTime.Now });
            lock (typeof(SDTokenFileHandler))
            {
                File.WriteAllText(SD_TOKEN_FILENAME, jsonString);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to save token to file. Error: {Error}", ex.Message);
        }
    }
}
