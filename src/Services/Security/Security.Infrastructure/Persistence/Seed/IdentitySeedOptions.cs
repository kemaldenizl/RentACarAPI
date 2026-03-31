namespace Security.Infrastructure.Persistence.Seed;

public sealed class IdentitySeedOptions
{
    public const string SectionName = "IdentitySeed";

    public bool Enabled { get; init; } = true;
    public string AdminEmail { get; init; } = "admin@local";
    public string AdminPassword { get; init; } = "ChangeMe123!";
}