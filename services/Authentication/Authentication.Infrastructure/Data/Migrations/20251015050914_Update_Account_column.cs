using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Account_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Lecture_LectureId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecture_Accounts_AccountId",
                table: "Lecture");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecture",
                table: "Lecture");

            migrationBuilder.DropIndex(
                name: "IX_Lecture_AccountId",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Lecture");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Lecture");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Lecture",
                newName: "LecturerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecture",
                table: "Lecture",
                column: "LecturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Lecture_LectureId",
                table: "Classes",
                column: "LectureId",
                principalTable: "Lecture",
                principalColumn: "LecturerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lecture_Accounts_LecturerId",
                table: "Lecture",
                column: "LecturerId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Lecture_LectureId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecture_Accounts_LecturerId",
                table: "Lecture");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecture",
                table: "Lecture");

            migrationBuilder.RenameColumn(
                name: "LecturerId",
                table: "Lecture",
                newName: "AccountId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Lecture",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Lecture",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Lecture",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Lecture",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Lecture",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Lecture",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Lecture",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Lecture",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecture",
                table: "Lecture",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Lecture_Accounts_AccountId",
                table: "Lecture",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
