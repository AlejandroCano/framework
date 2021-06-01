using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Signum.Engine;
using Signum.Engine.Basics;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Mailing;
using Signum.Engine.Maps;
using Signum.Engine.Operations;
using Signum.Entities;
using Signum.Entities.Authorization;
using Signum.Entities.DynamicQuery;
using Signum.Entities.Mailing;
using Signum.Utilities;
using Signum.Utilities.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Signum.Engine.Authorization
{
    public static class MicrosoftGraphLogic
    {
        public static Func<ClientCredentialProvider> GetClientCredentialProvider = () => ((ActiveDirectoryAuthorizer)AuthLogic.Authorizer!).GetConfig().GetAuthProvider();

        public static async Task<List<ActiveDirectoryUser>> FindActiveDirectoryUsers(string subStr, int top, CancellationToken token)
        {
            ClientCredentialProvider authProvider = GetClientCredentialProvider();
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);

            subStr = subStr.Replace("'", "''");

            var query = subStr.Contains("@") ? $"mail eq '{subStr}'" :
                subStr.Contains(",") ? $"startswith(givenName, '{subStr.After(",").Trim()}') AND startswith(surname, '{subStr.Before(",").Trim()}') OR startswith(displayname, '{subStr.Trim()}')" :
                subStr.Contains(" ") ? $"startswith(givenName, '{subStr.Before(" ").Trim()}') AND startswith(surname, '{subStr.After(" ").Trim()}') OR startswith(displayname, '{subStr.Trim()}')" :
                 $"startswith(givenName, '{subStr}') OR startswith(surname, '{subStr}') OR startswith(displayname, '{subStr.Trim()}') OR startswith(mail, '{subStr.Trim()}')";

            var result = await graphClient.Users.Request().Filter(query).Top(top).GetAsync(token);

            return result.Select(a => new ActiveDirectoryUser
            {
                UPN = a.UserPrincipalName,
                DisplayName = a.DisplayName,
                JobTitle = a.JobTitle,
                ObjectID = Guid.Parse(a.Id),
            }).ToList();
        }

        public static void Start(SchemaBuilder sb)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                QueryLogic.Queries.Register(UserADQuery.ActiveDirectoryUsers, () => DynamicQueryCore.Manual(async (request, queryDescription, cancellationToken) =>
                 {
                     ClientCredentialProvider authProvider = GetClientCredentialProvider();
                     GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                     var query = graphClient.Users.Request()
                        .Filter(request.Filters, queryDescription)
                        .Select(request.Columns, queryDescription)
                        .OrderBy(request.Orders, queryDescription)
                        .Paginate(request.Pagination);

                     var result = await query.GetAsync(cancellationToken);

                     return result.Select(u => new
                     {
                         Entity = (Lite<Entities.Entity>?)null,
                         u.Id,
                         u.DisplayName,
                         u.Mail,
                         u.GivenName,
                         u.Surname,
                         u.JobTitle,
                         LocationCode = u.OnPremisesExtensionAttributes?.ExtensionAttribute8,
                         u.CompanyName,
                     }).ToDEnumerableCount(queryDescription, null);
                 }).Column(a => a.Entity, c => c.Implementations = Implementations.By()), Implementations.By());
            }
        }


        static Dictionary<string, string> replace = new Dictionary<string, string>
        {
            { "LocationCode", "onPremisesExtensionAttributes/extensionAttribute8"}
        };

        static string ToGraphField(QueryToken token, bool simplify = false)
        {
            var field = replace.TryGetC(token.Key) ?? token.Key.FirstLower();

            if (simplify)
                return field.TryBefore("/") ?? field;

            return field;
        }


        private static string ToStringValue(object? value)
        {
            return value is string str ? $"'{str}'" :
                value is Utilities.Date date ? $"'{date.ToIsoString()}'" :
                value is DateTime dt ? $"'{dt.ToIsoString()}'" :
                value is DateTimeOffset dto ? $"'{dto.DateTime.ToIsoString()}'" :
                value is Guid guid ? $"'{guid.ToString()}'" :
                value?.ToString() ?? "";
        }

        static IGraphServiceUsersCollectionRequest Filter(this IGraphServiceUsersCollectionRequest users, List<Filter> filters, QueryDescription queryDescription)
        {
            string ToFilter(Filter f)
            {
                if (f is FilterCondition fc)
                {
                    return fc.Operation switch
                    {
                        FilterOperation.EqualTo => ToGraphField(fc.Token) + " eq " + ToStringValue(fc.Value),
                        FilterOperation.DistinctTo => ToGraphField(fc.Token) + " ne " + ToStringValue(fc.Value),
                        FilterOperation.GreaterThan => ToGraphField(fc.Token) + " gt " + ToStringValue(fc.Value),
                        FilterOperation.GreaterThanOrEqual => ToGraphField(fc.Token) + " ge " + ToStringValue(fc.Value),
                        FilterOperation.LessThan => ToGraphField(fc.Token) + " lt " + ToStringValue(fc.Value),
                        FilterOperation.LessThanOrEqual => ToGraphField(fc.Token) + " le " + ToStringValue(fc.Value),
                        FilterOperation.StartsWith => "startswith(" + ToGraphField(fc.Token) + "," + ToStringValue(fc.Value) + ")",
                        FilterOperation.EndsWith => "endswith(" + ToGraphField(fc.Token) + "," + ToStringValue(fc.Value) + ")",
                        FilterOperation.NotStartsWith => "not startswith(" + ToGraphField(fc.Token) + "," + ToStringValue(fc.Value) + ")",
                        FilterOperation.NotEndsWith => "not endswith(" + ToGraphField(fc.Token) + "," + ToStringValue(fc.Value) + ")",
                        FilterOperation.IsIn => "(" + ((object[])fc.Value!).ToString(a => ToGraphField(fc.Token) + " eq " + ToStringValue(a), " OR ") + ")",
                        FilterOperation.IsNotIn => "not (" + ((object[])fc.Value!).ToString(a => ToGraphField(fc.Token) + " eq " + ToStringValue(a), " OR ") + ")",
                        FilterOperation.Contains or
                        FilterOperation.Like or
                        FilterOperation.NotContains or
                        FilterOperation.NotLike or 
                        _ => throw new InvalidOperationException(fc.Operation + " is not implemented in Microsoft Graph API")
                    };
                }
                else if (f is FilterGroup fg)
                {
                    if (fg.GroupOperation == FilterGroupOperation.Or)
                        return "(" + fg.Filters.Select(f2 => ToFilter(f2)).ToString(" OR ") + ")";
                    else
                        return fg.Filters.Select(f2 => ToFilter(f2)).ToString(" AND ");
                }
                else
                    throw new UnexpectedValueException(f);
            }

            var filterStr = filters.Select(f => ToFilter(f)).ToString(" AND ");
            if (filterStr.HasText())
                return users.Filter(filterStr);
            
            return users;
        }

        static IGraphServiceUsersCollectionRequest Select(this IGraphServiceUsersCollectionRequest users, List<Column> columns, QueryDescription queryDescription)
        {
            var selectStr = columns.Select(c => ToGraphField(c.Token, simplify: true)).Distinct().ToString(",");
            return users.Select(selectStr);
        }

        static IGraphServiceUsersCollectionRequest OrderBy(this IGraphServiceUsersCollectionRequest users, List<Order> orders, QueryDescription queryDescription)
        {
            var orderStr = orders.Select(c => ToGraphField(c.Token) + " " + (c.OrderType == OrderType.Ascending ? "asc" : "desc")).ToString(",");
            //if (orderStr.HasText())
            //    return users.OrderBy(orderStr);

            return users;
        }

        static IGraphServiceUsersCollectionRequest Paginate(this IGraphServiceUsersCollectionRequest users, Pagination pagination)
        {
            return pagination switch
            {
                Pagination.All => users,
                Pagination.Firsts f => users.Top(f.TopElements),
                Pagination.Paginate p => users.Skip(p.StartElementIndex()).Top(p.ElementsPerPage),
                _ => throw new UnexpectedValueException(pagination)
            };
        }

        public static UserEntity CreateUserFromAD(ActiveDirectoryUser adUser)
        {
            var adAuthorizer = (ActiveDirectoryAuthorizer)AuthLogic.Authorizer!;
            var config = adAuthorizer.GetConfig();
            
            var acuCtx = GetMicrosoftGraphContext(adUser);

            using (ExecutionMode.Global())
            {
                var user = Database.Query<UserEntity>().SingleOrDefaultEx(a => a.Mixin<UserADMixin>().OID == acuCtx.OID);
                if (user == null)
                {
                    user = Database.Query<UserEntity>().SingleOrDefault(a => a.UserName == acuCtx.UserName) ??
                           (acuCtx.UserName.Contains("@") && config.AllowMatchUsersBySimpleUserName ? Database.Query<UserEntity>().SingleOrDefault(a => a.Email == acuCtx.UserName || a.UserName == acuCtx.UserName.Before("@")) : null);
                }

                if (user != null)
                {
                    adAuthorizer.UpdateUser(user, acuCtx);

                    return user;
                }
            }

            var result = adAuthorizer.OnAutoCreateUser(acuCtx);

            return result ?? throw new InvalidOperationException(ReflectionTools.GetPropertyInfo((ActiveDirectoryConfigurationEmbedded e) => e.AutoCreateUsers).NiceName() + " is not activated");
        }

        private static MicrosoftGraphCreateUserContext GetMicrosoftGraphContext(ActiveDirectoryUser adUser)
        {
            ClientCredentialProvider authProvider = GetClientCredentialProvider();
            GraphServiceClient graphClient = new GraphServiceClient(authProvider);
            var msGraphUser = graphClient.Users[adUser.ObjectID.ToString()].Request().GetAsync().Result;

            return new MicrosoftGraphCreateUserContext(msGraphUser);
        }
    }

    public class MicrosoftGraphCreateUserContext : IAutoCreateUserContext
    {
        public MicrosoftGraphCreateUserContext(User user)
        {
            User = user;
        }

        public User User { get; set; }

        public string UserName => User.UserPrincipalName;
        public string? EmailAddress => User.UserPrincipalName;

        public string FirstName => User.GivenName;
        public string LastName => User.Surname;

        public Guid? OID => Guid.Parse(User.Id);

        public string? SID => null;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class ActiveDirectoryUser
    {
        public string DisplayName;
        public string UPN;
        public Guid ObjectID;

        public string JobTitle;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
