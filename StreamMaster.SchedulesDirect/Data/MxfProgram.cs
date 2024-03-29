﻿using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlIgnore] public List<MxfProgram> ProgramsToProcess { get; set; } = [];

    [XmlArrayItem("Program")]
    public ConcurrentDictionary<string, MxfProgram> Programs { get; set; } = new();

    public MxfProgram FindOrCreateProgram(string programId)
    {
        if (!Programs.ContainsKey(programId))
        {
            WriteToCSV(programsCSV, $"{Programs.Count + 1},{programId}");

        }
        (MxfProgram program, bool created) = Programs.FindOrCreateWithStatus(programId, key => new MxfProgram(Programs.Count + 1, programId));
        if (created)
        {
            return program;
        }

        ProgramsToProcess.Add(program);
        return program;
    }

    public void RemoveProgram(string programId)
    {
        Programs.TryRemove(programId, out _);
    }
}

