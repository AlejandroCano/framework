using Signum.Entities.ConcurrentUser;

namespace Signum.Engine.ConcurrentUser;
public static class ConcurrentUserLogic
{
    public static Func<Type, bool> WatchSaveFor = null!; 

    public static void Start(SchemaBuilder sb, Func<Type, bool>? activatedFor)
    {
        if (sb.NotDefined(MethodBase.GetCurrentMethod()))
        {
            WatchSaveFor = activatedFor ?? (t => EntityKindCache.GetEntityKind(t) is not (EntityKind.System or EntityKind.SystemString));

            sb.Include<ConcurrentUserEntity>()
                .WithIndex(a => new { a.SignalRConnectionID })
                .WithUniqueIndex(a => new { a.SignalRConnectionID, a.User, a.StartTime, a.TargetEntity })
                .WithDelete(ConcurrentUserOperation.Delete)
                .WithQuery(() => e => new
                {
                    Entity = e,
                    e.Id,
                    e.TargetEntity,
                    e.IsModified,
                    e.User,
                    e.StartTime,
                    e.SignalRConnectionID,
                });
        }
    }
}
