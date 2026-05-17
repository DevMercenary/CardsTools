namespace CardsTools.Data.Tools;

public static class ValidationCard
{
    public static bool IsBlank(string? input) => string.IsNullOrWhiteSpace(input);

    public static bool TryParseInt(string? input, out int value) =>
        int.TryParse(input, out value);
}
