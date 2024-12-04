using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IProgramRepository
    {
        ConcurrentDictionary<string, MxfProgram> Programs { get; }

        Task<MxfProgram> FindOrCreateProgram(string programId, string md5);
        MxfProgram? FindProgram(string programId);
        void RemoveProgram(string programId);
    }
}