using Microsoft.AspNetCore.Authorization;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;

namespace StreamMaster.Infrastructure.Authentication
{
    public class UiAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IOptions<Setting> settings) : IAuthorizationPolicyProvider
    {
        private const string POLICY_NAME = "UI";

        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; } = new DefaultAuthorizationPolicyProvider(options);

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return FallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.EqualsIgnoreCase(POLICY_NAME))
            {
                AuthorizationPolicyBuilder policy = new AuthorizationPolicyBuilder(settings.Value.AuthenticationMethod)
                    .RequireAuthenticatedUser();
                return policy.Build();
            }

            return await FallbackPolicyProvider.GetPolicyAsync(policyName).ConfigureAwait(false);
        }
    }
}