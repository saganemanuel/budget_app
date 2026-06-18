# Dokumentacja Techniczna Projektu - BudgetApp.Web

**Nazwa aplikacji:** BudgetApp.Web  
**Architektura:** ASP.NET Core MVC  
**Środowisko uruchomieniowe:** .NET 9.0  
**System bazodanowy:** Microsoft SQL Server  

---

## 1. Architektura rozwiązania

Aplikacja została zaprojektowana i zaimplementowana w oparciu o architekturę Model-View-Controller (MVC). Model-View-Controller (MVC) to wzorzec architektoniczny, który dzieli kod aplikacji na trzy niezależne sekcje: dane (Model), interfejs użytkownika (View) oraz logikę sterującą (Controller). Taki podział oddziela wygląd strony od operacji na bazie danych, co ułatwia zarządzanie kodem i jego późniejszą rozbudowę.

* **Model (Warstwa Danych):** Klasy języka C# definiujące strukturę danych biznesowych oraz obiekty transferu danych (ViewModels). Odpowiadają one za reprezentację informacji przetwarzanych wewnątrz systemu oraz walidację danych wejściowych po stronie serwera.
* **View (Warstwa Prezentacji):** Szablony Razor (.cshtml) renderowane po stronie serwera. Łączą one statyczny kod HTML5 z dynamicznym kodem C#. Za warstwę wizualną i dopasowanie interfejsu do urządzeń mobilnych (RWD) odpowiada framework Bootstrap 5. Dynamiczne operacje po stronie klienta realizowane są za pomocą czystego języka JavaScript (Vanilla JS) oraz biblioteki Chart.js.
* **Controller (Warstwa Logiki):** Klasy sterujące przepływem aplikacji. Przechwytują żądania HTTP od użytkownika, wchodzą w interakcję z warstwą danych za pośrednictwem kontekstu bazy danych (DbContext), a następnie przekazują przetworzone informacje do odpowiednich widoków.

---

## 2. Struktura bazy danych

Dostęp do warstwy trwałego przechowywania danych zrealizowano za pomocą mapowania obiektowo-relacyjnego (ORM) przy użyciu narzędzia Entity Framework Core w podejściu Code-First. Struktura tabel generowana jest automatycznie na podstawie klas C#.

Główna relacja biznesowa w systemie zachodzi pomiędzy kategoriami a wydatkami i ma charakter jeden-do-wielu (1:N).

### Tabela: Categories (Kategorie)
Odpowiada za przechowywanie definicji kategorii wydatków.
* `Id` (int, Klucz główny, Autoincrement) – Unikalny identyfikator rekordu.
* `Name` (nvarchar(max), Required) – Nazwa własna kategorii (np. Jedzenie, Transport).

### Tabela: Expenses (Wydatki)
Przechowuje szczegółowe informacje o zarejestrowanych operacjach finansowych.
* `Id` (int, Klucz główny, Autoincrement) – Unikalny identyfikator transakcji.
* `Title` (nvarchar(100), Required) – Tytuł lub opis wydatku (ograniczony do 100 znaków).
* `Amount` (decimal(18,2), Required) – Kwota transakcji, zapisywana z dokładnością do dwóch miejsc po przecinku (typ odpowiedni dla obliczeń finansowych).
* `Date` (datetime2, Required) – Data i czas rejestracji wydatku.
* `CategoryId` (int, Klucz obcy) – Identyfikator powiązanej kategorii, odwołujący się bezpośrednio do tabeli Categories.

### Tabele Systemu Bezpieczeństwa (ASP.NET Core Identity)
Wbudowany moduł bezpieczeństwa generuje zestaw tabel odpowiedzialnych za autentykację i autoryzację. Najważniejsze z nich to:
* `AspNetUsers` – Przechowuje konta użytkowników, w tym adresy e-mail oraz jednostronnie zahaszowane hasła. Encja ta jest reprezentowana w kodzie przez klasę ApplicationUser.
* `AspNetRoles` – Definiuje role uprawnień w systemie (w tym rolę "Admin").
* `AspNetUserRoles` – Tabela pośrednicząca, łącząca użytkowników z przypisanymi im rolami w relacji wiele-do-wielu (M:N).

---

## 3. Główne funkcjonalności systemu

### System Autoryzacji
Dostęp do obszarów zarządzania aplikacją (Panel Administratora) jest ograniczony. Kontroler AdminController oraz moduły zarządzania danymi posiadają atrybuty weryfikujące uprawnienia użytkownika ([Authorize(Roles = "Admin")]). Próba nieautoryzowanego dostępu skutkuje automatycznym przekierowaniem do widoku logowania. Hasła użytkowników są bezpiecznie haszowane przed zapisem w bazie za pomocą domyślnych algorytmów kryptograficznych ASP.NET Core Identity.

### Zarządzanie zasobami (Operacje CRUD)
Aplikacja implementuje pełną logikę przetwarzania danych dla modułów Wydatków oraz Kategorii:
* **Create (Tworzenie):** Dodawanie nowych rekordów za pomocą formularzy wyposażonych w walidację danych po stronie serwera (ModelState.IsValid). Zapobiega to wprowadzeniu błędnych danych, np. pustych tytułów.
* **Read (Odczyt):** Prezentacja danych w postaci czytelnych tabel. Dane są dynamicznie wyciągane z bazy za pomocą zapytań LINQ i przekazywane do widoków indeksowych.
* **Update (Edycja) & Delete (Usuwanie):** Możliwość modyfikacji istniejących wpisów lub ich bezpowrotnego usunięcia na podstawie unikalnego identyfikatora Id przekazywanego w żądaniu HTTP.

### Panel Administracyjny i Agregacja Danych (Dashboard)
 Prezentuje łączną sumę wszystkich wydatków, liczbę zdefiniowanych kategorii oraz tabelę ostatnich transakcji.

Za interaktywną część panelu odpowiada skrypt JavaScript współpracujący z biblioteką Chart.js. Skrypt ten pobiera surowe dane tekstowe z wygenerowanej tabeli, przetwarza je w strukturę asocjacyjną (sumując wydatki w ramach poszczególnych kategorii) i renderuje dynamiczny wykres. 

Zaimplementowany mechanizm pozwala użytkownikowi na natychmiastową zmianę typu prezentacji danych (wykres słupkowy na wykres kołowy) z poziomu interfejsu. Operacja ta odbywa się w całości po stronie klienta (w przeglądarce), bez konieczności ponownego przeładowania strony internetowej lub wykonywania dodatkowych zapytań do bazy danych.
