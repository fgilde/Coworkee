using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Coworkee.Infrastructure;

public class KeycloakSeeder
{
    private readonly HttpClient _httpClient;
    private readonly string _adminUsername;
    private readonly string _adminPassword;
    private readonly string _clientSecret;
    private readonly string _realm;
    private readonly string _keycloakUrl;
    private readonly string _clientName;
    private readonly string _clientId;

    public KeycloakSeeder(string realm, string adminUsername, string adminPassword, string keycloakUrl, string clientName, string clientSecret)
    {
        _realm = realm;
        _adminUsername = adminUsername;
        _adminPassword = adminPassword;
        _keycloakUrl = keycloakUrl;
        _clientSecret = clientSecret;
        _clientName = _clientId = clientName;
        _httpClient = new HttpClient();
    }

    public async Task CreateClientAsync(bool force = false)
    {
        var token = await GetAdminAccessTokenAsync();

        var clientExists = await ClientExistsAsync(token);
        if (clientExists && !force)
        {
            return;
        }

        if (clientExists && force)
        {
            await DeleteClientAsync(token);
        }

        var clientData = new
        {
            clientId = _clientId,
            name = _clientName,
            protocol = "openid-connect",
            redirectUris = new[]
            {
                "https://localhost:7044/account/login-callback",
                "*"
            },
            webOrigins = new[]
            {
                "https://localhost:7044",
                "*"
            },
            enabled = true,
            publicClient = false,
            secret = _clientSecret,
            directAccessGrantsEnabled = true,
            standardFlowEnabled = true,
            implicitFlowEnabled = false,
            protocolMappers = new[]
            {
                new {
                    name = "my-custom-claim",
                    protocol = "openid-connect",
                    protocolMapper = "oidc-usermodel-attribute-mapper",
                    consentRequired = false,
                    config = new Dictionary<string, string>
                    {
                        ["claim.name"] = "my-custom-claim",
                        ["jsonType.label"] = "String",
                        ["user.attribute"] = "myUserAttribute",
                        ["id.token.claim"] = "true",
                        ["access.token.claim"] = "true",
                        ["userinfo.token.claim"] = "true"
                    }
                }
            }
        };

        var json = JsonConvert.SerializeObject(clientData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsync($"{_keycloakUrl}/admin/realms/{_realm}/clients", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error on creating client: {response.ReasonPhrase}");
    }

    public async Task<bool> ClientExistsAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync($"{_keycloakUrl}/admin/realms/{_realm}/clients?clientId={_clientId}");

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var clients = JsonConvert.DeserializeObject<dynamic[]>(responseContent);
            return clients is { Length: > 0 };
        }

        return false;
    }

    private async Task DeleteClientAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync($"{_keycloakUrl}/admin/realms/{_realm}/clients?clientId={_clientId}");

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var clients = JsonConvert.DeserializeObject<dynamic[]>(responseContent);

            if (clients != null && clients.Length > 0)
            {
                var clientId = clients[0].id.ToString();
                var deleteResponse = await _httpClient.DeleteAsync($"{_keycloakUrl}/admin/realms/{_realm}/clients/{clientId}");

                if (!deleteResponse.IsSuccessStatusCode)
                    throw new Exception($"Error on deleting client: {deleteResponse.ReasonPhrase}");
            }
        }
    }

    public async Task<string?> GetUserIdAsync(string username)
    {
        var token = await GetAdminAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync($"{_keycloakUrl}/admin/realms/{_realm}/users?username={Uri.EscapeDataString(username)}");

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error on getting user: {response.ReasonPhrase}");

        var responseContent = await response.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<List<dynamic>>(responseContent);

        // No user with this username found
        if (users == null || users.Count == 0)
            return null;

        var user = users[0];
        return (string)user.id;
    }

    public async Task CreateUserAsync(string username, string password, string email)
    {
        var token = await GetAdminAccessTokenAsync();

        var existingUserId = await GetUserIdAsync(username);

        if (existingUserId != null)
        {
            Console.WriteLine("User already exists. User will be updated");

            // update email
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var updateData = new
            {
                email = email,
                enabled = true
            };
            var updateJson = JsonConvert.SerializeObject(updateData);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

            var updateResponse = await _httpClient.PutAsync($"{_keycloakUrl}/admin/realms/{_realm}/users/{existingUserId}", updateContent);

            if (!updateResponse.IsSuccessStatusCode)
                throw new Exception($"Error on updating user: {updateResponse.ReasonPhrase}");

            // Reset password
            var credentialData = new
            {
                type = "password",
                value = password,
                temporary = false
            };
            var credentialJson = JsonConvert.SerializeObject(credentialData);
            var credentialContent = new StringContent(credentialJson, Encoding.UTF8, "application/json");

            var credentialResponse = await _httpClient.PutAsync($"{_keycloakUrl}/admin/realms/{_realm}/users/{existingUserId}/reset-password", credentialContent);

            if (!credentialResponse.IsSuccessStatusCode)
                throw new Exception($"Error on updating password: {credentialResponse.ReasonPhrase}");
        }
        else
        {
            var userData = new
            {
                username,
                enabled = true,
                email,
                credentials = new[]
                {
                new
                {
                    type = "password",
                    value = password,
                    temporary = false
                }
            }
            };

            var json = JsonConvert.SerializeObject(userData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync($"{_keycloakUrl}/admin/realms/{_realm}/users", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error on creating user: {response.ReasonPhrase}");
            }

        }
    }

    public async Task CreateRoleAsync(string roleName)
    {
        var token = await GetAdminAccessTokenAsync();

        var roleData = new
        {
            name = roleName
        };

        var json = JsonConvert.SerializeObject(roleData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsync($"{_keycloakUrl}/admin/realms/{_realm}/roles", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Error on creating role: {response.ReasonPhrase}");
    }

    private async Task<string> GetAdminAccessTokenAsync()
    {
        var content = new StringContent($"client_id=admin-cli&username={_adminUsername}&password={_adminPassword}&grant_type=password", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await _httpClient.PostAsync($"{_keycloakUrl}/realms/{_realm}/protocol/openid-connect/token", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic tokenResponse = JsonConvert.DeserializeObject(responseContent);
            return tokenResponse.access_token;
        }

        throw new Exception($"Error to get access token: {response.ReasonPhrase}");
    }
}
