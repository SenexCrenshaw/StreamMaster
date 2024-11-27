using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public List<MxfProgram> ProgramsToProcess { get; set; } = [];

    [XmlArrayItem("Program")]
    public ConcurrentDictionary<string, MxfProgram> Programs { get; set; } = new();

    public MxfProgram FindOrCreateProgram(string programId)
    {
        (MxfProgram program, bool created) = Programs.FindOrCreateWithStatus(programId, _ => new MxfProgram(Programs.Count + 1, programId));

        if (created)
        {
            return program;
        }

        ProgramsToProcess.Add(program);
        return program;
    }

    public void RemoveProgram(string programId)
    {
        _ = Programs.TryRemove(programId, out _);
    }
}
