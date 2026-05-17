# Keycloak Setup Guide

This guide configures a local Keycloak instance for the `ProjectManagement` web application.

> [!NOTE]
> The usernames, passwords, and URLs below are intended for local development only. Do not commit real secrets to source control.

## 1. Start Keycloak

From the `ProjectManagement.Web` directory, start the Keycloak container:

```bash
docker-compose up -d
```

Keycloak will be available at:

- Application URL: `http://localhost:8080`
- Admin Console: `http://localhost:8080/admin`
- Username: `admin`
- Password: `admin`

## 2. Create the Realm

1. Log in to the Keycloak Admin Console.
2. Open the realm dropdown in the top-left corner.
3. Select **Create realm**.
4. Set the realm name to `project-management`.
5. Click **Create**.

## 3. Create the Client

1. Go to **Clients** and select **Create client**.
2. Under **General settings**:
   - Client type: `OpenID Connect`
   - Client ID: `project-management-web`
3. Click **Next**.
4. Under **Capability config**:
   - Client authentication: `ON`
   - Authorization: `OFF`
   - Authentication flow:
     - `Standard flow`: checked
     - `Direct access grants`: checked
5. Click **Next**.
6. Under **Login settings**:
   - Root URL: `https://localhost:5036`
   - Home URL: `https://localhost:5036`
   - Valid redirect URIs: `https://localhost:5036/*`
   - Valid post logout redirect URIs: `https://localhost:5036/*`
   - Web origins: `https://localhost:5036`
7. Click **Save**.
8. Open the **Credentials** tab and copy the generated client secret.

## 4. Create Realm Roles

Create these realm roles:

1. `Admin`
2. `Employee`

## 5. Create Users

### Admin user

1. Go to **Users** and select **Add user**.
2. Enter:
   - Username: `admin`
   - Email: `admin@example.com`
   - First name: `Admin`
   - Last name: `User`
   - Email verified: `ON`
3. Click **Create**.
4. Open the **Credentials** tab and set:
   - Password: `Admin123!`
   - Temporary: `OFF`
5. Open the **Role mapping** tab:
   - Select **Assign role**
   - Filter by realm roles
   - Assign `Admin`

### Employee users

Create the following employee accounts:

| Username | Email | First name | Last name | Password | Role |
| --- | --- | --- | --- | --- | --- |
| `john.doe` | `john.doe@example.com` | `John` | `Doe` | `Employee123!` | `Employee` |
| `jane.smith` | `jane.smith@example.com` | `Jane` | `Smith` | `Employee123!` | `Employee` |

## 6. Configure Role Claims in Tokens

Keycloak includes realm roles in tokens by default. Verify the mapper configuration:

1. Go to **Clients** > `project-management-web`.
2. Open the **Client scopes** tab.
3. Select `project-management-web-dedicated`.
4. Click **Add mapper** > **By configuration** > **User Realm Role**.
5. Configure:
   - Name: `realm roles`
   - Token claim name: `roles`
   - Add to ID token: `ON`
   - Add to access token: `ON`
6. Click **Save**.

## 7. Configure `appsettings.json`

Add the following configuration to `ProjectManagement.Web/appsettings.json`:

```json
{
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/project-management",
    "ClientId": "project-management-web",
    "ClientSecret": "<paste-client-secret-here>",
    "RequireHttpsMetadata": false
  }
}
```

For a GitHub-hosted project, prefer storing `ClientSecret` in user secrets, environment variables, or another secret store instead of committing it to the repository.

## 8. Test URLs

- OpenID Connect discovery document: `http://localhost:8080/realms/project-management/.well-known/openid-configuration`
- Authorization endpoint: `http://localhost:8080/realms/project-management/protocol/openid-connect/auth`
- Token endpoint: `http://localhost:8080/realms/project-management/protocol/openid-connect/token`

