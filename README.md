# BankApp / BankingApp v1.0 — ASP.NET Core Web API (Back-end)

A simple **Banking Management System** back-end built as an **ASP.NET Core Web API** project.

This repository includes:
- API controllers and service logic (see `/Controllers` and `*Service.cs`)
- EF Core database setup and migrations (see `/Migrations`)
- Configuration (`appsettings.json`, `appsettings.Development.json`)
- Example HTTP requests (`SimpleController.http`)
- A step-by-step walkthrough with screenshots (`OlhaOmelchenko_Userguide.pdf`)

## Quick links

- ✅ **Getting started (run locally):** [GETTING_STARTED.md](GETTING_STARTED.md)
- 📘 **User guide (app walkthrough):** [OlhaOmelchenko_Userguide.pdf](OlhaOmelchenko_Userguide.pdf)

## Features (high level)

- **Accounts**: create/open, update, delete, view details/balance  
- **Transactions**: model/service layer (see `Transaction.cs` / `TransactionService.cs`)
- **Users / Auth DTOs**: e.g., `RegisterDto`, `LoginDto`, plus user/role models
- **SignalR notifications**: see `NotificationHub.cs`

## Tech stack

- **ASP.NET Core Web API**
- **Entity Framework Core** (migrations in `/Migrations`)
- **DTO-based request/response models**
- **SignalR** (notification hub)

## Repository structure (high level)

```text
BankApp/
  Controllers/                 # API controllers
  Migrations/                  # EF Core migrations
  Properties/                  # launch settings (ports/profiles), if present
  appsettings.json             # configuration
  appsettings.Development.json # local development overrides
  SimpleController.csproj      # project file
  SimpleController.sln         # solution file
  SimpleController.http        # example HTTP requests
  OlhaOmelchenko_Userguide.pdf # user guide / walkthrough
  GETTING_STARTED.md           # developer setup instructions

  SimpleController.csproj      # project file (.NET 8)
  SimpleController.sln         # solution file
  SimpleController.http        # sample HTTP requests (VS/VS Code)
  OlhaOmelchenko_Userguide.pdf # user guide / walkthrough

