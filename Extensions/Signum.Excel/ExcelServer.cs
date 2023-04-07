using Microsoft.AspNetCore.Builder;
using Signum.API;
using Signum.Excel;

namespace Signum.React.Excel;

public static class ExcelServer
{
    public static void Start(IApplicationBuilder app)
    {
        SignumControllerFactory.RegisterArea(MethodInfo.GetCurrentMethod());

        ReflectionServer.RegisterLike(typeof(ExcelMessage), () => ExcelPermission.PlainExcel.IsAuthorized());
    }
}