using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Pedidos.Migrations
{
    /// <inheritdoc />
    public partial class alteraTabelaPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fechado",
                table: "Pedidos",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Pedidos",
                newName: "Fechado");
        }
    }
}
