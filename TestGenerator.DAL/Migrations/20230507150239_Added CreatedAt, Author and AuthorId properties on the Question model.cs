using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestGenerator.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedAtAuthorandAuthorIdpropertiesontheQuestionmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Question",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Question",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Question_AuthorId",
                table: "Question",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_AspNetUsers_AuthorId",
                table: "Question",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_AspNetUsers_AuthorId",
                table: "Question");

            migrationBuilder.DropIndex(
                name: "IX_Question_AuthorId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Question");
        }
    }
}
