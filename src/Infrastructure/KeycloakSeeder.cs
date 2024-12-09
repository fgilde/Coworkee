namespace Coworkee.Infrastructure;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class KeycloakSeeder
{
    private readonly HttpClient _httpClient;
    private readonly string _adminUsername = "admin";
    private readonly string _adminPassword = "admin";
    private readonly string _realm = "master";
    private readonly string _keycloakUrl = "http://localhost:8080";

    private const string ClientName = "WeatherWeb";
    private const string ClientId = "WeatherWeb";

    public KeycloakSeeder()
    {
        _httpClient = new HttpClient();
    }

    public async Task CreateClientAsync(bool force = false)
    {
        var token = await GetAdminAccessTokenAsync();
        if (token == null)
        {
            Console.WriteLine("Fehler beim Abrufen des Zugriffstokens.");
            return;
        }

        var clientExists = await ClientExistsAsync(token);
        if (clientExists && !force)
        {
            Console.WriteLine("Client existiert bereits. Keine Aktion erforderlich.");
            return;
        }

        if (clientExists && force)
        {
            Console.WriteLine("Client existiert bereits. Lösche alten Client...");
            await DeleteClientAsync(token);
        }

        var clientData = new
        {
            clientId = ClientId,
            name = ClientName,
            protocol = "openid-connect",
            redirectUris = new[]
            {
                "https://localhost:5001/account/login-callback",
                "https://your-production-url.com/account/login-callback",
                "*"
            },
            webOrigins = new[]
            {
                "https://localhost:5001",
                "https://your-production-url.com",
                "*"
            },
            enabled = true,
            publicClient = false,
            secret = "dein-client-secret",
            directAccessGrantsEnabled = true,
            standardFlowEnabled = true,
            implicitFlowEnabled = false
        };

        var json = JsonConvert.SerializeObject(clientData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsync($"{_keycloakUrl}/admin/realms/{_realm}/clients", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Client erfolgreich erstellt.");
        }
        else
        {
            Console.WriteLine($"Fehler beim Erstellen des Clients: {response.ReasonPhrase}");
        }
    }

    public async Task<bool> ClientExistsAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync($"{_keycloakUrl}/admin/realms/{_realm}/clients?clientId={ClientId}");

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var clients = JsonConvert.DeserializeObject<dynamic[]>(responseContent);
            return clients != null && clients.Length > 0;
        }

        Console.WriteLine($"Fehler beim Prüfen, ob der Client existiert: {response.ReasonPhrase}");
        return false;
    }

    private async Task DeleteClientAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync($"{_keycloakUrl}/admin/realms/{_realm}/clients?clientId={ClientId}");

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var clients = JsonConvert.DeserializeObject<dynamic[]>(responseContent);

            if (clients != null && clients.Length > 0)
            {
                var clientId = clients[0].id.ToString();
                var deleteResponse = await _httpClient.DeleteAsync($"{_keycloakUrl}/admin/realms/{_realm}/clients/{clientId}");

                if (deleteResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Client erfolgreich gelöscht.");
                }
                else
                {
                    Console.WriteLine($"Fehler beim Löschen des Clients: {deleteResponse.ReasonPhrase}");
                }
            }
        }
    }

    public async Task CreateUserAsync(string username, string password, string email)
    {
        var token = await GetAdminAccessTokenAsync();
        if (token == null)
        {
            Console.WriteLine("Fehler beim Abrufen des Zugriffstokens.");
            return;
        }

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

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Benutzer erfolgreich erstellt.");
        }
        else
        {
            Console.WriteLine($"Fehler beim Erstellen des Benutzers: {response.ReasonPhrase}");
        }
    }

    public async Task CreateRoleAsync(string roleName)
    {
        var token = await GetAdminAccessTokenAsync();
        if (token == null)
        {
            Console.WriteLine("Fehler beim Abrufen des Zugriffstokens.");
            return;
        }

        var roleData = new
        {
            name = roleName
        };

        var json = JsonConvert.SerializeObject(roleData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsync($"{_keycloakUrl}/admin/realms/{_realm}/roles", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Rolle erfolgreich erstellt.");
        }
        else
        {
            Console.WriteLine($"Fehler beim Erstellen der Rolle: {response.ReasonPhrase}");
        }
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

        return null;
    }
}
