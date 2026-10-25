using FibiEmlakDanismanlik.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FibiEmlakDanismanlik.Persistence.Tests;

/// <summary>
/// Testler için LocalDB'de geçici veritabanı oluşturur, test sonunda siler.
/// </summary>
public sealed class LocalDbFixture : IDisposable
{
    public string ConnectionString { get; } =
        $"Server=(localdb)\\MSSQLLocalDB;Database=FibiTests_{Guid.NewGuid():N};Integrated Security=True;TrustServerCertificate=True";

    public LocalDbFixture()
    {
        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public FibiEmlakDanismanlikContext CreateContext() =>
        new(new DbContextOptionsBuilder<FibiEmlakDanismanlikContext>().UseSqlServer(ConnectionString).Options);

    public void Dispose()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
    }
}

[CollectionDefinition("LocalDb")]
public sealed class LocalDbCollection : ICollectionFixture<LocalDbFixture>
{
}
