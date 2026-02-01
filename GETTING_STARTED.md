````md
# GETTING_STARTED — BankApp / BankingApp v1.0

This guide explains how to **clone, configure, run, and test** the ASP.NET Core Web API locally.  
📘 For the app walkthrough (what the app does), see: **[OlhaOmelchenko_Userguide.pdf](OlhaOmelchenko_Userguide.pdf)**

## Prerequisites
- **.NET SDK** (check the target framework in `SimpleController.csproj`)
- A database supported by the configured **Entity Framework Core** provider (see `ApplicationDbContext.cs` + `appsettings*.json`)
- Recommended: **Visual Studio 2022** (or Rider / VS Code)
- Recommended: **EF Core CLI tool** (`dotnet-ef`) for applying migrations

## 1) Clone the repository
```bash
git clone https://github.com/girlWithPython/BankApp.git
cd BankApp
````

## 2) Restore dependencies

```bash
dotnet restore
```

## 3) Configure the database connection

Open and update your local connection string/settings:

* `appsettings.json`
* `appsettings.Development.json`

Tip: avoid committing secrets. Prefer environment variables or **.NET User Secrets** for credentials.

### Where to find the connection string name

In `appsettings.json`, look for a section like:

```json
"ConnectionStrings": {
  "DefaultConnection": "..."
}
```

The key (e.g., `DefaultConnection`) is the **connection string name**.

If the name is not obvious, check `Program.cs` for something like:

```csharp
builder.Configuration.GetConnectionString("DefaultConnection");
```

That `"DefaultConnection"` value is the name you must match in `appsettings*.json`.

If your setup uses environment variables, the common .NET naming pattern is:

```text
ConnectionStrings__DefaultConnection
```

(Replace `DefaultConnection` with your actual key name.)

## 4) Apply EF Core migrations (create/update the database)

Install EF tooling (only if you don’t already have it):

```bash
dotnet tool install --global dotnet-ef
```

Apply migrations:

```bash
dotnet ef database update
```

If you need to specify the project explicitly:

```bash
dotnet ef database update --project SimpleController.csproj
```

## 5) Run the API

Run from the command line:

```bash
dotnet run --project SimpleController.csproj
```

Or run from Visual Studio:

1. Open `SimpleController.sln`
2. Run (F5)

The console output will show the local URL(s) and ports.

## 6) Test the API

### Option A — Use the included HTTP requests file (recommended)

1. Open `SimpleController.http`
2. Update the base URL if your port differs
3. Run requests:

   * Visual Studio: built-in `.http` runner
   * VS Code: REST Client extension

### Option B — Swagger UI (if enabled in `Program.cs`)

Open:

* `https://localhost:<port>/swagger`

### Option C — Postman/Insomnia

Recreate requests based on `SimpleController.http`

```
::contentReference[oaicite:0]{index=0}
```
