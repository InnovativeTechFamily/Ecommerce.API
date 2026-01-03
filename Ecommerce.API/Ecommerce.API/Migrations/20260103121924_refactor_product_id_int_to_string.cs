using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class refactor_product_id_int_to_string : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete dependent data first
            migrationBuilder.Sql("DELETE FROM Media");
            migrationBuilder.Sql("DELETE FROM Products");

            // Drop primary key constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            // Drop the column (now that constraint is removed)
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Products");

            // Add new column
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false);

            // Add primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
