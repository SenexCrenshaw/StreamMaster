namespace StreamMaster.Infrastructure.Common;

public static class MediatorExtensions
{
    public static bool InheritsFrom(this Type t1, Type t2)
    {
        return t1 != null && t2 != null
&& ((t1.BaseType?.IsGenericType == true &&
            t1.BaseType.GetGenericTypeDefinition() == t2)
|| (t1.BaseType?.InheritsFrom(t2) == true)
|| (t2.IsAssignableFrom(t1) && t1 != t2)
            ||
            t1.GetInterfaces().Any(x =>
              x.IsGenericType &&
              x.GetGenericTypeDefinition() == t2));
    }

    public static bool IsCancelled(this CancellationToken? token)
    {
        return token is not null && ((CancellationToken)token).IsCancellationRequested;
    }
}
