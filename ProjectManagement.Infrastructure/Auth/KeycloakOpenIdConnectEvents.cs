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

            await ProvisionOrRefreshUserAsync(context, claimsIdentity);

            await base.TokenValidated(context);
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
