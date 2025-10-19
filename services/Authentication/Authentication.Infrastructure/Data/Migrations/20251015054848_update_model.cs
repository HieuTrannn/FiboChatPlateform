using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class update_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Lecture_LectureId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_LectureId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "LectureId",
                table: "Classes");

            migrationBuilder.RenameColumn(
                name: "ClassName",
                table: "Classes",
                newName: "Code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Semester",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Semester",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Term",
                table: "Semester",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Semester",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "LecturerId",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LecturerId1",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Classes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassEnrollment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RoleInClass = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassEnrollment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassEnrollment_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassEnrollment_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_LecturerId1",
                table: "Classes",
                column: "LecturerId1");

            migrationBuilder.CreateIndex(
                name: "IX_ClassEnrollment_ClassId",
                table: "ClassEnrollment",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassEnrollment_GroupId",
                table: "ClassEnrollment",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_ClassId",
                table: "Group",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Lecture_LecturerId1",
                table: "Classes",
                column: "LecturerId1",
                principalTable: "Lecture",
                principalColumn: "LecturerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Lecture_LecturerId1",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "ClassEnrollment");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Classes_LecturerId1",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "Term",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "LecturerId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "LecturerId1",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Classes");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Classes",
                newName: "ClassName");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Semester",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Semester",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LectureId",
                table: "Classes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Classes_LectureId",
                table: "Classes",
                column: "LectureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Lecture_LectureId",
                table: "Classes",
                column: "LectureId",
                principalTable: "Lecture",
                principalColumn: "LecturerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
