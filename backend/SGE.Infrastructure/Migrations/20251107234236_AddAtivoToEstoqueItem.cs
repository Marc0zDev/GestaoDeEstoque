using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAtivoToEstoqueItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "EstoqueMovimentos",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "EstoqueItens",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Categorias",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "EstoqueMovimentos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "EstoqueItens");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Categorias");
        }
    }
}
