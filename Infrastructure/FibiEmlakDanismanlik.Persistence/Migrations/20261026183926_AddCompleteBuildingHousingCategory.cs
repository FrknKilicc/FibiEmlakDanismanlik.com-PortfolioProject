using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompleteBuildingHousingCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Komple bina satışı/kiralaması konut ilanı olarak girilebilsin
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT 1 FROM dbo.HousingCategories WHERE HousingCategoryName = N'Komple Bina')
    INSERT INTO dbo.HousingCategories (HousingCategoryName) VALUES (N'Komple Bina');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Kullanan ilan varsa silinmez
            migrationBuilder.Sql(@"DELETE c FROM dbo.HousingCategories c
WHERE c.HousingCategoryName = N'Komple Bina'
  AND NOT EXISTS (SELECT 1 FROM dbo.forSaleHousingPropertyListings h WHERE h.HousingCategoryId = c.HousingCategoryId)
  AND NOT EXISTS (SELECT 1 FROM dbo.rentalHousingListings r WHERE r.HousingCategoryId = c.HousingCategoryId);");
        }
    }
}
