using Microsoft.AspNetCore.Builder;
using Signum.API;
using Signum.Authorization;
using Signum.Dynamic.Views;
using Signum.Eval.TypeHelp;

namespace Signum.React.Dynamic;

public static class DynamicServer
{
    public static void Start(IApplicationBuilder app)
    {
        TypeHelpServer.Start(app);
        SignumControllerFactory.RegisterArea(MethodInfo.GetCurrentMethod());
        ReflectionServer.RegisterLike(typeof(DynamicViewMessage), () => UserEntity.Current != null);

        SignumServer.WebEntityJsonConverterFactory.AfterDeserilization.Register((PropertyRouteEntity wc) =>
        {
            var route = PropertyRouteLogic.TryGetPropertyRouteEntity(wc.RootType, wc.Path);
            if (route != null)
            {
                wc.SetId(route.Id);
                wc.SetIsNew(false);
                wc.SetCleanModified(false);
            }
        });
    }
}