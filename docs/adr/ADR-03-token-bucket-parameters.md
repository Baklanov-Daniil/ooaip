# ADR-03: Per-player token bucket

## Контекст

Ограничение должно быть независимым для каждого игрока, работать по монотонным часам и не допускать бесконечного роста таблицы.

## Рассматривали варианты

- глобальное ограничение на весь сервер;
- фиксированные окна;
- token bucket на username с TTL и LRU-вытеснением.

## Решили

Использовать token bucket с параметрами `RateLimitRefillPerSecond`, `RateLimitBurstCapacity`, `RateLimitMaxEntries` и `RateLimitEntryTtlSeconds`. Время представлено монотонными timestamp, пополнение вычисляется непрерывно, а при переполнении удаляется наименее недавно использованная запись.

## Где смотреть реализацию

`SpaceBattle.Server/TokenBucketRateLimiter.cs`, `SpaceBattle.Server/appsettings.json`.