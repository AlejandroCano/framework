using Signum.Authorization.Rules;
using System.Globalization;

namespace Signum.Authorization;

[EntityKind(EntityKind.Main, EntityData.Transactional)]
public class UserEntity : Entity, IEmailOwnerEntity, IUserEntity
{
    public static Func<string, string?> ValidatePassword = p =>
    {
        if (p.Length >= 5)
            return null;

        return LoginAuthMessage.ThePasswordMustHaveAtLeast0Characters.NiceToString(5);
    };

    public static string? OnValidatePassword(string password)
    {
        if (ValidatePassword != null)
            return ValidatePassword(password);

        return null;
    }

    [UniqueIndex(AvoidAttachToUniqueIndexes = true)]
    [StringLengthValidator(Min = 2, Max = 100)]
    public string UserName { get; set; }

    [DbType(Size = 128), QueryableProperty(false)]
    public byte[]? PasswordHash { get; set; }

    public Lite<RoleEntity> Role { get; set; }

    [StringLengthValidator(Max = 200), EMailValidator]
    public string? Email { get; set; }

    public CultureInfoEntity? CultureInfo { get; set; }

    public DateTime? DisabledOn { get; set; }

    public UserState State { get; set; } = UserState.New;

    protected override string? PropertyValidation(PropertyInfo pi)
    {
        if (pi.Name == nameof(State))
        {
            if (DisabledOn != null && State != UserState.Deactivated)
                return AuthAdminMessage.TheUserStateMustBeDisabled.NiceToString();
        }

        return base.PropertyValidation(pi);
    }

    public int LoginFailedCounter { get; set; }

    public static Lite<UserEntity> Current => (Lite<UserEntity>)UserHolder.Current?.User!;

    public static CultureInfo? CurrentUserCulture
    {
        get
        {
            var culture = UserHolder.Current?.GetClaim("Culture") as string;
            return culture == null ? null : System.Globalization.CultureInfo.GetCultureInfo(culture);
        }
    }

    [AutoExpressionField]
    public EmailOwnerData EmailOwnerData => As.Expression(() => new EmailOwnerData
    {
        Owner = this.ToLite(),
        CultureInfo = CultureInfo,
        DisplayName = UserName,
        Email = Email,
        AzureUserId = null,
    });

    [AutoExpressionField]
    public override string ToString() => As.Expression(() => UserName);
}


public enum UserState
{
    [Ignore]
    New = -1,
    Active,
    Deactivated,
}

[AutoInit]
public static class UserOperation
{
    public static ConstructSymbol<UserEntity>.Simple Create;
    public static ExecuteSymbol<UserEntity> Save;
    public static ExecuteSymbol<UserEntity> Reactivate;
    public static ExecuteSymbol<UserEntity> Deactivate;
    public static ExecuteSymbol<UserEntity> SetPassword;
    public static DeleteSymbol<UserEntity> Delete;
}

public class IncorrectUsernameException : ApplicationException
{
    public IncorrectUsernameException() { }
    public IncorrectUsernameException(string message) : base(message) { }
    protected IncorrectUsernameException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    { }
}

public class UserLockedException : ApplicationException
{
    public UserLockedException() { }
    public UserLockedException(string message) : base(message) { }
    protected UserLockedException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    { }
}

public class IncorrectPasswordException : ApplicationException
{
    public IncorrectPasswordException() { }
    public IncorrectPasswordException(string message) : base(message) { }
    protected IncorrectPasswordException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    { }
}




[AutoInit]
public static class UserTypeCondition
{
    public static readonly TypeConditionSymbol DeactivatedUsers;
}


[AllowUnathenticated]
public class UserLiteModel : ModelEntity
{
    public string UserName { get; set; }

    public string? ToStringValue { get; set; }

    public Guid? OID { get; set; }

    public string? SID { get; set; }

    [AutoExpressionField]
    public override string ToString() => As.Expression(() => ToStringValue ?? UserName);
}
