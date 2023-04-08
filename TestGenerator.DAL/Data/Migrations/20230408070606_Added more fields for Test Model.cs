using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGenerator.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedmorefieldsforTestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Test",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Test",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfAnswersPerQuestion",
                table: "Test",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfQuestions",
                table: "Test",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "NumberOfAnswersPerQuestion",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "NumberOfQuestions",
                table: "Test");
        }
    }
}
