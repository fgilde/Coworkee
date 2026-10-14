# Sign in to MyApp

Start the app with `dotnet run --project src/MyApp.AppHost`, open *myapp-web* from the Aspire dashboard and sign in with one of these users:

| User | Password | Roles |
|---|---|---|
| `info@coworkee.de` | `123Pa$$word!` | Administrator |
| `fgilde@gmail.com` | `123Pa$$word!` | Administrator |
| `john@coworkee.de` | `123Pa$$word!` | - |

The AirWorks web desktop *myapp-airworks* signs in the same way (it is a second client of the same sign-in) and works on the same data.

The *Keycloak* button on the sign-in page takes the same users with the same passwords (realm `myapp`); the app host brings the realm up to date on every start.

Visitors can also create an account on the sign-in page: they confirm their email and an administrator activates them (settings `Account.AllowRegistration`, `Account.RegistrationRequiresEmailConfirmation`, `Account.RegistrationRequiresActivation`).

## Development tools

The administrator `info@coworkee.de` signs in with the same email and password to the tools the app host starts next to the app (open them from the Aspire dashboard):

- *grafana*: dashboards over the metrics, logs and traces of every service and the app database (folder *MyApp*).
- *webdatastudio*: the app database in the browser.

Both also offer the *Keycloak* button; users with the realm role *Administrator* are administrators there, everybody else may look.

- *aspireui*: host and try more apps and stacks next to MyApp; what you host there also shows up on the admin page *Services* of the app. Sign in as `info@coworkee.de` with the same password, or with the *Keycloak* button (administrators of the realm become AspireUI admins, everybody else gets the app-user permissions). Its data lives in the volume `aspireui-data-aspireui`.

The users and passwords come from `src/MyApp.Migrations/DemoUsers.cs` and are seeded on the first start of an empty database. They are for development only: change them before you deploy anywhere.
