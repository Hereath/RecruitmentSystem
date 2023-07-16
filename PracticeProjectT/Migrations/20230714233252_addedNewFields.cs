using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeProjectT.Migrations
{
    /// <inheritdoc />
    public partial class addedNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Availability",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyRegistrationNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyRegistrationNumber",
                table: "AspNetUsers");
        }
    }
}
