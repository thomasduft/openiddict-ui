using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerWithCustomKey.Models.Data.UI
{
  public partial class InitialUI : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "OpenIddictApplications",
          columns: table => new
          {
            Id = table.Column<string>(type: "TEXT", nullable: false),
            ClientId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
            ClientSecret = table.Column<string>(type: "TEXT", nullable: true),
            ConcurrencyToken = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
            ConsentType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
            DisplayName = table.Column<string>(type: "TEXT", nullable: true),
            DisplayNames = table.Column<string>(type: "TEXT", nullable: true),
            Permissions = table.Column<string>(type: "TEXT", nullable: true),
            PostLogoutRedirectUris = table.Column<string>(type: "TEXT", nullable: true),
            Properties = table.Column<string>(type: "TEXT", nullable: true),
            RedirectUris = table.Column<string>(type: "TEXT", nullable: true),
            Requirements = table.Column<string>(type: "TEXT", nullable: true),
            Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_OpenIddictApplications", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "OpenIddictScopes",
          columns: table => new
          {
            Id = table.Column<string>(type: "TEXT", nullable: false),
            ConcurrencyToken = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
            Description = table.Column<string>(type: "TEXT", nullable: true),
            Descriptions = table.Column<string>(type: "TEXT", nullable: true),
            DisplayName = table.Column<string>(type: "TEXT", nullable: true),
            DisplayNames = table.Column<string>(type: "TEXT", nullable: true),
            Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
            Properties = table.Column<string>(type: "TEXT", nullable: true),
            Resources = table.Column<string>(type: "TEXT", nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_OpenIddictScopes", x => x.Id);
          });

      migrationBuilder.CreateTable(
          name: "OpenIddictAuthorizations",
          columns: table => new
          {
            Id = table.Column<string>(type: "TEXT", nullable: false),
            ApplicationId = table.Column<string>(type: "TEXT", nullable: true),
            ConcurrencyToken = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
            CreationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
            Properties = table.Column<string>(type: "TEXT", nullable: true),
            Scopes = table.Column<string>(type: "TEXT", nullable: true),
            Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
            Subject = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
            Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_OpenIddictAuthorizations", x => x.Id);
            table.ForeignKey(
                      name: "FK_OpenIddictAuthorizations_OpenIddictApplications_ApplicationId",
                      column: x => x.ApplicationId,
                      principalTable: "OpenIddictApplications",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateTable(
          name: "OpenIddictTokens",
          columns: table => new
          {
            Id = table.Column<string>(type: "TEXT", nullable: false),
            ApplicationId = table.Column<string>(type: "TEXT", nullable: true),
            AuthorizationId = table.Column<string>(type: "TEXT", nullable: true),
            ConcurrencyToken = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
            CreationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
            ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
            Payload = table.Column<string>(type: "TEXT", nullable: true),
            Properties = table.Column<string>(type: "TEXT", nullable: true),
            RedemptionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
            ReferenceId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
            Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
            Subject = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
            Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_OpenIddictTokens", x => x.Id);
            table.ForeignKey(
                      name: "FK_OpenIddictTokens_OpenIddictApplications_ApplicationId",
                      column: x => x.ApplicationId,
                      principalTable: "OpenIddictApplications",
                      principalColumn: "Id");
            table.ForeignKey(
                      name: "FK_OpenIddictTokens_OpenIddictAuthorizations_AuthorizationId",
                      column: x => x.AuthorizationId,
                      principalTable: "OpenIddictAuthorizations",
                      principalColumn: "Id");
          });

      migrationBuilder.CreateIndex(
          name: "IX_OpenIddictApplications_ClientId",
          table: "OpenIddictApplications",
          column: "ClientId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type",
          table: "OpenIddictAuthorizations",
          columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

      migrationBuilder.CreateIndex(
          name: "IX_OpenIddictScopes_Name",
          table: "OpenIddictScopes",
          column: "Name",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type",
          table: "OpenIddictTokens",
          columns: new[] { "ApplicationId", "Status", "Subject", "Type" });

      migrationBuilder.CreateIndex(
          name: "IX_OpenIddictTokens_AuthorizationId",
          table: "OpenIddictTokens",
          column: "AuthorizationId");

      migrationBuilder.CreateIndex(
          name: "IX_OpenIddictTokens_ReferenceId",
          table: "OpenIddictTokens",
          column: "ReferenceId",
          unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "OpenIddictScopes");

      migrationBuilder.DropTable(
          name: "OpenIddictTokens");

      migrationBuilder.DropTable(
          name: "OpenIddictAuthorizations");

      migrationBuilder.DropTable(
          name: "OpenIddictApplications");
    }
  }
}