using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Profiles.Commands;
using StreamMaster.Application.Profiles.Queries;

namespace StreamMaster.Application.Profiles
{
    public interface IProfilesController
    {        
        Task<ActionResult<List<FileOutputProfileDto>>> GetFileProfiles();
        Task<ActionResult<List<VideoOutputProfileDto>>> GetVideoProfiles();
        Task<ActionResult<APIResponse>> AddFileProfile(AddFileProfileRequest request);
        Task<ActionResult<APIResponse>> AddVideoProfile(AddVideoProfileRequest request);
        Task<ActionResult<APIResponse>> RemoveFileProfile(RemoveFileProfileRequest request);
        Task<ActionResult<APIResponse>> RemoveVideoProfile(RemoveVideoProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateFileProfile(UpdateFileProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateVideoProfile(UpdateVideoProfileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IProfilesHub
    {
        Task<List<FileOutputProfileDto>> GetFileProfiles();
        Task<List<VideoOutputProfileDto>> GetVideoProfiles();
        Task<APIResponse> AddFileProfile(AddFileProfileRequest request);
        Task<APIResponse> AddVideoProfile(AddVideoProfileRequest request);
        Task<APIResponse> RemoveFileProfile(RemoveFileProfileRequest request);
        Task<APIResponse> RemoveVideoProfile(RemoveVideoProfileRequest request);
        Task<APIResponse> UpdateFileProfile(UpdateFileProfileRequest request);
        Task<APIResponse> UpdateVideoProfile(UpdateVideoProfileRequest request);
    }
}
