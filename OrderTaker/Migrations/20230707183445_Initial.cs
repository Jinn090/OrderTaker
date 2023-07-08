using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderTaker.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "varchar(102)", unicode: false, maxLength: 102, nullable: false, computedColumnSql: "(([LastName]+', ')+[FirstName])", stored: true),
                    MobileNumber = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SKUs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKU", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrder",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    DateOfDelivery = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false, defaultValueSql: "('New')"),
                    AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrder", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Customers",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderID = table.Column<int>(type: "int", nullable: false),
                    SKUID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PurchaseItems_PurchaseOrder",
                        column: x => x.PurchaseOrderID,
                        principalTable: "PurchaseOrder",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PurchaseItems_SKUs",
                        column: x => x.SKUID,
                        principalTable: "SKUs",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "Is_Unique_FullName",
                table: "Customers",
                column: "FullName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Is_Unique_Mobile",
                table: "Customers",
                column: "MobileNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_PurchaseOrderID",
                table: "PurchaseItems",
                column: "PurchaseOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_SKUID",
                table: "PurchaseItems",
                column: "SKUID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_CustomerID",
                table: "PurchaseOrder",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "Is_Unique_CODE",
                table: "SKUs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Is_Unique_NAME",
                table: "SKUs",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseItems");

            migrationBuilder.DropTable(
                name: "PurchaseOrder");

            migrationBuilder.DropTable(
                name: "SKUs");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
