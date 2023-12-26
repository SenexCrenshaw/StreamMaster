namespace StreamMaster.Domain.Events;

public class StreamFailedEventArgs(string errorMessage) : EventArgs
{
    public string ErrorMessage { get; } = errorMessage;
}

public class ClientRegisteredEventArgs(Guid clientId) : EventArgs
{
    public Guid ClientId { get; } = clientId;
}

public class ClientUnregisteredEventArgs(Guid clientId) : EventArgs
{
    public Guid ClientId { get; } = clientId;
}

public class StreamHandlerStoppedEventArgs() : EventArgs
{
}

