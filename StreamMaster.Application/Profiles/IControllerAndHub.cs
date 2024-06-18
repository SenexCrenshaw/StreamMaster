using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Profiles.Commands;
using StreamMaster.Application.Profiles.Queries;

namespace StreamMaster.Application.Profiles
{
    public interface IProfilesController
    {        
        Task<ActionResult<List<OutputProfileDto>>> GetOutputProfiles();
        Task<ActionResult<List<VideoOutputProfileDto>>> GetVideoProfiles();
        Task<ActionResult<APIResponse>> AddOutputProfile(AddOutputProfileRequest request);
        Task<ActionResult<APIResponse>> AddVideoProfile(AddVideoProfileRequest request);
        Task<ActionResult<APIResponse>> RemoveOutputProfile(RemoveOutputProfileRequest request);
        Task<ActionResult<APIResponse>> RemoveVideoProfile(RemoveVideoProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateOutputProfile(UpdateOutputProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateVideoProfile(UpdateVideoProfileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IProfilesHub
    {
        Task<List<OutputProfileDto>> GetOutputProfiles();
        Task<List<VideoOutputProfileDto>> GetVideoProfiles();
        Task<APIResponse> AddOutputProfile(AddOutputProfileRequest request);
        Task<APIResponse> AddVideoProfile(AddVideoProfileRequest request);
        Task<APIResponse> RemoveOutputProfile(RemoveOutputProfileRequest request);
        Task<APIResponse> RemoveVideoProfile(RemoveVideoProfileRequest request);
        Task<APIResponse> UpdateOutputProfile(UpdateOutputProfileRequest request);
        Task<APIResponse> UpdateVideoProfile(UpdateVideoProfileRequest request);
    }
}
