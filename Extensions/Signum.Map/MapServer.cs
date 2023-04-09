using Microsoft.AspNetCore.Builder;
using Signum.API;

namespace Signum.Map;

public static class MapServer
{
    public static void Start(IApplicationBuilder app)
    {
        ReflectionServer.RegisterLike(typeof(MapMessage), () => MapPermission.ViewMap.IsAuthorized());
        SignumControllerFactory.RegisterArea(MethodInfo.GetCurrentMethod());

        MapColorProvider.GetColorProviders += () => new[]
        {
            new MapColorProvider
            {
                Name = "namespace",
                NiceName = MapMessage.Namespace.NiceToString(),
            },
        };

        MapColorProvider.GetColorProviders += () => new[]
        {
            new MapColorProvider
            {
                Name = "entityKind",
                NiceName = typeof(EntityKind).Name,
            }
        };

        MapColorProvider.GetColorProviders += () => new[]
        {
            new MapColorProvider
            {
                Name = "columns",
                NiceName = MapMessage.Columns.NiceToString(),
            }
        };

        MapColorProvider.GetColorProviders += () => new[]
        {
            new MapColorProvider
            {
                Name = "entityData",
                NiceName = typeof(EntityData).Name,
            }
        };

        MapColorProvider.GetColorProviders += () => new[]
        {
            new MapColorProvider
            {
                Name = "rows",
                NiceName = MapMessage.Rows.NiceToString(),
            }
        };

        if (Schema.Current.Tables.Any(a => a.Value.SystemVersioned != null))
        {
            MapColorProvider.GetColorProviders += () => new[]
            {
                new MapColorProvider
                {
                    Name = "rows_history",
                    NiceName = MapMessage.RowsHistory.NiceToString(),
                }
            };
        }

        MapColorProvider.GetColorProviders += () => new[]
        {
            new MapColorProvider
            {
                Name = "tableSize",
                NiceName = MapMessage.TableSize.NiceToString(),
            }
        };

        if(Schema.Current.Tables.Any(a => a.Value.SystemVersioned != null))
        {
            MapColorProvider.GetColorProviders += () => new[]
            {
                new MapColorProvider
                {
                    Name = "tableSize_history",
                    NiceName = MapMessage.TableSizeHistory.NiceToString(),
                }
            };
        }
    }
}
