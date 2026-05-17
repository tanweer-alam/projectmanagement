using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
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
            await ProvisionuserAsync(context);

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
            var realmAccessClaim = context.Principal?.FindFirst("realm_access")?.Value;
            if (!string.IsNullOrWhiteSpace(realmAccessClaim))
            {
                foreach (var role in ReadRolesFromRealmAccessJson(realmAccessClaim))
                {
                    yield return role;
                }
            }

            var accessToken = context.TokenEndpointResponse?.AccessToken;
            if (string.IsNullOrWhiteSpace(accessToken))
                yield break;

            var tokenParts = accessToken.Split('.');
            if (tokenParts.Length < 2)
                yield break;

            var payloadJson = Base64UrlEncoder.Decode(tokenParts[1]);
            using var payload = JsonDocument.Parse(payloadJson);
            if (!payload.RootElement.TryGetProperty("realm_access", out var realmAccess))
                yield break;

            foreach (var role in ReadRolesFromRealmAccessElement(realmAccess))
            {
                yield return role;
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

        private async Task ProvisionuserAsync(TokenValidatedContext context)
        {
            var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value
                 ?? context.Principal?.FindFirst("email")?.Value;

            if(string.IsNullOrWhiteSpace(email))
                return; 

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
            if (existingUser is not null)
                return;

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
        }
    }
}
