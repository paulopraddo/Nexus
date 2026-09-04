using Nexus.Domain.Common;
using FluentResults;

namespace Nexus.Domain.Cards;

public sealed class CardTitle : ValueObject
{
    public const int MinLength = 1;
    public const int MaxLength = 200;

    public string Value { get; }

    private CardTitle(string value)
    {
        Value = value;
    }

    public static Result<CardTitle> Create(string value)
    {
        value = value?.Trim() ?? string.Empty;

        if (value.Length is < MinLength or > MaxLength)
        {
            return Result.Fail($"O título do card deve ter entre {MinLength} e {MaxLength} caracteres.");
        }

        return Result.Ok(new CardTitle(value));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
