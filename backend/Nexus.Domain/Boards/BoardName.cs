using Nexus.Domain.Common;
using FluentResults;

namespace Nexus.Domain.Boards;

public sealed class BoardName : ValueObject
{
    public const int MinLength = 1;
    public const int MaxLength = 100;

    public string Value { get; }

    private BoardName(string value)
    {
        Value = value;
    }

    public static Result<BoardName> Create(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (value.Length is < MinLength or > MaxLength)
        {
            return Result.Fail($"O nome do board deve ter entre {MinLength} e {MaxLength} caracteres.");
        }

        return Result.Ok(new BoardName(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
