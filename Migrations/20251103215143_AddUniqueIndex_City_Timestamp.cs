using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BAERecruitmentProject.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndex_City_Timestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WeatherObservations_CityName_ObservedAtUtc",
                table: "WeatherObservations",
                columns: new[] { "CityName", "ObservedAtUtc" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeatherObservations_CityName_ObservedAtUtc",
                table: "WeatherObservations");
        }
    }
}
