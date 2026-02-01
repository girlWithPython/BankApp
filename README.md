This project aims to create an essential realisation of the back-end part of a simple banking application. This app should allow users to manage accounts (open new, edit, delete, get a balance)

# BankingApp v1.0 — .NET 8 Web API (Back-end)

A simple **Banking Management System** back-end built with **ASP.NET Core (.NET 8)**.  
The API provides CRUD operations for core banking entities (e.g., **Customer/User**, **Account**) and supports basic account management (create/open, edit, delete, view balance).  

> 📘 For a walkthrough and example flows, see **`OlhaOmelchenko_Userguide.pdf`** in this repository.

---

## Features

- **Accounts**
  - Create/open accounts
  - Update account details
  - Delete accounts
  - Get account balance / account details
- **Transactions** *(see `Transaction.cs` / `TransactionService.cs`)*
  - Transaction model and service layer to support transaction workflows
- **Users / Authentication**
  - DTOs for registration and login (e.g., `RegisterDto`, `LoginDto`)
  - App user/role models (e.g., `ApplicationUser`, `ApplicationRole`)
- **Real-time notifications (SignalR)**
  - Notification hub (see `NotificationHub.cs`)

---

## Tech stack

- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core** (migrations are included in `/Migrations`)
- **DTO-based API surface** (request/response models)
- **SignalR** (notification hub)

---

## Repository structure (high level)

```text
BankApp/
  Controllers/                 # API controllers
  Migrations/                  # EF Core migrations
  Properties/                  # project properties / launch settings (if present)
  *.cs                         # domain models, DTOs, services
  appsettings.json             # configuration (connection strings, etc.)
  appsettings.Development.json # dev overrides
  SimpleController.csproj      # project file (.NET 8)
  SimpleController.sln         # solution file
  SimpleController.http        # sample HTTP requests (VS/VS Code)
  OlhaOmelchenko_Userguide.pdf # user guide / walkthrough

