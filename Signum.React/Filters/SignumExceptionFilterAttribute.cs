using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Signum.Engine;
using Signum.Engine.Basics;
using Signum.Entities;
using Signum.Entities.Basics;
using Signum.Entities.Reflection;
using Signum.React.Facades;
using Signum.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Signum.React.Filters
{
    public class SignumExceptionFilterAttribute : IAsyncResourceFilter
    {
        public static Func<Exception, bool> TranslateExceptionMessage = ex => ex is ApplicationException;

        public static Func<Exception, bool> IncludeErrorDetails = ex => true;

        public static readonly List<Type> IgnoreExceptions = new List<Type> { typeof(OperationCanceledException) };


        public async Task OnResourceExecutionAsync(ResourceExecutingContext precontext, ResourceExecutionDelegate next)
        {
            var context = await next();

            if (context.Exception != null)
            {
                if (!IgnoreExceptions.Contains(context.Exception.GetType()))
                {
                    var statusCode = GetStatus(context.Exception.GetType());

                    var req = context.HttpContext.Request;

                    var connFeature = context.HttpContext.Features.Get<IHttpConnectionFeature>();

                    var exLog = context.Exception.LogException(e =>
                    {
                        e.ActionName = Try(100, ()=>(context.ActionDescriptor as ControllerActionDescriptor)?.ActionName);
                        e.ControllerName = Try(100, () => (context.ActionDescriptor as ControllerActionDescriptor)?.ControllerName);
                        e.UserAgent = Try(300, () => req.Headers["User-Agent"].FirstOrDefault());
                        e.RequestUrl = Try(int.MaxValue, () => req.GetDisplayUrl());
                        e.UrlReferer = Try(int.MaxValue, () => req.Headers["Referer"].ToString());
                        e.UserHostAddress = Try(100, () => connFeature.RemoteIpAddress.ToString());
                        e.UserHostName = Try(100, () => Dns.GetHostEntry(connFeature.RemoteIpAddress).HostName);
                        e.User = (UserHolder.Current ?? (IUserEntity)context.HttpContext.Items[SignumAuthenticationFilter.Signum_User_Key])?.ToLite() ?? e.User;
                        e.QueryString = Try(int.MaxValue, () => req.QueryString.ToString());
                        e.Form = Try(int.MaxValue, () => ReadAllBody(context.HttpContext));
                        e.Session = null;
                    });
                    
                    if (ExpectsJsonResult(context))
                    {

                        var ci = TranslateExceptionMessage(context.Exception) ? SignumCultureSelectorFilter.GetCurrentCulture?.Invoke(precontext) : null;

                        using (ci == null ? null : CultureInfoUtils.ChangeBothCultures(ci))
                        {
                            var error = new HttpError(context.Exception, IncludeErrorDetails(context.Exception));

                            var response = context.HttpContext.Response;
                            response.StatusCode = (int)statusCode;
                            response.ContentType = "application/json";
                            await response.WriteAsync(JsonConvert.SerializeObject(error, SignumServer.JsonSerializerSettings));
                            context.ExceptionHandled = true;
                        }
                    }
                }
            }
        }

        private string? Try(int size, Func<string?> getValue)
        {
            try
            {
                return getValue()?.TryStart(size);
            }
            catch(Exception e)
            {
                return (e.GetType().Name + ":"  + e.Message).TryStart(size);
            }
        }

        public static Func<ResourceExecutedContext, bool> ExpectsJsonResult = (context) =>
        {
            if (context.ActionDescriptor is ControllerActionDescriptor cad)
            {
                return !typeof(IActionResult).IsAssignableFrom(cad.MethodInfo.ReturnType) ||
                    typeof(FileResult).IsAssignableFrom(cad.MethodInfo.ReturnType) && context.HttpContext.Request.Method != "GET";

            }
            return false;
        };

        public string ReadAllBody(HttpContext httpContext)
        {
            httpContext.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
            return Encoding.UTF8.GetString(httpContext.Request.Body.ReadAllBytes());
        }

        private HttpStatusCode GetStatus(Type type)
        {
            if (type == typeof(UnauthorizedAccessException))
                return HttpStatusCode.Forbidden;

            if (type == typeof(AuthenticationException))
                return HttpStatusCode.Forbidden; // Unauthorized produces Login Password dialog in Mixed mode

            if (type == typeof(EntityNotFoundException))
                return HttpStatusCode.NotFound;

            if (type == typeof(IntegrityCheckException))
                return HttpStatusCode.BadRequest;

            return HttpStatusCode.InternalServerError;
        }


    }

    public class HttpError
    {
        public HttpError(Exception e, bool includeErrorDetails = true)
        {
            this.ExceptionMessage = e.Message;
            this.ExceptionType = e.GetType().FullName!;
            if (includeErrorDetails)
            {
                this.ExceptionId = e.GetExceptionEntity()?.Id.ToString();
                this.StackTrace = e.StackTrace;
                this.InnerException = e.InnerException == null ? null : new HttpError(e.InnerException);
            }
        }

        public string ExceptionType;
        public string ExceptionMessage;
        public string? ExceptionId;
        public string? StackTrace;
        public HttpError? InnerException;
    }
}