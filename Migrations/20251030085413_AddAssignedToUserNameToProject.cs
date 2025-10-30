using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepartmentalSystemAPIs.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToUserNameToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToUserName",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToUserName",
                table: "Projects");
        }
    }
}
