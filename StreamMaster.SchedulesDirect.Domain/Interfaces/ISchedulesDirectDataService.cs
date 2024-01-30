using StreamMaster.SchedulesDirect.Domain.Models;

using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

/// <summary>
/// Represents a service for managing and accessing SchedulesDirect data.
/// Provides methods and properties to interact with television scheduling data.
/// </summary>
public interface ISchedulesDirectDataService
{
    /// <summary>
    /// Gets a list of station channel names.
    /// </summary>
    /// <returns>List of <see cref="StationChannelName"/>.</returns>
    List<StationChannelName> GetStationChannelNames();

    /// <summary>
    /// Gets a concurrent dictionary containing SchedulesDirect data, keyed by EPG ID.
    /// </summary>
    ConcurrentDictionary<int, ISchedulesDirectData> SchedulesDirectDatas { get; }

    /// <summary>
    /// Resets the data service for a specific EPG Number or all data if no ID is provided.
    /// </summary>
    /// <param name="EPGNumber">Optional EPG Number to reset data for. If null, resets all data.</param>
    void Reset(int? EPGNumber = null);

    /// <summary>
    /// Gets all keywords.
    /// </summary>
    List<MxfKeyword> AllKeywords { get; }

    /// <summary>
    /// Gets all lineups.
    /// </summary>
    List<MxfLineup> AllLineups { get; }

    /// <summary>
    /// Gets all series information.
    /// </summary>
    List<MxfSeriesInfo> AllSeriesInfos { get; }

    /// <summary>
    /// Gets all services.
    /// </summary>
    List<MxfService> AllServices { get; }

    /// <summary>
    /// Gets all programs.
    /// </summary>
    List<MxfProgram> AllPrograms { get; }

    /// <summary>
    /// Retrieves SchedulesDirect data for a specified EPG number.
    /// </summary>
    /// <param name="EPGNumber">The EPG number for which to retrieve data.</param>
    /// <returns>The <see cref="ISchedulesDirectData"/> associated with the specified EPG number.</returns>
    ISchedulesDirectData GetEPGData(int EPGNumber);

    /// <summary>
    /// Retrieves service information for a specified station ID.
    /// </summary>
    /// <param name="stationId">The station ID for which to retrieve service information.</param>
    /// <returns>The <see cref="MxfService"/> associated with the specified station ID, or null if not found.</returns>
    MxfService? GetService(string stationId);

    ISchedulesDirectData SchedulesDirectData();
    ISchedulesDirectData DummyData();
    void ChangeServiceEPGNumber(int oldEPGNumber, int newEPGNumber);
    List<MxfService> GetAllSDServices { get; }
    List<MxfProgram> GetAllSDPrograms { get; }
}
