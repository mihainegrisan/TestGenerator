using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGenerator.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedQuestionTagsmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionTag");

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Question",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_TagId",
                table: "Question",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Tag_TagId",
                table: "Question",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Tag_TagId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_TagId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Question");

            migrationBuilder.CreateTable(
                name: "QuestionTag",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTag", x => new { x.QuestionId, x.TagId });
                    table.ForeignKey(
                        name: "FK_QuestionTag_Question_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_TagId",
                table: "QuestionTag",
                column: "TagId");
        }
    }
}
