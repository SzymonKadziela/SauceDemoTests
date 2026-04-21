# SauceDemo Test Automation Suite

Automated test suite for [saucedemo.com](https://www.saucedemo.com) built in C# using Playwright and NUnit.

## 🛠 Tech Stack

| Area | Technology |
|---|---|
| Language | C# / .NET 10 |
| UI Tests | Playwright |
| API Tests | RestSharp |
| Test Framework | NUnit |
| CI/CD | GitHub Actions |

## 🏗 Architecture

- **Page Object Model** - each page has its own class
- **Configuration layer** - credentials and settings loaded from `appsettings.json`
- **UserFactory** - test data generated dynamically via `TestCaseSource`
- **BaseTest** - automatic screenshots on failure

## 📁 Project Structure
```
SauceDemoTests/
├── PageObjects/
│   ├── LoginPage.cs        # Login page
│   ├── InventoryPage.cs    # Product catalog
│   ├── CartPage.cs         # Shopping cart
│   └── CheckoutPage.cs     # Checkout process
├── LoginTests.cs           # Login tests
├── InventoryTests.cs       # Catalog tests
├── CheckoutTests.cs        # E2E checkout tests
├── ApiTests.cs             # API tests
└── .github/workflows/
    └── ci.yml              # CI/CD pipeline
```

## 🧪 Test Scenarios (48 tests)

### Login
- ✅ Successful login with standard user
- ✅ Login with different user types (problem, visual, performance, error)
- ✅ Wrong password - error message displayed
- ✅ Locked out user - error message displayed
- ✅ Blocked access to inventory without login
- ✅ Login error disappears after clicking X
- ✅ Error message disappears when clicking X

### Product Catalog
- ✅ Page displays 6 products
- ✅ Sorting A-Z and Z-A
- ✅ Sorting by price ascending and descending
- ✅ Adding products to cart
- ✅ Adding 3 products - cart count equals 3
- ✅ Removing products from cart
- ✅ Product price validation
- ✅ Cart badge visibility - shows correct count and disappears when empty
- ✅ Product details title and price match catalog
- ✅ Continue shopping returns to catalog with cart preserved
- ✅ Cart badge count matches actual cart items
- ✅ Cheapest product price after sorting matches price in cart
- ✅ Verifies description is not empty
- ✅ Verifies problem user has broken images



### Cart
- ✅ Product visible in cart after adding
- ✅ Price in cart matches price in catalog

### Checkout E2E
- ✅ Full happy path - from adding to cart to order confirmation
- ✅ Checkout with all 6 products
- ✅ Form validation - missing first name
- ✅ Checkout cancellation
- ✅ Verifies URL change correctly on step two

### API (JSONPlaceholder)
- ✅ GET - fetch list of posts
- ✅ GET - fetch single post by ID
- ✅ GET — fetch user with valid fields and email format
- ✅ POST - create new post
- ✅ PUT - update existing post
- ✅ DELETE - delete post
- ✅ 404 -git  non-existent resource
- ✅ PUT - update existing post

## 🚀 Running Locally

### Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PowerShell](https://github.com/PowerShell/PowerShell/releases)

### Setup
```bash
# Clone the repository
git clone https://github.com/SzymonKadziela/SauceDemoTests.git
cd SauceDemoTests/SauceDemoTests

# Restore dependencies
dotnet restore

# Install Playwright browsers
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=SauceDemoTests.LoginTests"
```

## ⚙️ CI/CD

GitHub Actions pipeline runs automatically on every Pull Request and push to `main`.
