using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetItDone.Migrations
{
    /// <inheritdoc />
    public partial class DbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_AspNetUsers_Ownerid",
                schema: "identity",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTask_AspNetUsers_UserId",
                schema: "identity",
                table: "UserTask");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTask_Task_TaskId",
                schema: "identity",
                table: "UserTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTask",
                schema: "identity",
                table: "UserTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Task",
                schema: "identity",
                table: "Task");

            migrationBuilder.RenameTable(
                name: "UserTask",
                schema: "identity",
                newName: "UserTasks",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Task",
                schema: "identity",
                newName: "Tasks",
                newSchema: "identity");

            migrationBuilder.RenameIndex(
                name: "IX_UserTask_UserId",
                schema: "identity",
                table: "UserTasks",
                newName: "IX_UserTasks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTask_TaskId",
                schema: "identity",
                table: "UserTasks",
                newName: "IX_UserTasks_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Task_Ownerid",
                schema: "identity",
                table: "Tasks",
                newName: "IX_Tasks_Ownerid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTasks",
                schema: "identity",
                table: "UserTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                schema: "identity",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_Ownerid",
                schema: "identity",
                table: "Tasks",
                column: "Ownerid",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_AspNetUsers_UserId",
                schema: "identity",
                table: "UserTasks",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_Tasks_TaskId",
                schema: "identity",
                table: "UserTasks",
                column: "TaskId",
                principalSchema: "identity",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_Ownerid",
                schema: "identity",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_AspNetUsers_UserId",
                schema: "identity",
                table: "UserTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_Tasks_TaskId",
                schema: "identity",
                table: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTasks",
                schema: "identity",
                table: "UserTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                schema: "identity",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "UserTasks",
                schema: "identity",
                newName: "UserTask",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Tasks",
                schema: "identity",
                newName: "Task",
                newSchema: "identity");

            migrationBuilder.RenameIndex(
                name: "IX_UserTasks_UserId",
                schema: "identity",
                table: "UserTask",
                newName: "IX_UserTask_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTasks_TaskId",
                schema: "identity",
                table: "UserTask",
                newName: "IX_UserTask_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_Ownerid",
                schema: "identity",
                table: "Task",
                newName: "IX_Task_Ownerid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTask",
                schema: "identity",
                table: "UserTask",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Task",
                schema: "identity",
                table: "Task",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_AspNetUsers_Ownerid",
                schema: "identity",
                table: "Task",
                column: "Ownerid",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTask_AspNetUsers_UserId",
                schema: "identity",
                table: "UserTask",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTask_Task_TaskId",
                schema: "identity",
                table: "UserTask",
                column: "TaskId",
                principalSchema: "identity",
                principalTable: "Task",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
