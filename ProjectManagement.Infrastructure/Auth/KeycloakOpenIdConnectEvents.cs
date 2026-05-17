using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace ProjectManagement.Infrastructure.Auth
{
    public class KeycloakOpenIdConnectEvents : OpenIdConnectEvents
    {
        private readonly ProjectManagementDbContext _dbContext;
        public KeycloakOpenIdConnectEvents(ProjectManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
            if (claimsIdentity is null)
                return;

            await MapRealmRolesAsync(context, claimsIdentity);
            await ProvisionOrRefreshUserAsync(context, claimsIdentity);

            await base.TokenValidated(context);
        }

        private static Task MapRealmRolesAsync(TokenValidatedContext context, ClaimsIdentity claimsIdentity)
        {
            foreach (var roleName in GetRealmRoles(context))
            {
                if (!claimsIdentity.HasClaim(ClaimTypes.Role, roleName))
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                }
            }

            return Task.CompletedTask;
        }

        private static IEnumerable<string> GetRealmRoles(TokenValidatedContext context)
        {
            foreach (var roleClaim in context.Principal?.FindAll("roles") ?? Enumerable.Empty<Claim>())
            {
                foreach (var role in ReadRolesClaim(roleClaim.Value))
                {
                    yield return role;
                }
            }

            var realmAccessClaim = context.Principal?.FindFirst("realm_access")?.Value;
            if (!string.IsNullOrWhiteSpace(realmAccessClaim))
            {
                foreach (var role in ReadRolesFromRealmAccessJson(realmAccessClaim))
                {
                    yield return role;
                }
            }

            foreach (var token in new[]
            {
                context.TokenEndpointResponse?.IdToken,
                context.TokenEndpointResponse?.AccessToken
            })
            {
                foreach (var role in ReadRolesFromToken(token))
                {
                    yield return role;
                }
            }
        }

        private static IEnumerable<string> ReadRolesClaim(string rolesClaimValue)
        {
            if (string.IsNullOrWhiteSpace(rolesClaimValue))
                yield break;

            if (rolesClaimValue.StartsWith("[", StringComparison.Ordinal))
            {
                using var roles = JsonDocument.Parse(rolesClaimValue);
                if (roles.RootElement.ValueKind != JsonValueKind.Array)
                    yield break;

                foreach (var role in roles.RootElement.EnumerateArray())
                {
                    var roleName = role.GetString();
                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        yield return roleName;
                    }
                }

                yield break;
            }

            yield return rolesClaimValue;
        }

        private static IEnumerable<string> ReadRolesFromToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                yield break;

            var tokenParts = token.Split('.');
            if (tokenParts.Length < 2)
                yield break;

            var payloadJson = Base64UrlEncoder.Decode(tokenParts[1]);
            using var payload = JsonDocument.Parse(payloadJson);

            if (payload.RootElement.TryGetProperty("roles", out var roles))
            {
                foreach (var role in ReadRolesElement(roles))
                {
                    yield return role;
                }
            }

            if (payload.RootElement.TryGetProperty("realm_access", out var realmAccess))
            {
                foreach (var role in ReadRolesFromRealmAccessElement(realmAccess))
                {
                    yield return role;
                }
            }
        }

        private static IEnumerable<string> ReadRolesFromRealmAccessJson(string realmAccessJson)
        {
            using var realmAccess = JsonDocument.Parse(realmAccessJson);
            foreach (var role in ReadRolesFromRealmAccessElement(realmAccess.RootElement))
            {
                yield return role;
            }
        }

        private static IEnumerable<string> ReadRolesFromRealmAccessElement(JsonElement realmAccess)
        {
            if (!realmAccess.TryGetProperty("roles", out var roles) || roles.ValueKind != JsonValueKind.Array)
                yield break;

            foreach (var role in roles.EnumerateArray())
            {
                var roleName = role.GetString();
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    yield return roleName;
                }
            }
        }

        private static IEnumerable<string> ReadRolesElement(JsonElement roles)
        {
            if (roles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in roles.EnumerateArray())
                {
                    var roleName = role.GetString();
                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        yield return roleName;
                    }
                }
            }
            else if (roles.ValueKind == JsonValueKind.String)
            {
                var roleName = roles.GetString();
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    yield return roleName;
                }
            }
        }

        private async Task ProvisionOrRefreshUserAsync(TokenValidatedContext context, ClaimsIdentity claimsIdentity)
        {
            var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value
                 ?? context.Principal?.FindFirst("email")?.Value;

            if(string.IsNullOrWhiteSpace(email))
                return; 

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
            if (existingUser is not null)
            {
                EnsureApplicationRoleClaim(claimsIdentity, existingUser.Role);
                return;
            }

            var firstName = context.Principal?.FindFirst(ClaimTypes.GivenName)?.Value
                ?? context.Principal?.FindFirst("given_name")?.Value
                ?? "Unknown";

            var lastName = context.Principal?.FindFirst(ClaimTypes.Surname)?.Value
                ?? context.Principal?.FindFirst("family_name")?.Value
                ?? "Unknown";
            
            var isAdmin = context.Principal?.IsInRole("Admin") ?? false;
            var userRole = isAdmin ? Domain.Enums.UserRole.Admin : Domain.Enums.UserRole.Employee;

            var user = Domain.Entities.User.Create(email, firstName, lastName, userRole);
            
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            EnsureApplicationRoleClaim(claimsIdentity, user.Role);
        }

        private static void EnsureApplicationRoleClaim(ClaimsIdentity claimsIdentity, Domain.Enums.UserRole role)
        {
            var roleName = role.ToString();
            if (!claimsIdentity.HasClaim(ClaimTypes.Role, roleName))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));
            }
        }
    }
}
