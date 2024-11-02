namespace DataAccess.EF;

public class PostgresqlFunctions
{
    [DbFunction("tr_lower", "public")]
    public static string TrLower(string? input) =>
        throw new InvalidOperationException();

    [DbFunction("tr_upper", "public")]
    public static string TrUpper(string? input) =>
        throw new InvalidOperationException();
}