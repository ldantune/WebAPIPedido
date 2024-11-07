using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Pedidos.Migrations
{
    /// <inheritdoc />
    public partial class ColunaPrecoUnitario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoUnitario",
                table: "PedidoProdutos",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoUnitario",
                table: "PedidoProdutos");
        }
    }
}
