using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNearbyCategoryOther : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM NearbyCategories WHERE [Key] = N'other')
    INSERT INTO NearbyCategories ([Key], Name, IconCss, SortOrder, IsActive)
    VALUES (N'other', N'Diğer', N'fas fa-map-marker-alt', 9, 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE c FROM NearbyCategories c
WHERE c.[Key] = N'other'
  AND NOT EXISTS (SELECT 1 FROM ListingNearbyPlaces p WHERE p.NearbyCategoryId = c.Id);");
        }
    }
}
