using Microsoft.AspNetCore.Authorization;

using StreamMaster.Domain.Configuration;

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

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.Equals(POLICY_NAME, StringComparison.OrdinalIgnoreCase))
            {
                AuthorizationPolicyBuilder policy = new AuthorizationPolicyBuilder(settings.Value.AuthenticationMethod.ToString())
                    .RequireAuthenticatedUser();
                return policy.Build();
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName).Result;
        }
    }
}