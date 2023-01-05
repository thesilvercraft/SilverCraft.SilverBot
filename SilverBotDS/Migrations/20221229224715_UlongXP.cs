using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    /// <inheritdoc />
    public partial class UlongXP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XPString",
                table: "userExperiences");

            migrationBuilder.AddColumn<ulong>(
                name: "XP",
                table: "userExperiences",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XP",
                table: "userExperiences");

            migrationBuilder.AddColumn<string>(
                name: "XPString",
                table: "userExperiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
