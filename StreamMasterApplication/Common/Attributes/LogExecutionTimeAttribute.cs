using AspectInjector.Broker;

using StreamMasterApplication.Common.Logging;

using System.Diagnostics;
using System.Reflection;

namespace StreamMasterApplication.Common.Attributes;


[Aspect(Scope.Global)]
[Injection(typeof(LogExecutionTimeAspect))]
public class LogExecutionTimeAspect : Attribute
{
    [Advice(Kind.Around, Targets = Target.Method)]
    public object HandleMethod(
    [Argument(Source.Name)] string name,
    [Argument(Source.Arguments)] object[] args,
    [Argument(Source.Target)] Func<object[], object> target,
    [Argument(Source.Metadata)] MethodBase method,
    [Argument(Source.ReturnType)] Type retType,
    [Argument(Source.Triggers)] Attribute[] triggers)
    {

        string abbreviatedNamespace = AbbreviateNamespace(method.DeclaringType.FullName);
        string nameToLog = $"{abbreviatedNamespace}.{name}";

        ILogger logger = GlobalLoggerProvider.CreateLogger(nameToLog);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        object result = target(args);

        if (result is Task task && !retType.IsValueType && retType != typeof(void))
        {
            task.ContinueWith(t =>
            {
                stopwatch.Stop();
                logger.LogInformation($"executed in {stopwatch.ElapsedMilliseconds} ms.");

                // Note: We're only logging in the continuation. 
                // We're not trying to modify the result or rethrow exceptions.
                // Any exception or result from the original task will flow through as-is.
            });

            return result;  // Return the original task
        }

        stopwatch.Stop();
        logger.LogInformation($"executed in {stopwatch.ElapsedMilliseconds} ms.");


        return result;
    }
    private static string AbbreviateNamespace(string fullNamespace)
    {
        string[] parts = fullNamespace.Split('.');
        if (parts.Length <= 3)
        {
            return fullNamespace; // If there are 3 or fewer parts, just return the original
        }

        // Only take the last three parts
        return string.Join('.', parts.Skip(parts.Length - 3));
    }


}
