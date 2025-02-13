using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetItDone.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedIncorrectOwnerIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_AspNetUsers_OwnerId",
                schema: "identity",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_OwnerId",
                schema: "identity",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                schema: "identity",
                table: "Task");

            migrationBuilder.AlterColumn<string>(
                name: "Ownerid",
                schema: "identity",
                table: "Task",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Ownerid",
                schema: "identity",
                table: "Task",
                column: "Ownerid");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_AspNetUsers_Ownerid",
                schema: "identity",
                table: "Task",
                column: "Ownerid",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_AspNetUsers_Ownerid",
                schema: "identity",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_Ownerid",
                schema: "identity",
                table: "Task");

            migrationBuilder.AlterColumn<int>(
                name: "Ownerid",
                schema: "identity",
                table: "Task",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                schema: "identity",
                table: "Task",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Task_OwnerId",
                schema: "identity",
                table: "Task",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_AspNetUsers_OwnerId",
                schema: "identity",
                table: "Task",
                column: "OwnerId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
