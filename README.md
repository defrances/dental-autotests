# DMG Portal Playwright Tests

C# NUnit + Playwright E2E tests for DMG Portal. Ported from Cypress (see `CYPRESS_TO_PLAYWRIGHT_MAPPING.md`).

## Tech stack

- .NET 8.0  
- NUnit 3  
- Microsoft.Playwright / Microsoft.Playwright.NUnit  
- Serilog (logging)  
- appsettings.json (configuration)

## Requirements

- .NET 8.0 SDK  
- Playwright browsers — after the first build, run once:

```powershell
dotnet build
.\bin\Debug\net8.0\playwright.ps1 install chromium
```

Without installing browsers, tests fail with: `Executable doesn't exist at ...\playwright\chromium-...\chrome-win\chrome.exe`.

## Project structure

```
├── TestBase.cs           # Base test class (login, mocks, timeouts)
├── TestHost.cs           # SetUpFixture: Serilog + IHost
├── appsettings.json      # Default configuration
├── appsettings.Development.json
├── appsettings.Test.json
├── test.runsettings      # NUnit/Playwright run settings
├── test.dev.runsettings  # DMG_ENV=Development
├── test.test.runsettings # DMG_ENV=Test
├── Helpers/
│   ├── ApiMocks.cs       # API mocks (route/fixtures)
│   ├── TestSettings.cs   # Config and timeout resolution
│   └── UserCredentials.cs # Roles and logins (Default, Creator, Designer, …)
├── fixtures/             # JSON fixtures for mocks (dashboardCases.json, notifications.json, …)
└── Tests/
    ├── Login/            # Login, licenses
    ├── Dashboard/        # Cases, patients, files, hover bar
    ├── Case/             # Delegation, CAD, comments, session storage
    ├── Notification/     # Notification bell
    ├── Help/             # Manuals (URL)
    ├── TopBar/           # Top bar
    ├── Patient/          # Patients, pagination
    ├── User/             # User profile
    └── Connection/       # Tabs, button visibility (kaniedenta)
```

## Configuration

Main config file is `appsettings.json`. Environment variables override config values.

### Environment and URL

- **DMG_ENV** — environment: `Development`, `Test`, etc. Can also be set via runsettings (see below).  
- **DmgPortal:Environment** — same in appsettings.  
- **DmgPortal:BaseUrl** / **DMG_BASE_URL** — application URL (default `http://localhost:8080`).  
- **DmgPortal:AuthOrigin** / **DMG_AUTH_ORIGIN** — Keycloak origin (e.g. `https://dev.showdmc.com`).

### Users and password

- **DmgPortal:Password** — shared password for all users.  
- **DmgPortal:Users** — logins by role: `Default`, `Creator`, `CreatorPlus`, `Designer`, `Producer`, `TenantAdmin`, `DesignerKan`, `CreatorAusten`, `Lorenz`.  
- **DMG_PASSWORD** — environment variable for password.

For Test env (test.showdmc.com), override `DmgPortal:Password` and optionally `DmgPortal:Users` in `appsettings.Test.json` if needed.

### Playwright

In `appsettings.json`, section **Playwright**: browser, Headless, Viewport, Video, Har, timeouts (Login, Element, Selector, Dialog, etc.).

### Logging (Serilog)

