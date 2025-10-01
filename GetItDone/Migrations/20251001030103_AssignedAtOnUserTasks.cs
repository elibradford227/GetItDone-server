using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetItDone.Migrations
{
    /// <inheritdoc />
    public partial class AssignedAtOnUserTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                schema: "identity",
                table: "UserTasks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedAt",
                schema: "identity",
                table: "UserTasks");
        }
    }
}
