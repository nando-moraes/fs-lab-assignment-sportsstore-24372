# Sports Store Assignment — Fernando Moraes (24372)

  ## How to Run Locally

  1. Clone the repo
  2. Set Stripe test keys using user secrets:
     dotnet user-secrets set "Stripe:SecretKey" "sk_test_YOUR_KEY" --project SportsStore
     dotnet user-secrets set "Stripe:PublishableKey" "pk_test_YOUR_KEY" --project SportsStore
  3. Run the application:
     dotnet run --project SportsStore
  4. Open browser at `http://localhost:5000`

  ## .NET Upgrade Notes

  - Upgraded from `net6.0` to `net10.0`
  - Updated packages:
    - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 6.0.0 → 10.0.0
    - `Microsoft.EntityFrameworkCore.Design` 6.0.0 → 10.0.0
    - `Microsoft.EntityFrameworkCore.SqlServer` 6.0.0 → 10.0.0
    - `Microsoft.NET.Test.Sdk` 16.11.0 → 17.12.0
    - `xunit` 2.4.1 → 2.9.3
    - `Moq` 4.16.1 → 4.20.72
  - No breaking changes required — solution builds with zero errors

  ## Stripe Configuration

  - Uses Stripe .NET SDK (`Stripe.net`)
  - Test keys obtained from `https://dashboard.stripe.com/test/apikeys`
  - Keys stored via `dotnet user-secrets` — never committed to source control
  - Use Stripe test card `4242 4242 4242 4242` with any future expiry and any CVC to test payments
  - Payment flow: Cart → Checkout → Stripe card input → Order confirmation
  - Failed and cancelled payments are handled and logged

  ## Logging Setup

  - Uses `Serilog.AspNetCore` with console and rolling file sinks
  - Log files written to `SportsStore/logs/` folder, new file created daily
  - Minimum log level configured in `appsettings.json` under the `Serilog` section
  - Logged events:
    - Application startup and shutdown
    - Checkout flow (start, success, validation errors)
    - Order creation with order ID and customer details
    - Payment success and failure with Stripe PaymentIntent ID

  ## CI Pipeline

  - GitHub Actions workflow at `.github/workflows/ci.yml`
  - Triggers on push to `main` and on pull requests
  - Pipeline steps: restore → build → test → upload test results
  - Pipeline fails if any test fails