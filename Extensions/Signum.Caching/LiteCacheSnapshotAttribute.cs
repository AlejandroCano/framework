namespace Signum.Cache;

/// <summary>Keeps the Lite model until its containing cache is reset. Does not prevent deletion of the referenced entity.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class LiteCacheSnapshotAttribute : Attribute
{
    public Type? ForEntityType { get; set; }

    internal static bool IsEnabled(SchemaSettings settings, PropertyRoute? route, Type type)
    {
        if (route == null)
            return false;

        var attributes = settings.FieldAttributes(route)?.OfType<LiteCacheSnapshotAttribute>().ToList();
        if (attributes == null || attributes.Count == 0)
            return false;

        if (!route.Type.IsLite())
            throw new InvalidOperationException($"{nameof(LiteCacheSnapshotAttribute)} requires a Lite reference: {route}");

        if (settings.FieldAttribute<ImplementedByAllAttribute>(route) != null)
            throw new NotSupportedException($"Snapshot references do not support ImplementedByAll: {route}");

        foreach (var attribute in attributes)
            if (attribute.ForEntityType != null && !route.Type.CleanType().IsAssignableFrom(attribute.ForEntityType))
                throw new InvalidOperationException($"Invalid snapshot implementation {attribute.ForEntityType} for {route}");

        return attributes.Any(a => a.ForEntityType == null || a.ForEntityType == type);
    }
}
