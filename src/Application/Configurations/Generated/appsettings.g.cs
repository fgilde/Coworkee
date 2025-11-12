/// <summary>
/// --- AUTO GENERATED CODE (12.11.2025 23:10:57) ---
/// --- ServerConfiguration ---
/// </summary>

namespace Coworkee.Application.Configurations
{
	public partial class ServerConfiguration
	{
		public string ClientUrl { get; set; }
		public ConnectionStrings ConnectionStrings { get; set; }
		public PublicSettings PublicSettings { get; set; }
		public string AllowedHosts { get; set; }
		public AppConfiguration AppConfiguration { get; set; }
		public CognitiveServices CognitiveServices { get; set; }
		public ApiDocumentation ApiDocumentation { get; set; }
		public MailConfiguration MailConfiguration { get; set; }
		public BackupOptions BackupOptions { get; set; }
	}

	public partial class BackupOptions
	{
		public string BucketName { get; set; }
	}

	public partial class MailConfiguration
	{
		public string SendGridApiKey { get; set; }
		public string From { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string DisplayName { get; set; }
	}

	public partial class ApiDocumentation
	{
		public bool RequireLogin { get; set; }
		public bool RequirePermission { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Contact Contact { get; set; }
		public License License { get; set; }
	}

	public partial class License
	{
		public string Name { get; set; }
		public string SpdxId { get; set; }
		public string Url { get; set; }
	}

	public partial class Contact
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string Url { get; set; }
	}

	public partial class CognitiveServices
	{
		public OpenAi OpenAi { get; set; }
		public Translation Translation { get; set; }
	}

	public partial class Translation
	{
		public string Key { get; set; }
		public string TextTranslationEndpoint { get; set; }
		public string DocumentTranslationEndpoint { get; set; }
		public string Region { get; set; }
	}

	public partial class OpenAi
	{
		public string ApiKey { get; set; }
		public string Model { get; set; }
	}

	public partial class AppConfiguration
	{
		public IdHashing IdHashing { get; set; }
		public string Secret { get; set; }
	}

	public partial class IdHashing
	{
		public bool Enabled { get; set; }
		public int MinLength { get; set; }
		public bool AllowAccessWithNotHashedId { get; set; }
		public string Salt { get; set; }
	}

	public partial class PublicSettings
	{
		public bool AssistantAvailable { get; set; }
		public string ContactAddress { get; set; }
		public bool HostClientInServer { get; set; }
		public UserRegistration UserRegistration { get; set; }
		public LoginSettings LoginSettings { get; set; }
	}

	public partial class LoginSettings
	{
		public bool AllowLoginWithUsername { get; set; }
		public System.Collections.Generic.List<string> AllowedEmails { get; set; }
	}

	public partial class UserRegistration
	{
		public bool Enabled { get; set; }
		public bool RequireAddress { get; set; }
		public bool RequiresAdministratorActivation { get; set; }
		public bool EmailConfirmationRequired { get; set; }
		public UsernameRules UsernameRules { get; set; }
		public PasswordRules PasswordRules { get; set; }
		public bool RequireDocuments { get; set; }
		public int RegistrationDocumentsMaxFileSize { get; set; }
		public System.Collections.Generic.List<string> RegistrationDocumentTypes { get; set; }
		public System.Collections.Generic.List<string> AllowedEmails { get; set; }
	}

	public partial class PasswordRules
	{
		public int MinLength { get; set; }
		public bool CapitalLetterRequired { get; set; }
		public bool LowercaseLetterRequired { get; set; }
		public bool NumberRequired { get; set; }
	}

	public partial class UsernameRules
	{
		public int MinLength { get; set; }
		public bool UsernameCanChangedAfterRegistration { get; set; }
		public bool EmailCanChangedAfterRegistration { get; set; }
	}

	public partial class ConnectionStrings
	{
		public string DefaultConnection { get; set; }
		public string Ollama { get; set; }
	}

}
