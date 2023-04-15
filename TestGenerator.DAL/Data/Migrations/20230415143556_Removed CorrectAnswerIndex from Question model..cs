using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGenerator.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCorrectAnswerIndexfromQuestionmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswerIndex",
                table: "Question");

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Answer",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Answer");

            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswerIndex",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
