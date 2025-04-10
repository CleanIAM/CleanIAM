using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedKernel.Migrations
{
    /// <inheritdoc />
    public partial class Update_oidc_objects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "oidc");

            migrationBuilder.RenameTable(
                name: "OpenIddictTokens",
                newName: "OpenIddictTokens",
                newSchema: "oidc");

            migrationBuilder.RenameTable(
                name: "OpenIddictScopes",
                newName: "OpenIddictScopes",
                newSchema: "oidc");

            migrationBuilder.RenameTable(
                name: "OpenIddictAuthorizations",
                newName: "OpenIddictAuthorizations",
                newSchema: "oidc");

            migrationBuilder.RenameTable(
                name: "OpenIddictApplications",
                newName: "OpenIddictApplications",
                newSchema: "oidc");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthorizationId",
                schema: "oidc",
                table: "OpenIddictTokens",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationId",
                schema: "oidc",
                table: "OpenIddictTokens",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "oidc",
                table: "OpenIddictTokens",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "oidc",
                table: "OpenIddictScopes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationId",
                schema: "oidc",
                table: "OpenIddictAuthorizations",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "oidc",
                table: "OpenIddictAuthorizations",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "oidc",
                table: "OpenIddictApplications",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OpenIddictTokens",
                schema: "oidc",
                newName: "OpenIddictTokens");

            migrationBuilder.RenameTable(
                name: "OpenIddictScopes",
                schema: "oidc",
                newName: "OpenIddictScopes");

            migrationBuilder.RenameTable(
                name: "OpenIddictAuthorizations",
                schema: "oidc",
                newName: "OpenIddictAuthorizations");

            migrationBuilder.RenameTable(
                name: "OpenIddictApplications",
                schema: "oidc",
                newName: "OpenIddictApplications");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorizationId",
                table: "OpenIddictTokens",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationId",
                table: "OpenIddictTokens",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OpenIddictTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OpenIddictScopes",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationId",
                table: "OpenIddictAuthorizations",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OpenIddictAuthorizations",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "OpenIddictApplications",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
