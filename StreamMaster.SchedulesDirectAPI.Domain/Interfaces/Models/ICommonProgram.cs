namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

public interface ICommonProgram
{
    // Common identifier for the program
    string Identifier { get; }

    // Title or name of the program
    string Title { get; }

    // Description of the program
    string Description { get; }

    // Genres or categories associated with the program
    IEnumerable<string> Genres { get; }

    // Cast or credits of the program
    IEnumerable<string> CastAndCrew { get; }

    // Timing information (start/stop or air dates)
    (DateTime? Start, DateTime? End) Timing { get; }
}