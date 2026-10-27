using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LeadTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedAgentId",
                table: "FibiServiceLeads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadStatus",
                table: "FibiServiceLeads",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusUpdatedAtUtc",
                table: "FibiServiceLeads",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedAgentId",
                table: "customerContacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadStatus",
                table: "customerContacts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusUpdatedAtUtc",
                table: "customerContacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LeadActivities",
                columns: table => new
                {
                    LeadActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source = table.Column<int>(type: "int", nullable: false),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OldStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: true),
                    AssignedAgentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadActivities", x => x.LeadActivityId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FibiServiceLeads_AssignedAgentId",
                table: "FibiServiceLeads",
                column: "AssignedAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_FibiServiceLeads_LeadStatus",
                table: "FibiServiceLeads",
                column: "LeadStatus");

            migrationBuilder.CreateIndex(
                name: "IX_customerContacts_AssignedAgentId",
                table: "customerContacts",
                column: "AssignedAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_customerContacts_LeadStatus",
                table: "customerContacts",
                column: "LeadStatus");

            migrationBuilder.CreateIndex(
                name: "IX_LeadActivities_Source_LeadId_CreatedAtUtc",
                table: "LeadActivities",
                columns: new[] { "Source", "LeadId", "CreatedAtUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_customerContacts_Agents_AssignedAgentId",
                table: "customerContacts",
                column: "AssignedAgentId",
                principalTable: "Agents",
                principalColumn: "AgentId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FibiServiceLeads_Agents_AssignedAgentId",
                table: "FibiServiceLeads",
                column: "AssignedAgentId",
                principalTable: "Agents",
                principalColumn: "AgentId",
                onDelete: ReferentialAction.SetNull);

            // Mevcut randevu talepleri ilan danışmanına atanır; aranmış FİBİ talepleri 'Arandı' olur
            migrationBuilder.Sql(@"
UPDATE c SET c.AssignedAgentId = l.AgentId FROM [customerContacts] c INNER JOIN [forSaleLandListings] l ON l.[ForSaleLandListingId] = c.PropertyListingId WHERE c.PropertyListingTypeEnum = 1 AND c.AssignedAgentId IS NULL AND EXISTS (SELECT 1 FROM [Agents] a WHERE a.AgentId = l.AgentId);
UPDATE c SET c.AssignedAgentId = l.AgentId FROM [customerContacts] c INNER JOIN [forSaleHousingPropertyListings] l ON l.[ForSaleHousingListId] = c.PropertyListingId WHERE c.PropertyListingTypeEnum = 2 AND c.AssignedAgentId IS NULL AND EXISTS (SELECT 1 FROM [Agents] a WHERE a.AgentId = l.AgentId);
UPDATE c SET c.AssignedAgentId = l.AgentId FROM [customerContacts] c INNER JOIN [forSaleCommercialPropertyListings] l ON l.[ForSaleCommercialListingId] = c.PropertyListingId WHERE c.PropertyListingTypeEnum = 3 AND c.AssignedAgentId IS NULL AND EXISTS (SELECT 1 FROM [Agents] a WHERE a.AgentId = l.AgentId);
UPDATE c SET c.AssignedAgentId = l.AgentId FROM [customerContacts] c INNER JOIN [rentalLandListings] l ON l.[RentalLandListingId] = c.PropertyListingId WHERE c.PropertyListingTypeEnum = 7 AND c.AssignedAgentId IS NULL AND EXISTS (SELECT 1 FROM [Agents] a WHERE a.AgentId = l.AgentId);
UPDATE c SET c.AssignedAgentId = l.AgentId FROM [customerContacts] c INNER JOIN [rentalHousingListings] l ON l.[RentalHousingListId] = c.PropertyListingId WHERE c.PropertyListingTypeEnum = 8 AND c.AssignedAgentId IS NULL AND EXISTS (SELECT 1 FROM [Agents] a WHERE a.AgentId = l.AgentId);
UPDATE c SET c.AssignedAgentId = l.AgentId FROM [customerContacts] c INNER JOIN [rentalCommercialPropertyListings] l ON l.[RentalCommercialListId] = c.PropertyListingId WHERE c.PropertyListingTypeEnum = 9 AND c.AssignedAgentId IS NULL AND EXISTS (SELECT 1 FROM [Agents] a WHERE a.AgentId = l.AgentId);

UPDATE [FibiServiceLeads] SET [LeadStatus] = 2, [StatusUpdatedAtUtc] = SYSUTCDATETIME() WHERE [IsContacted] = 1;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customerContacts_Agents_AssignedAgentId",
                table: "customerContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_FibiServiceLeads_Agents_AssignedAgentId",
                table: "FibiServiceLeads");

            migrationBuilder.DropTable(
                name: "LeadActivities");

            migrationBuilder.DropIndex(
                name: "IX_FibiServiceLeads_AssignedAgentId",
                table: "FibiServiceLeads");

            migrationBuilder.DropIndex(
                name: "IX_FibiServiceLeads_LeadStatus",
                table: "FibiServiceLeads");

            migrationBuilder.DropIndex(
                name: "IX_customerContacts_AssignedAgentId",
                table: "customerContacts");

            migrationBuilder.DropIndex(
                name: "IX_customerContacts_LeadStatus",
                table: "customerContacts");

            migrationBuilder.DropColumn(
                name: "AssignedAgentId",
                table: "FibiServiceLeads");

            migrationBuilder.DropColumn(
                name: "LeadStatus",
                table: "FibiServiceLeads");

            migrationBuilder.DropColumn(
                name: "StatusUpdatedAtUtc",
                table: "FibiServiceLeads");

            migrationBuilder.DropColumn(
                name: "AssignedAgentId",
                table: "customerContacts");

            migrationBuilder.DropColumn(
                name: "LeadStatus",
                table: "customerContacts");

            migrationBuilder.DropColumn(
                name: "StatusUpdatedAtUtc",
                table: "customerContacts");
        }
    }
}