using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureFlowAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "PremiumPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InsuranceProductId",
                table: "Policies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Claims",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PremiumPayments_CustomerId",
                table: "PremiumPayments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_InsuranceProductId",
                table: "Policies",
                column: "InsuranceProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_CustomerId",
                table: "Claims",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Customers_CustomerId",
                table: "Claims",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_InsuranceProducts_InsuranceProductId",
                table: "Policies",
                column: "InsuranceProductId",
                principalTable: "InsuranceProducts",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PremiumPayments_Customers_CustomerId",
                table: "PremiumPayments",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Customers_CustomerId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Policies_InsuranceProducts_InsuranceProductId",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_PremiumPayments_Customers_CustomerId",
                table: "PremiumPayments");

            migrationBuilder.DropIndex(
                name: "IX_PremiumPayments_CustomerId",
                table: "PremiumPayments");

            migrationBuilder.DropIndex(
                name: "IX_Policies_InsuranceProductId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Claims_CustomerId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "PremiumPayments");

            migrationBuilder.DropColumn(
                name: "InsuranceProductId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Claims");
        }
    }
}
