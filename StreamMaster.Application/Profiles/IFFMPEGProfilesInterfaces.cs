using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Profiles.Commands;

namespace StreamMaster.Application.Profiles;

public interface IProfilesController
{
    Task<ActionResult<FFMPEGProfileDtos>> GetFFMPEGProfiles();

    Task<ActionResult<UpdateSettingResponse>> AddFFMPEGProfile(AddFFMPEGProfileRequest request);
    Task<ActionResult<UpdateSettingResponse>> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request);
    Task<ActionResult<UpdateSettingResponse>> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request);
}


public interface IProfilesHub
{

    Task<UpdateSettingResponse> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request);
    Task<UpdateSettingResponse> AddFFMPEGProfile(AddFFMPEGProfileRequest request);
    Task<UpdateSettingResponse> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request);
    Task<FFMPEGProfileDtos> GetFFMPEGProfiles();
}
