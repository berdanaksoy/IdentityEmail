using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityEmail.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStatus",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "MessageCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageCategory", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "UserMessageBoxes",
                columns: table => new
                {
                    UserMessageBoxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserMessageBoxEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    IsTrash = table.Column<bool>(type: "bit", nullable: false),
                    IsSpam = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsStarred = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessageBoxes", x => x.UserMessageBoxId);
                    table.ForeignKey(
                        name: "FK_UserMessageBoxes_MessageCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "MessageCategory",
                        principalColumn: "CategoryId");
                    table.ForeignKey(
                        name: "FK_UserMessageBoxes_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessageBoxes_CategoryId",
                table: "UserMessageBoxes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessageBoxes_MessageId",
                table: "UserMessageBoxes",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMessageBoxes");

            migrationBuilder.DropTable(
                name: "MessageCategory");

            migrationBuilder.AddColumn<bool>(
                name: "IsStatus",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
