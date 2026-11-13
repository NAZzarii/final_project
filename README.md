# GameMatch

Сайт для пошуку тіммейтів (ASP.NET Core MVC + SQLite + Identity).

## Запуск
```bash
dotnet restore
dotnet run
```
Відкрий http://localhost:5080. База `app.db` створюється сама при першому запуску (разом з іграми та демо-лобі).

Іконки ігор лежать у `wwwroot/img/games/` (власні SVG). Колір кожної гри задається в `wwwroot/css/site.css` (`[data-game=...]`).

## Акаунти
| Роль | Email | Пароль |
|------|-------|--------|
| Admin | admin@gamematch.local | Admin123 |
| Демо | kostya@demo.local / nikita@demo.local | Demo123 |

Вхід у Identity відбувається за **email**.

## Хто за що відповідає (файли)
- **Главний:** Program.cs, Data/*, Models/*, Areas/Identity
- **Костя:** Controllers/LobbiesController.cs, Controllers/HomeController.cs, Views/Lobbies/Create.cshtml, Views/Home/Index.cshtml
- **Нікіта:** Views/Shared/*, wwwroot/css/site.css, wwwroot/img/*, Views/Profile/*, Views/Lobbies/Details.cshtml
- **Сергій:** Controllers/RequestsController.cs, Controllers/AdminController.cs, Views/Requests/*, Views/Admin/*

## Примітка про міграції
Для простоти використано `EnsureCreated()` (у Data/DbSeeder.cs). Якщо потрібні EF-міграції:
1. видали `app.db`, заміни `EnsureCreatedAsync()` на `MigrateAsync()`;
2. `dotnet tool install --global dotnet-ef`
3. `dotnet ef migrations add InitialCreate`
