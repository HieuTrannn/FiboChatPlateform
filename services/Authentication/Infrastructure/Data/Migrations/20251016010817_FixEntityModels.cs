using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixEntityModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrollment_Classes_ClassId",
                table: "ClassEnrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrollment_Group_GroupId",
                table: "ClassEnrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Lecture_LecturerId1",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Semester_SemesterId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_Classes_ClassId",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK_Lecture_Accounts_LecturerId",
                table: "Lecture");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Semester",
                table: "Semester");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecture",
                table: "Lecture");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassEnrollment",
                table: "ClassEnrollment");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Semester");

            migrationBuilder.RenameTable(
                name: "Semester",
                newName: "Semesters");

            migrationBuilder.RenameTable(
                name: "Lecture",
                newName: "Lectures");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameTable(
                name: "ClassEnrollment",
                newName: "ClassEnrollments");

            migrationBuilder.RenameIndex(
                name: "IX_Group_ClassId",
                table: "Groups",
                newName: "IX_Groups_ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassEnrollment_GroupId",
                table: "ClassEnrollments",
                newName: "IX_ClassEnrollments_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassEnrollment_ClassId",
                table: "ClassEnrollments",
                newName: "IX_ClassEnrollments_ClassId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Semesters",
                table: "Semesters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lectures",
                table: "Lectures",
                column: "LecturerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassEnrollments",
                table: "ClassEnrollments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrollments_Classes_ClassId",
                table: "ClassEnrollments",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrollments_Groups_GroupId",
                table: "ClassEnrollments",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Lectures_LecturerId1",
                table: "Classes",
                column: "LecturerId1",
                principalTable: "Lectures",
                principalColumn: "LecturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Semesters_SemesterId",
                table: "Classes",
                column: "SemesterId",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Classes_ClassId",
                table: "Groups",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lectures_Accounts_LecturerId",
                table: "Lectures",
                column: "LecturerId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrollments_Classes_ClassId",
                table: "ClassEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassEnrollments_Groups_GroupId",
                table: "ClassEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Lectures_LecturerId1",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Semesters_SemesterId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Classes_ClassId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Lectures_Accounts_LecturerId",
                table: "Lectures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Semesters",
                table: "Semesters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lectures",
                table: "Lectures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassEnrollments",
                table: "ClassEnrollments");

            migrationBuilder.RenameTable(
                name: "Semesters",
                newName: "Semester");

            migrationBuilder.RenameTable(
                name: "Lectures",
                newName: "Lecture");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameTable(
                name: "ClassEnrollments",
                newName: "ClassEnrollment");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_ClassId",
                table: "Group",
                newName: "IX_Group_ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassEnrollments_GroupId",
                table: "ClassEnrollment",
                newName: "IX_ClassEnrollment_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassEnrollments_ClassId",
                table: "ClassEnrollment",
                newName: "IX_ClassEnrollment_ClassId");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Classes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Classes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Classes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Semester",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Semester",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Semester",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Semester",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Semester",
                table: "Semester",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecture",
                table: "Lecture",
                column: "LecturerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassEnrollment",
                table: "ClassEnrollment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrollment_Classes_ClassId",
                table: "ClassEnrollment",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEnrollment_Group_GroupId",
                table: "ClassEnrollment",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Lecture_LecturerId1",
                table: "Classes",
                column: "LecturerId1",
                principalTable: "Lecture",
                principalColumn: "LecturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Semester_SemesterId",
                table: "Classes",
                column: "SemesterId",
                principalTable: "Semester",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Classes_ClassId",
                table: "Group",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lecture_Accounts_LecturerId",
                table: "Lecture",
                column: "LecturerId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
