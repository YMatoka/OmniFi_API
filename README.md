# OmniFi API

![Alt text](OmniFi_API/Images/OmniFiLogo.png)

This project is an ASP.NET Core Web API that aggregates balance information from multiple sources, including traditional bank accounts and cryptocurrency accounts. The API provides a single endpoint to retrieve the consolidated balance data from both bank and crypto accounts, offering a comprehensive view of a user's financial status.

## Table of Contents
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Sample Request and Response](#sample-request-and-response)
- [Project Structure](#project-structure)
- [Future Enhancements](#future-enhancements)
- [License](#license)

## Features

- **Bank Account Balance Integration**: Connects with multiple bank APIs with a bridge Api to retrieve account balances.
- **Cryptocurrency Balance Integration**: Integrates with crypto exchange APIs (e.g., Kraken, Binance and Crypto.com) to fetch crypto wallet balances.
- **Consolidated Balance**: Aggregates balances from different sources into a single JSON response.
- **Modular and Extensible**: Easily add new bank or crypto integrations by creating new provider classes.
- **Secure and Configurable**: Sensitive information, like API keys and authentication tokens, is securely managed via configuration, environnement variables and Sql Server.
## Tech Stack

- **Backend**: ASP.NET Core Web API (C#)
- **Data Access**: Entity Framework Core
- **API Integrations**: Custom HTTP clients to interact with external bank and crypto APIs
- **Authentication**: JWT for user authentification and API Key for crypto exchange credentials

## API EndPoints

| Method |  Endpoint  | Description |
|:-----|:--------:|------:|
| GET   | /api/Bank/GetBank | Retrieve a bank by its id |
| GET   | /api/Bank/GetBanks  | Retrieve all the banks of the API |
| POST   | /api/BankAccount/CreateAuthorisationLink | Create an authorisation link in order to authorize the access to a bank account |
| GET   | /api/BankAccount/AuthorizationCallback | Retrieve the response of the bank account authorization and add it to the database|
| GET   | /api/BankAccount/GetAccessDuration | Get the lasted access duration to a bank account |
| DELETE   | /api/BankAccount/Delete | Deelte a bank account |
| GET   | /api/CryptoExchange/GetCryptoExchange| Retrieve a crypto exchange by its id   |
| GET   | /api/CryptoExchange/GetCryptoExchanges| Retrieve all the crypto exchanges of the API |
| POST   | /api/CryptoExchangeAccount/Create | Create a crypto exchange account with its API Keys|
| PUT   | /api/CryptoExchangeAccount/Put | Update a crypto exchange API Keys |
| DELETE   | /api/CryptoExchangeAccount/Delete | Delete a crypto exchange account |
| GET   | /api/FinancialAsset/GetFinancialAssets | Retrieve all current asset possessed by the user |
| GET   | /api/FinancialAsset/GetAggregatedFinancialAssets| Get the aggregated balance of the user's assets |
| GET   | /api/FinancialAsset/GetFinancialAssetsByAssetId | Retrieve a current asset possessed by the user by its id |
| GET   | /api/FinancialAssetHistory/GetAllFinancialHistoryAsset| Retrieve the history an user's assets |
| GET   | /api/FinancialAssetHistory/GetFinancialHistoryByAssetId |Retrieve the history an user's asset by its id |
| POST   | /api/PortfolioService/FetchPortfolio | Retrieve and refresh the balance of all user's assets |
| POST   | /api/UsersAuth/Login| Give to the client a JWT token in order to be authentified |
| PUT   | /api/UsersAuth/Put | Modify user information |
| POST   | /api/UsersAuth/Register | Create a new user |
| DELETE   | /api/UsersAuth/Delete | Delete an user |

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A supported IDE (e.g., [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/))
- Crypto API keys (e.g., from Crypto exchanges such as Kraken, Binance and Crypto.com)


### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/YMatoka/OmniFi_API.git
   cd 'OmniFi_API'

2. Install dependencies
dotnet restore

3. Run the API
dotnet run