In `appsettings.json`, section **Serilog**: level (Debug), console and file output (rolling by day). An additional timestamped log file is created in **Logs/** (`Logs/test-log-yyyyMMdd_HHmmss.txt`). For each test, context is logged in this format:

```
Test: <test name>,
	Description: <test [Description]>,
	Environment: <environment>,
	Role: <user role>,
	Login: <login>,
	Password: <password>,
	URL: <BaseUrl>
```

## Running tests

All tests (default config from `appsettings.json` + `test.runsettings`):

```powershell
dotnet test
```

**Development** environment (e.g. localhost + appsettings.Development.json):

```powershell
dotnet test --settings test.dev.runsettings
```

**Test** environment (test.showdmc.com, appsettings.Test.json):

```powershell
dotnet test --settings test.test.runsettings
```

Sequential run (single thread, useful for debugging):

```powershell
dotnet test -m:1 --settings test.test.runsettings
```

Run by NUnit category:

```powershell
dotnet test --filter "Category=Smoke"
dotnet test --filter "Category=Login"
```

Ensure the application is running (local or Test env) before running tests. Some tests use API mocks (`Helpers/ApiMocks.cs`, fixtures in `fixtures/`).

## User roles

Default role-to-login mapping in config: Creator/Default/TenantAdmin — lohmann; CreatorPlus — berry; Designer — kohlmann; Producer — mollenhauer; DesignerKan — schmidt; CreatorAusten — austen; Lorenz — lorenz. Tests use methods like `LoginAsync()`, `LoginAsCreatorAsync()`, `LoginAsDesignerAsync()`, etc.

## Mocks and fixtures

API mocks are attached before navigating to pages that call those APIs. Examples: `MockDashboardCases`, `MockNotifications`, `MockOrganizationDmg`, `MockCollaborationsWithConnections`, `MockOperationSteps`. Fixtures live in `fixtures/` (JSON). For Test env, e.g. `dashboardCases.Test.json` may be used. See `MOCKS_FROM_CYPRESS.md`, `DESIGNER_LICENSE_FLOW.md` for details.

## Troubleshooting

- **Executable doesn't exist ... chromium ...** — install browsers:  
  `.\bin\Debug\net8.0\playwright.ps1 install chromium`
- **Invalid username or password** (Keycloak) — for Test env, check password and logins in `appsettings.Test.json` or set `DMG_PASSWORD`.
- **Timeout on `[data-cy^='hoverButton-']`** — case list not returned; check `MockDashboardCases` and fixtures `fixtures/dashboardCases.json` / `dashboardCases.Test.json`. See `FAILURE_ANALYSIS.md`.

## Jenkins (local Docker)

The pipeline is defined in `Jenkinsfile` (parameters: **ENVIRONMENT**, **TEST_CATEGORY**). To run Jenkins locally in Docker:

**Requirements:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed (start it before running the commands below).

**Option 1 — docker compose (recommended):**

Builds a Jenkins image with .NET 8 SDK (`docker/Dockerfile.jenkins`) so the pipeline can run `dotnet restore/build/test`.

```powershell
cd e:\Work\MyGit\dental-autotests
docker compose build --no-cache   # first time or after changing Dockerfile
docker compose up -d
```

**Option 2 — single docker run command:**

```powershell
docker run -d --name dental-jenkins -p 8080:8080 -p 50000:50000 -v jenkins_home:/var/jenkins_home jenkins/jenkins:lts
```

**After starting:**

1. Open in browser: **http://localhost:8080**
2. Unlock password on first run — from container logs:
   ```powershell
   docker compose logs jenkins
   ```
   Look for a line like `Jenkins initial password is ...` or `Password: ...`
3. Complete the setup wizard and install required plugins (at least **Pipeline**, **JUnit**).
4. Create a **Pipeline** job: point to this project's repository and the path to Jenkinsfile (`Jenkinsfile`).

Jenkins data is stored in volume `jenkins_home` and persists between restarts. To stop: `docker compose down`.

**Note:** To run tests from the pipeline, the Jenkins agent needs .NET 8 SDK and (for Playwright) browsers installed. The Docker controller image does not include them — configure an agent with a label (e.g. `dotnet8`) on a machine with .NET 8 and Playwright, or run tests on a self-hosted agent.

### Git: SSH vs HTTPS

- **SSH** (`git@github.com:...`): If Jenkins shows **"Load key ... error in libcrypto"** on clone, the key in Credentials is incorrect. Fix: open the Credential (SSH Username with private key), paste the **private** key again (full file, from `-----BEGIN OPENSSH PRIVATE KEY-----` to `-----END OPENSSH PRIVATE KEY-----`), ensure there is a **newline** at the end. Save. If it still fails — try creating an RSA key: `ssh-keygen -t rsa -b 2048 -C "your@email"` and add it to Jenkins and GitHub.
- **HTTPS** (recommended in Docker): Does not depend on libcrypto. In Jenkins create a Credential **Username with password**: ID = `github-https`, Username = your GitHub login, Password = [Personal Access Token](https://github.com/settings/tokens) (repo scope). Apply the job config from `jenkins-job-config-https.xml` (script `scripts/Apply-JenkinsJobConfig.ps1 -ConfigPath ..\jenkins-job-config-https.xml`) so the job clones via HTTPS with this credential.

- **Failed to connect to github.com port 443**: No internet from the container (timeout). Try re-running the build (often a one-off failure). If it persists: check internet on the host, restart Docker Desktop; behind a proxy — in `docker-compose.yml` uncomment `HTTP_PROXY`/`HTTPS_PROXY` and configure the proxy in Jenkins (Manage Jenkins → System → HTTP Proxy).

## Further documentation

- `CYPRESS_TO_PLAYWRIGHT_MAPPING.md` — Cypress to Playwright mapping  
- `CYPRESS_LOGIC_VERIFICATION.md` — porting logic verification  
- `MOCKS_FROM_CYPRESS.md` — mocks from Cypress  
- `DESIGNER_LICENSE_FLOW.md` — Designer license flow  
- `FAILURE_ANALYSIS.md` — common failure analysis
