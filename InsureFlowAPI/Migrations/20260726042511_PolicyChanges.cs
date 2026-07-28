using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureFlowAPI.Migrations
{
    /// <inheritdoc />
    public partial class PolicyChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPremiumPaymentDate",
                table: "Policies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "NextPremiumDueDate",
                table: "Policies",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPremiumPaymentDate",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "NextPremiumDueDate",
                table: "Policies");
        }
    }
}
