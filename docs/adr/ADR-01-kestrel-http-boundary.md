# ADR-01: Kestrel HTTP boundary

## Контекст

Серверу нужна REST/JSON граница на .NET 10. CoreWCF не соответствует целевому протоколу и не нужен для HTTP-команд.

## Рассматривали варианты

- CoreWCF;
- HttpListener;
- ASP.NET Core Kestrel с minimal hosting.

## Решили

Использовать Kestrel через `WebApplication.CreateBuilder`. Сервер принимает `POST /command`, а порт задаётся конфигурацией с default `5080`.

## Где смотреть реализацию

`SpaceBattle.Server/Program.cs`, `SpaceBattle.Server/appsettings.json`.