using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Services;

namespace StreamMaster.Infrastructure.Authentication
{
    public class UiAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_NAME = "UI";
        private readonly ISettingsService _settingsService;

        public UiAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, ISettingsService settingsService)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
            _settingsService = settingsService;
        }

        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return FallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.Equals(POLICY_NAME, StringComparison.OrdinalIgnoreCase))
            {
                Setting setting = await _settingsService.GetSettingsAsync();
                AuthorizationPolicyBuilder policy = new AuthorizationPolicyBuilder(setting.AuthenticationMethod.ToString())
                    .RequireAuthenticatedUser();
                return policy.Build();
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName).Result;
        }
    }
}