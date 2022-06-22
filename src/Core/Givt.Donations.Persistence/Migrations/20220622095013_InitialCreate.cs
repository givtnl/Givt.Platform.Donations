using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Givt.Donations.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonationHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MediumId = table.Column<Guid>(type: "uuid", nullable: false),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    DonationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PayinId = table.Column<Guid>(type: "uuid", nullable: false),
                    Last4 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Fingerprint = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Reason = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonationHistory", x => new { x.Id, x.Modified });
                });

            migrationBuilder.CreateTable(
                name: "PayIns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentProviderExecutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    PayInMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalPaid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayIns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayOuts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentProviderExecutionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    TransactionCount = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionCost = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionTaxes = table.Column<decimal>(type: "numeric", nullable: false),
                    MandateCostCount = table.Column<int>(type: "integer", nullable: false),
                    MandateCost = table.Column<decimal>(type: "numeric", nullable: false),
                    MandateTaxes = table.Column<decimal>(type: "numeric", nullable: false),
                    RTransactionT1Count = table.Column<decimal>(type: "numeric", nullable: false),
                    RTransactionT1Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    RTransactionT2Count = table.Column<decimal>(type: "numeric", nullable: false),
                    RTransactionT2Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    RTransactionAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RTransactionTaxes = table.Column<decimal>(type: "numeric", nullable: false),
                    GivtServiceFee = table.Column<decimal>(type: "numeric", nullable: false),
                    GivtServiceFeeTaxes = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentCost = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentCostTaxes = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentProviderId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayOuts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    MediumId = table.Column<Guid>(type: "uuid", nullable: false),
                    DonorId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    DonationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionReference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PayinId = table.Column<Guid>(type: "uuid", nullable: false),
                    Last4 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Fingerprint = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donations_PayIns_PayinId",
                        column: x => x.PayinId,
                        principalTable: "PayIns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonationHistory_TransactionReference",
                table: "DonationHistory",
                column: "TransactionReference");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_PayinId",
                table: "Donations",
                column: "PayinId");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_TransactionReference",
                table: "Donations",
                column: "TransactionReference");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonationHistory");

            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.DropTable(
                name: "PayOuts");

            migrationBuilder.DropTable(
                name: "PayIns");
        }
    }
}
