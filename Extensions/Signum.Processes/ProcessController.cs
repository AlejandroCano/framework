using Microsoft.AspNetCore.Mvc;
using Signum.API;
using Signum.API.Controllers;
using Signum.API.Filters;
using System.ComponentModel.DataAnnotations;

namespace Signum.Processes;

[ValidateModelFilter]
public class ProcessController : ControllerBase
{
    [HttpPost("api/processes/constructFromMany")]
    public EntityPackTS ConstructFromMany([Required, FromBody]OperationController.MultiOperationRequest request)
    {
        var type = request.Type == null ? null : TypeLogic.GetType(request.Type);

        var op = request.GetOperationSymbol(type!);
        var entity = PackageLogic.CreatePackageOperation(request.Lites, op, request.ParseArgs(op));

        return SignumServer.GetEntityPack(entity);
    }

    [HttpGet("api/processes/view")]
    public ProcessLogicState View()
    {
        ProcessLogicState state = ProcessRunner.ExecutionState();

        return state;
    }

    [HttpPost("api/processes/start")]
    public void Start()
    {
        ProcessPermission.ViewProcessPanel.AssertAuthorized();

        ProcessRunner.StartRunningProcesses();

        Thread.Sleep(1000);
    }

    [HttpPost("api/processes/stop")]
    public void Stop()
    {
        ProcessPermission.ViewProcessPanel.AssertAuthorized();

        ProcessRunner.Stop();

        Thread.Sleep(1000);
    }

    [HttpGet("api/processes/simpleStatus"), SignumAllowAnonymous]
    public SimpleStatus SimpleStatus()
    {
        return ProcessRunner.GetSimpleStatus();
    }
}
