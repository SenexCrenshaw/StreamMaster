using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Common.Interfaces;

public interface IClientStreamerManager
{
    int ClientCount { get; }

    IEnumerable<ClientStreamerConfiguration> GetAllClientConfigurations();
    ClientStreamerConfiguration? GetClientConfiguration(Guid clientId);
    void RegisterClientConfiguration(ClientStreamerConfiguration config);
    bool UnregisterClientConfiguration(Guid clientId);
}