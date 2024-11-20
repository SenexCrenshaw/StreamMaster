namespace StreamMaster.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public string[] Errors { get; set; }
    public bool Succeeded { get; set; }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }

    public static Result Success()
    {
        return new Result(true, []);
    }
}