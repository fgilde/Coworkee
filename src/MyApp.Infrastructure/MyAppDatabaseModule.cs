using Coworkee.Auditing;
using Coworkee.AuthServer;
using Coworkee.Core.Modularity;
using Coworkee.Mailing;
using Coworkee.Notifications;
using Coworkee.Theming;

namespace MyApp.Infrastructure;

[DependsOn(typeof(MyAppInfrastructureModule), typeof(CoworkeeAuthStoreModule), typeof(CoworkeeMailingModule), typeof(CoworkeeAuditingModule), typeof(CoworkeeThemingModule), typeof(CoworkeeNotificationsModule), typeof(Coworkee.Account.CoworkeeAccountModule), typeof(MyApp.Catalog.MyAppCatalogModule), typeof(MyApp.Documents.MyAppDocumentsModule))]
public sealed class MyAppDatabaseModule : CoworkeeModule;
