using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionProfileRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_AspNetUsers_IdentityUserId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_IdentityUserId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_IdentityUserId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Answers_IdentityUserId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Answers");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "Answers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ProfileId",
                table: "Questions",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_ProfileId",
                table: "Answers",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Profiles_ProfileId",
                table: "Answers",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Profiles_ProfileId",
                table: "Questions",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Profiles_ProfileId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Profiles_ProfileId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ProfileId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Answers_ProfileId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Answers");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Answers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_IdentityUserId",
                table: "Questions",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_IdentityUserId",
                table: "Answers",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_AspNetUsers_IdentityUserId",
                table: "Answers",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_IdentityUserId",
                table: "Questions",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
