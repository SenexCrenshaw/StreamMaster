using StreamMaster.Ring.API.Entities;

using System.Text.Json;

namespace RingTester
{
    internal class Program
    {
        private static readonly string OAuthTokenFilePath = "oauthToken.json";

        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            // Attempt to load OAuth token from file
            OAutToken? oAuthToken = LoadOAuthToken();
            StreamMaster.Ring.API.Session session = new("a", "b");

            if (oAuthToken != null)
            {
                Console.WriteLine("Loaded OAuth token from file.");
                session = await StreamMaster.Ring.API.Session.GetSessionByRefreshToken(oAuthToken.RefreshToken);
                //session.OAuthToken = oAuthToken;
            }
            else
            {
                // await session.Authenticate();

                await session.Authenticate(twoFactorAuthCode: "a");
                oAuthToken = session.OAuthToken;

                // Save the new OAuth token to a file
                SaveOAuthToken(oAuthToken);
            }

            // Use the OAuth token as needed

            Devices devices = await session.GetRingDevices();

            List<DoorbotHistoryEvent> doorbotHistory = await session.GetDoorbotsHistory();

            Console.WriteLine("Fetched Ring devices.");
        }

        private static OAutToken? LoadOAuthToken()
        {
            if (File.Exists(OAuthTokenFilePath))
            {
                string json = File.ReadAllText(OAuthTokenFilePath);
                return JsonSerializer.Deserialize<OAutToken>(json);
            }

            return null;
        }

        private static void SaveOAuthToken(OAutToken oAuthToken)
        {
            string json = JsonSerializer.Serialize(oAuthToken, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(OAuthTokenFilePath, json);
        }
    }
}