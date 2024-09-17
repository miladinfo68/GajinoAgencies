using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GajinoAgencies.Migrations
{
    public partial class addisAdmintoagencytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Agencies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2024, 9, 7, 15, 16, 7, 724, DateTimeKind.Local).AddTicks(3660));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2024, 9, 7, 15, 16, 7, 724, DateTimeKind.Local).AddTicks(3678));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2024, 9, 7, 15, 16, 7, 724, DateTimeKind.Local).AddTicks(3680));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2024, 9, 7, 15, 16, 7, 724, DateTimeKind.Local).AddTicks(3682));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreationDate",
                value: new DateTime(2024, 9, 7, 15, 16, 7, 724, DateTimeKind.Local).AddTicks(3683));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreationDate",
                value: new DateTime(2024, 9, 7, 15, 16, 7, 724, DateTimeKind.Local).AddTicks(3688));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreationDate",
                value: new DateTime(2024, 9, 7, 15, 16, 7, 724, DateTimeKind.Local).AddTicks(3690));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Agencies");

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8448));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8460));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8461));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8462));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreationDate",
                value: new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8463));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreationDate",
                value: new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8535));

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreationDate",
                value: new DateTime(2024, 8, 18, 10, 30, 27, 453, DateTimeKind.Local).AddTicks(8536));
        }
    }
}
