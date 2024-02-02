﻿using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;
using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMaster.Domain.Services
{
    public interface IIconService
    {
        void AddIcon(string artworkUri, string title);
        ImagePath? GetValidImagePath(string URL);
        List<TvLogoFile> GetTvLogos();
        void AddIcon(IconFileDto iconFile);
        void AddIcons(List<IconFileDto> newIconFiles);
        void ClearIcons();
        void ClearTvLogos();
        IconFileDto? GetIconBySource(string source);
        List<IconFileDto> GetIcons(SMFileTypes? SMFileType = null);
        Task<bool> ReadDirectoryTVLogos(CancellationToken cancellationToken = default);
        void RemoveIconsByM3UFileId(int id);
        //void SetIndexes();
    }
}