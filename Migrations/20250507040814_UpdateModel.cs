using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_AspNetUsers_Id",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_Id",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Students_StudentId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_StudentId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "ID_Ticket",
                table: "Tickets",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Students",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MSSV",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Staffs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "MSSV");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_StudentId",
                table: "Tickets",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_UserId",
                table: "Staffs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_AspNetUsers_UserId",
                table: "Staffs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Students_StudentId",
                table: "Tickets",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "MSSV",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_AspNetUsers_UserId",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Students_StudentId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_StudentId",
                table: "Tickets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_UserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_UserId",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Staffs");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tickets",
                newName: "ID_Ticket");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Students",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Tickets",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MSSV",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_StudentId",
                table: "Tickets",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_AspNetUsers_Id",
                table: "Staffs",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_Id",
                table: "Students",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Students_StudentId",
                table: "Tickets",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
