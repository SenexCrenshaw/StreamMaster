﻿using System.Collections.Specialized;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISeriesImages : IEPGCached, IDisposable
    {
        NameValueCollection SportsSeries { get; set; }

        Task<bool> GetAllSeriesImages();
    }
}