# ADR-02: Explicit validation chain

## Контекст

Входное сообщение должно проходить проверки в фиксированном порядке, а ранние ошибки не должны создавать записи в rate limiter.

## Рассматривали варианты

- один обработчик с последовательностью `if`;
- ASP.NET Core middleware для каждого независимого этапа;
- внешний pipeline framework.

## Решили

Каждый этап реализован отдельным middleware. Порядок в `Program.cs`: параллелизм разбора, тело, строгий JSON, игра, участник, rate limiter, постановка в очередь и ответ.

## Где смотреть реализацию

`SpaceBattle.Server/Middleware.cs`, `SpaceBattle.Server/Program.cs`.