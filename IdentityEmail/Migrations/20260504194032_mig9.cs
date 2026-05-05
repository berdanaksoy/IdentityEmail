using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityEmail.Migrations
{
    /// <inheritdoc />
    public partial class mig9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserContactCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContactCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserContactCategories_MessageCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "MessageCategory",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserContactCategories_CategoryId",
                table: "UserContactCategories",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserContactCategories");
        }
    }
}
