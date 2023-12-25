using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public List<MxfProgram> ProgramsToProcess { get; set; } = [];

    private Dictionary<string, MxfProgram> _programs = [];
    public MxfProgram FindOrCreateProgram(string programId, int? epgId = null)
    {
        if (_programs.TryGetValue(programId, out MxfProgram? program))
        {
            return program;
        }
        program = new MxfProgram(Programs.Count + 1, programId);
        if (epgId != null)
        {
            program.extras.Add("epgid", epgId);
        }
        Programs.Add(program);
        _programs.Add(programId, program);
        ProgramsToProcess.Add(program);
        return program;
    }

    public void RemoveProgram(string programId)
    {
        if (!_programs.TryGetValue(programId, out MxfProgram? program))
        {
            return;
        }
        _programs.Remove(programId);
        Programs.Remove(program);
    }
}

