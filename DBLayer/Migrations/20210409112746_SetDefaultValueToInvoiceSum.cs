using Microsoft.EntityFrameworkCore.Migrations;

namespace DBLayer.Migrations
{
    public partial class SetDefaultValueToInvoiceSum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Sum",
                table: "Invoice",
                type: "money",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldPrecision: 19,
                oldScale: 4);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Sum",
                table: "Invoice",
                type: "money",
                precision: 19,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldPrecision: 19,
                oldScale: 4,
                oldDefaultValue: 0m);
        }
    }
}
