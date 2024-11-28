using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public ConcurrentDictionary<string, MxfProgram> ProgramsToProcess { get; set; } = [];

    [XmlArrayItem("Program")]
    public ConcurrentDictionary<string, MxfProgram> Programs { get; set; } = new();

    public MxfProgram FindOrCreateProgram(string programId)
    {
        MxfProgram program = Programs.FindOrCreate(programId, _ => new MxfProgram(Programs.Count + 1, programId));

        ProgramsToProcess.TryAdd(programId, program);
        return program;
    }

    public void RemoveProgram(string programId)
    {
        Programs.TryRemove(programId, out _);
        ProgramsToProcess.TryRemove(programId, out _);
    }
}
