using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedOurPartners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE OurPartners 
SET PartnerLogoUrl = N'/Realshed/assets/images/clients/eminevim_180x70.png' 
WHERE PartnerLogoUrl = N'realshed/assets/images/clients/eminevim_180x70.png';

IF NOT EXISTS (SELECT 1 FROM OurPartners WHERE PartnerName = N'sahibinden.com')
    INSERT INTO OurPartners (PartnerName, PartnerLogoUrl) VALUES (N'sahibinden.com', N'/Realshed/assets/images/clients/sahibinden_180x70.png');

IF NOT EXISTS (SELECT 1 FROM OurPartners WHERE PartnerName = N'Hepsiemlak')
    INSERT INTO OurPartners (PartnerName, PartnerLogoUrl) VALUES (N'Hepsiemlak', N'/Realshed/assets/images/clients/hepsiemlak_180x70.png');

IF NOT EXISTS (SELECT 1 FROM OurPartners WHERE PartnerName = N'Emlakjet')
    INSERT INTO OurPartners (PartnerName, PartnerLogoUrl) VALUES (N'Emlakjet', N'/Realshed/assets/images/clients/emlakjet_180x70.png');

IF NOT EXISTS (SELECT 1 FROM OurPartners WHERE PartnerName = N'Zingat')
    INSERT INTO OurPartners (PartnerName, PartnerLogoUrl) VALUES (N'Zingat', N'/Realshed/assets/images/clients/zingat_180x70.png');

IF NOT EXISTS (SELECT 1 FROM OurPartners WHERE PartnerName = N'Ziraat Bankası')
    INSERT INTO OurPartners (PartnerName, PartnerLogoUrl) VALUES (N'Ziraat Bankası', N'/Realshed/assets/images/clients/ziraatbankasi_180x70.png');
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM OurPartners WHERE PartnerName = N'sahibinden.com' AND PartnerLogoUrl = N'/Realshed/assets/images/clients/sahibinden_180x70.png';
DELETE FROM OurPartners WHERE PartnerName = N'Hepsiemlak' AND PartnerLogoUrl = N'/Realshed/assets/images/clients/hepsiemlak_180x70.png';
DELETE FROM OurPartners WHERE PartnerName = N'Emlakjet' AND PartnerLogoUrl = N'/Realshed/assets/images/clients/emlakjet_180x70.png';
DELETE FROM OurPartners WHERE PartnerName = N'Zingat' AND PartnerLogoUrl = N'/Realshed/assets/images/clients/zingat_180x70.png';
DELETE FROM OurPartners WHERE PartnerName = N'Ziraat Bankası' AND PartnerLogoUrl = N'/Realshed/assets/images/clients/ziraatbankasi_180x70.png';

UPDATE OurPartners 
SET PartnerLogoUrl = N'realshed/assets/images/clients/eminevim_180x70.png' 
WHERE PartnerLogoUrl = N'/Realshed/assets/images/clients/eminevim_180x70.png';
");
        }
    }
}
