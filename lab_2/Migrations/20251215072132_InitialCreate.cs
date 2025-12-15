using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace lab_2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailLogs",
                columns: table => new
                {
                    MailItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    LogDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailLogs", x => new { x.MailItemId, x.LogDate });
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Label = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SenderName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ReceiverName = table.Column<string>(type: "TEXT", nullable: true, defaultValue: "Невідомий"),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    TrackingNumber = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailItems_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BaseMailItemTag",
                columns: table => new
                {
                    MailItemsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseMailItemTag", x => new { x.MailItemsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_BaseMailItemTag_MailItems_MailItemsId",
                        column: x => x.MailItemsId,
                        principalTable: "MailItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseMailItemTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParcelMetadatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    ParcelId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelMetadatas", x => x.Id);
                    table.CheckConstraint("CK_Weight_Positive", "Weight > 0");
                    table.ForeignKey(
                        name: "FK_ParcelMetadatas_MailItems_ParcelId",
                        column: x => x.ParcelId,
                        principalTable: "MailItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "Address" },
                values: new object[] { 1, "Київ, вул. Хрещатик, 1" });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Label" },
                values: new object[,]
                {
                    { 1, "Терміново" },
                    { 2, "Крихке" }
                });

            migrationBuilder.InsertData(
                table: "MailItems",
                columns: new[] { "Id", "BranchId", "Discriminator", "Name", "ReceiverName", "SenderName" },
                values: new object[] { 1, 1, "Letter", "Лист", "Іван", "Петро" });

            migrationBuilder.InsertData(
                table: "MailItems",
                columns: new[] { "Id", "BranchId", "Discriminator", "Name", "ReceiverName", "SenderName", "TrackingNumber" },
                values: new object[] { 2, 1, "Parcel", "Посилка", "Марія", "Ольга", "TRACK123" });

            migrationBuilder.CreateIndex(
                name: "IX_BaseMailItemTag_TagsId",
                table: "BaseMailItemTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_MailItems_BranchId",
                table: "MailItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MailItems_TrackingNumber",
                table: "MailItems",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParcelMetadatas_ParcelId",
                table: "ParcelMetadatas",
                column: "ParcelId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseMailItemTag");

            migrationBuilder.DropTable(
                name: "MailLogs");

            migrationBuilder.DropTable(
                name: "ParcelMetadatas");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "MailItems");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
