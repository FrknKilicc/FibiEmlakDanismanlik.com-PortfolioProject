using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FibiEmlakDanismanlik.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdminSecurityFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanViewAllListings",
                table: "Agents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginCount",
                table: "Agents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Agents",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAtUtc",
                table: "Agents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEndUtc",
                table: "Agents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Agents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordChangedAtUtc",
                table: "Agents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Agents",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Agents",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValueSql: "CONVERT(nvarchar(64), NEWID())");

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnWebsite",
                table: "Agents",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Agents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorSecretProtected",
                table: "Agents",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgentRecoveryCodes",
                columns: table => new
                {
                    AgentRecoveryCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentRecoveryCodes", x => x.AgentRecoveryCodeId);
                    table.ForeignKey(
                        name: "FK_AgentRecoveryCodes_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChangesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "ListingAccessGrants",
                columns: table => new
                {
                    ListingAccessGrantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    ListingId = table.Column<int>(type: "int", nullable: false),
                    ListingTypeId = table.Column<int>(type: "int", nullable: false),
                    Access = table.Column<int>(type: "int", nullable: false),
                    GrantedByAgentId = table.Column<int>(type: "int", nullable: false),
                    GrantedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingAccessGrants", x => x.ListingAccessGrantId);
                    table.ForeignKey(
                        name: "FK_ListingAccessGrants_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginAuditLogs",
                columns: table => new
                {
                    LoginAuditLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttemptedMail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAuditLogs", x => x.LoginAuditLogId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentRecoveryCodes_AgentId",
                table: "AgentRecoveryCodes",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_AgentId",
                table: "AuditLogs",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAtUtc",
                table: "AuditLogs",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_EntityKey",
                table: "AuditLogs",
                columns: new[] { "EntityName", "EntityKey" });

            migrationBuilder.CreateIndex(
                name: "IX_ListingAccessGrants_AgentId_ListingTypeId_ListingId",
                table: "ListingAccessGrants",
                columns: new[] { "AgentId", "ListingTypeId", "ListingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListingAccessGrants_ListingTypeId_ListingId",
                table: "ListingAccessGrants",
                columns: new[] { "ListingTypeId", "ListingId" });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAuditLogs_AgentId",
                table: "LoginAuditLogs",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAuditLogs_CreatedAtUtc",
                table: "LoginAuditLogs",
                column: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentRecoveryCodes");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ListingAccessGrants");

            migrationBuilder.DropTable(
                name: "LoginAuditLogs");

            migrationBuilder.DropColumn(
                name: "CanViewAllListings",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "FailedLoginCount",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "LastLoginAtUtc",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "LockoutEndUtc",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "PasswordChangedAtUtc",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "ShowOnWebsite",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "TwoFactorSecretProtected",
                table: "Agents");
        }
    }
}
