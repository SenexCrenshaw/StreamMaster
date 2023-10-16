using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

public interface IClientStreamerManager2
{
    int GetClientCount(string StreamURL);
    IEnumerable<ClientStreamerConfiguration> GetAllClientStreamerConfigurations(string StreamURL);
    ClientStreamerConfiguration? GetClientStreamerConfiguration(string StreamURL, Guid clientId);
    void RegisterClientConfiguration(string StreamURL, ClientStreamerConfiguration config);
    bool UnregisterClientConfiguration(string StreamURL, Guid clientId);
}