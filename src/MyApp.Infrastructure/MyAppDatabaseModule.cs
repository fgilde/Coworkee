using Coworkee.Auditing;
using Coworkee.AuthServer;
using Coworkee.Core.Modularity;
using Coworkee.Localization;
using Coworkee.Localization.Resources;
using Coworkee.Mailing;
using Coworkee.Notifications;
using Coworkee.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Infrastructure;

[DependsOn(typeof(MyAppInfrastructureModule), typeof(CoworkeeAuthStoreModule), typeof(CoworkeeMailingModule), typeof(CoworkeeAuditingModule), typeof(CoworkeeThemingModule),
    typeof(CoworkeeNotificationsModule), typeof(Coworkee.Account.CoworkeeAccountModule), typeof(CoworkeeLocalizationModule), typeof(Coworkee.Backup.CoworkeeBackupModule), typeof(Coworkee.Chat.CoworkeeChatModule), typeof(Coworkee.Ai.CoworkeeAiModule), typeof(Coworkee.Features.CoworkeeFeaturesModule), typeof(Coworkee.Files.CoworkeeFilesModule), typeof(Coworkee.Social.CoworkeeSocialModule), typeof(MyApp.Catalog.MyAppCatalogModule),
    typeof(MyApp.Documents.MyAppDocumentsModule))]
public sealed class MyAppDatabaseModule : CoworkeeModule
{
    public override void ConfigureServices(ModuleServiceContext context) =>
        context.Services.AddSingleton<ILocalizationResourceContributor, MyAppTexts>();
}
