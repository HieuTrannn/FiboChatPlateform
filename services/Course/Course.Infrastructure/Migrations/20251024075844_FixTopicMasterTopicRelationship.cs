using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTopicMasterTopicRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_MasterTopics_MasterTopicId1",
                schema: "public",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_MasterTopicId1",
                schema: "public",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "MasterTopicId1",
                schema: "public",
                table: "Topics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MasterTopicId1",
                schema: "public",
                table: "Topics",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Topics_MasterTopicId1",
                schema: "public",
                table: "Topics",
                column: "MasterTopicId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_MasterTopics_MasterTopicId1",
                schema: "public",
                table: "Topics",
                column: "MasterTopicId1",
                principalSchema: "public",
                principalTable: "MasterTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
