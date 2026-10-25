# Sign in to MyApp

Start the app with `dotnet run --project src/MyApp.AppHost`, open *myapp-web* from the Aspire dashboard and sign in with one of these users:

| User | Password | Roles |
|---|---|---|
| `info@coworkee.de` | `123Pa$$word!` | Administrator |
| `fgilde@gmail.com` | `123Pa$$word!` | Administrator |
| `john@coworkee.de` | `123Pa$$word!` | - |

The AirWorks web desktop *myapp-airworks* signs in the same way (it is a second client of the same sign-in) and works on the same data.

The workflow designer *myapp-elsastudio* (Elsa Studio, *Administration → Workflow designer* in the app) signs in the same way; it needs the permission `Workflows.Elsa.Manage`, which the administrators have. John is a product manager: he submits products, the administrators approve them in *Approvals*; the *Approval* column of *Products* and *Sales orders* filters by the state, *Awaiting my approval* included. Every row of brands, products and sales orders has a *History* button with restore, and deleted brands and products come back from *Deleted records* in the table toolbar. Administrators manage webhooks under *Administration → System → Webhooks* (`/cw/admin/webhooks`); the seeded example endpoint is switched off.

The *Keycloak* button on the sign-in page takes the same users with the same passwords (realm `myapp`); the app host brings the realm up to date on every start.

Visitors can also create an account on the sign-in page: they confirm their email and an administrator activates them (settings `Account.AllowRegistration`, `Account.RegistrationRequiresEmailConfirmation`, `Account.RegistrationRequiresActivation`).

## Development tools

The administrator `info@coworkee.de` signs in with the same email and password to the tools the app host starts next to the app (open them from the Aspire dashboard):

- *grafana*: dashboards over the metrics, logs and traces of every service and the app database (folder *MyApp*).
- *webdatastudio*: the app database in the browser.

Both also offer the *Keycloak* button; users with the realm role *Administrator* are administrators there, everybody else may look.

- *aspireui*: host and try more apps and stacks next to MyApp; what you host there also shows up on the admin page *Services* of the app. Sign in as `info@coworkee.de` with the same password, or at the auth server of the app (only administrators of the system organisation get in, as AspireUI admins). Its data lives in the volume `myapp-aspireui-data`.

The users and passwords come from `src/MyApp.Migrations/DemoUsers.cs` and are seeded on the first start of an empty database. They are for development only: change them before you deploy anywhere.
