# SauceDemo Test Automation Suite 

Projekt testów automatycznych dla [saucedemo.com](https://www.saucedemo.com) stworzony w C# z użyciem Playwright i NUnit.

## 🛠 Stack technologiczny

| Obszar | Technologia |
|---|---|
| Język | C# / .NET 10 |
| Testy UI | Playwright |
| Testy API | RestSharp |
| Framework testów | NUnit |
| CI/CD | GitHub Actions |

## 📁 Struktura projektu
```
SauceDemoTests/
├── PageObjects/
│   ├── LoginPage.cs        # Strona logowania
│   ├── InventoryPage.cs    # Katalog produktów
│   ├── CartPage.cs         # Koszyk
│   └── CheckoutPage.cs     # Proces zamówienia
├── LoginTests.cs           # Testy logowania
├── InventoryTests.cs       # Testy katalogu
├── CheckoutTests.cs        # Testy E2E zakupów
├── ApiTests.cs             # Testy API
└── .github/workflows/
    └── ci.yml              # Pipeline CI/CD
```

## 🧪 Scenariusze testowe (27 testów)

### Logowanie
- ✅ Poprawne logowanie standardowego użytkownika
- ✅ Logowanie różnych typów użytkowników (problem, visual, performance, error)
- ✅ Błędne hasło — komunikat o błędzie
- ✅ Zablokowany użytkownik — komunikat o błędzie

### Katalog produktów
- ✅ Strona wyświetla 6 produktów
- ✅ Sortowanie A-Z i Z-A
- ✅ Sortowanie cena rosnąco i malejąco
- ✅ Dodawanie produktów do koszyka
- ✅ Usuwanie produktów z koszyka
- ✅ Walidacja cen produktów

### Checkout E2E
- ✅ Pełny happy path — od dodania do potwierdzenia zamówienia
- ✅ Checkout ze wszystkimi 6 produktami
- ✅ Walidacja formularza — brak imienia
- ✅ Anulowanie checkout

### API (JSONPlaceholder)
- ✅ GET - pobieranie listy postów
- ✅ GET - pobieranie pojedynczego posta
- ✅ POST - tworzenie nowego posta
- ✅ PUT - aktualizacja posta
- ✅ DELETE - usunięcie posta
- ✅ 404 - nieistniejący zasób

## 🚀 Uruchomienie lokalne

### Wymagania
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PowerShell](https://github.com/PowerShell/PowerShell/releases)

### Instalacja
```bash
# Klonuj repo
git clone https://github.com/SzymonKadziela/SauceDemoTests.git
cd SauceDemoTests/SauceDemoTests

# Przywróć zależności
dotnet restore

# Zainstaluj przeglądarki Playwright
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### Uruchomienie testów
```bash
# Wszystkie testy
dotnet test

# Konkretna klasa
dotnet test --filter "ClassName=SauceDemoTests.LoginTests"
```

## ⚙️ CI/CD

Pipeline GitHub Actions uruchamia się automatycznie przy każdym Pull Request i push na `main`.