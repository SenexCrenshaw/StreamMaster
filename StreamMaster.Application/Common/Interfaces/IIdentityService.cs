using StreamMaster.Application.Common.Models;

namespace StreamMaster.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<string> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);
}