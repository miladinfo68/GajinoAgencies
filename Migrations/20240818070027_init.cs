using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GajinoAgencies.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Province = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CityCode = table.Column<string>(type: "char(3)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GetDate()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mobile = table.Column<string>(type: "varchar(15)", nullable: false),
                    Password = table.Column<string>(type: "varchar(256)", nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", nullable: true),
                    Salt = table.Column<string>(type: "varchar(256)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Institute = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GetDate()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agencies_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountNo = table.Column<string>(type: "varchar(20)", nullable: false),
                    TraceNo = table.Column<string>(type: "varchar(20)", nullable: false),
                    AgencyId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GetDate()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(100)", nullable: false),
                    Code = table.Column<string>(type: "varchar(10)", nullable: false),
                    Discount = table.Column<byte>(type: "tinyint", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    UnUsed = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PackageDetailIds = table.Column<string>(type: "varchar(200)", nullable: false),
                    AgencyId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GetDate()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vouchers_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "City", "CityCode", "CreationDate", "IsActive", "Province" },
                values: new object[,]
                {
                    { 1, "تهران", "THR", new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8448), true, "تهران" },
                    { 2, "همدان", "HMD", new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8460), true, "همدان" },
                    { 3, "شیراز", "SHZ", new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8461), true, "شیراز" },
                    { 4, "اصفهان", "ESF", new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8462), true, "اصفهان" },
                    { 5, "یزد", "YZD", new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8463), true, "یزد" },
                    { 6, "لرستان", "LOR", new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8535), true, "لرستان" },
                    { 7, "کرج", "KRJ", new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8536), true, "کرج" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_LocationId",
                table: "Agencies",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AgencyId",
                table: "Payments",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_AgencyId",
                table: "Vouchers",
                column: "AgencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "Agencies");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
