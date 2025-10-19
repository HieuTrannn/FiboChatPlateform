using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class update_lecturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Accounts_HomeroomTeacherId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_HomeroomTeacherId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "HomeroomTeacherId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Accounts");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "ClassName",
                table: "Classes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "LectureId",
                table: "Classes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "IsVerified",
                table: "Accounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.CreateTable(
                name: "Lecture",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lecture_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_LectureId",
                table: "Classes",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_AccountId",
                table: "Lecture",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Lecture_LectureId",
                table: "Classes",
                column: "LectureId",
                principalTable: "Lecture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Lecture_LectureId",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "Lecture");

            migrationBuilder.DropIndex(
                name: "IX_Classes_LectureId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "LectureId",
                table: "Classes");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Semester",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ClassName",
                table: "Classes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Classes",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HomeroomTeacherId",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                table: "Accounts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_HomeroomTeacherId",
                table: "Classes",
                column: "HomeroomTeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Accounts_HomeroomTeacherId",
                table: "Classes",
                column: "HomeroomTeacherId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
