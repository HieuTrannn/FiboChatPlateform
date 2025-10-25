using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToAccount : Migration
    {
        /// <inheritdoc />
        // File: services/Authentication/Authentication.Infrastructure/Migrations/20251024152328_AddAvatarUrlAndClassIdToAccount.cs

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "Accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ClassId",
                table: "Accounts",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Classes_ClassId",
                table: "Accounts",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Classes_ClassId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ClassId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Accounts");
        }
    }
}
