using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BienSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HetHan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayRa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OgiuXe = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInfos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleInfos");
        }
    }
}
