using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Profiles.Commands;
using StreamMaster.Application.Profiles.Queries;

namespace StreamMaster.Application.Profiles
{
    public interface IProfilesController
    {        
        Task<ActionResult<List<CommandProfileDto>>> GetCommandProfiles();
        Task<ActionResult<OutputProfileDto>> GetOutputProfile(GetOutputProfileRequest request);
        Task<ActionResult<List<OutputProfileDto>>> GetOutputProfiles();
        Task<ActionResult<APIResponse>> AddCommandProfile(AddCommandProfileRequest request);
        Task<ActionResult<APIResponse>> AddOutputProfile(AddOutputProfileRequest request);
        Task<ActionResult<APIResponse>> RemoveCommandProfile(RemoveCommandProfileRequest request);
        Task<ActionResult<APIResponse>> RemoveOutputProfile(RemoveOutputProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateCommandProfile(UpdateCommandProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateOutputProfile(UpdateOutputProfileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IProfilesHub
    {
        Task<List<CommandProfileDto>> GetCommandProfiles();
        Task<OutputProfileDto> GetOutputProfile(GetOutputProfileRequest request);
        Task<List<OutputProfileDto>> GetOutputProfiles();
        Task<APIResponse> AddCommandProfile(AddCommandProfileRequest request);
        Task<APIResponse> AddOutputProfile(AddOutputProfileRequest request);
        Task<APIResponse> RemoveCommandProfile(RemoveCommandProfileRequest request);
        Task<APIResponse> RemoveOutputProfile(RemoveOutputProfileRequest request);
        Task<APIResponse> UpdateCommandProfile(UpdateCommandProfileRequest request);
        Task<APIResponse> UpdateOutputProfile(UpdateOutputProfileRequest request);
    }
}
