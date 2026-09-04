using Nexus.Domain.Common;
using FluentResults;

namespace Nexus.Domain.Workspaces;

public sealed class WorkspaceName : ValueObject
{
    public const int MinLength = 1;
    public const int MaxLength = 100;

    public string Value { get; }

    private WorkspaceName(string value)
    {
        Value = value;
    }

    public static Result<WorkspaceName> Create(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (value.Length is < MinLength or > MaxLength)
        {
            return Result.Fail($"O nome do workspace deve ter entre {MinLength} e {MaxLength} caracteres.");
        }

        return Result.Ok(new WorkspaceName(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
