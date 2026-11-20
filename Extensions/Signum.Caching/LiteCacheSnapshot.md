# Lite cache snapshots

`[LiteCacheSnapshot]` opts a cached Lite reference into snapshot semantics. Its ID and Lite model are retained until its containing table cache is reset. Updates to the referenced entity do not register a semi-cache saving handler, an SQL notification subscription, or an invalidation dependency for that property.

```csharp
[LiteCacheSnapshot]
public Lite<ContactEntity>? From { get; set; }
```

The attribute also supports `MList<Lite<T>>` and typed `ImplementedBy` references. Use `ForEntityType = typeof(ContactEntity)` to select one implementation. Unmarked references, including other references to the same type, retain their existing behavior. A normal dependency elsewhere can still invalidate the containing cache and consequently renew its snapshots.

A snapshot is loaded lazily as part of cache loading/materialization, not captured transactionally at the instant a container is saved. A container reset, SQL notification on a containing table/list, broadcast reset of the container, or a global reset discards the snapshot. The next load reads current data, including new reference IDs. Stored Lite data can still appear as a subtable in cache statistics; it no longer independently follows target changes.

Use only where stale presentation data is acceptable and deletion of referenced entities is prevented by the application/database. This attribute does not prohibit deletion. It does not change ordinary retrievals outside these table caches or independently retained GlobalLazy results.

The initial implementation requires Lite models built from scalar fields and does not support `ImplementedByAll`. Unsupported marked fields fail during cache construction. Snapshot selection removes only the marked relation from the cache dependency graph, not the database relationship or foreign key.

SERP applies this policy to the five Contact references found in cache statistics:

- `ProbeEntity.From` and `ProbeEntity.Contacts` under scheduled tasks.
- `ChatDashboardPartEntity.DefaultContacts` under dashboards.
- `EmailSenderConfigurationEntity.DefaultFrom.EmailOwner` and `AdditionalRecipients[].EmailOwner`, configured by `StarterOverrideAttributes.ConfigureEmailContactSnapshots` only for `ContactEntity`. User references and ordinary email message owners retain their existing behavior.
