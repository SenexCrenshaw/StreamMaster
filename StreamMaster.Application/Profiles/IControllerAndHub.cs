using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Profiles.Commands;

namespace StreamMaster.Application.Profiles
{
    public interface IProfilesController
    {        
        Task<ActionResult<APIResponse>> AddFFMPEGProfile(AddFFMPEGProfileRequest request);
        Task<ActionResult<APIResponse>> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IProfilesHub
    {
        Task<APIResponse> AddFFMPEGProfile(AddFFMPEGProfileRequest request);
        Task<APIResponse> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request);
        Task<APIResponse> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request);
    }
}
