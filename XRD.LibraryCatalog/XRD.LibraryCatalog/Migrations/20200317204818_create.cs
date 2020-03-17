using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XRD.LibCat.Migrations
{
    public partial class create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblCatalog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uid = table.Column<Guid>(nullable: false),
                    Ts = table.Column<DateTime>(nullable: false),
                    Ec = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 1000, nullable: false),
                    Subtitle = table.Column<string>(maxLength: 1000, nullable: true),
                    Publisher = table.Column<string>(maxLength: 1000, nullable: true),
                    PubDate = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PageCount = table.Column<int>(nullable: true),
                    MinAge = table.Column<int>(nullable: true),
                    MaxAge = table.Column<int>(nullable: true),
                    MinGrade = table.Column<long>(nullable: false),
                    MaxGrade = table.Column<long>(nullable: false),
                    ShelfLocation = table.Column<string>(maxLength: 150, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCatalog", x => x.Id);
                    table.UniqueConstraint("AK_tblCatalog_Uid", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "tblStaffMembers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uid = table.Column<Guid>(nullable: false),
                    Ts = table.Column<DateTime>(nullable: false),
                    Ec = table.Column<int>(nullable: false),
                    Prefix = table.Column<string>(maxLength: 50, nullable: true),
                    First = table.Column<string>(maxLength: 50, nullable: false),
                    Middle = table.Column<string>(maxLength: 50, nullable: true),
                    Last = table.Column<string>(maxLength: 50, nullable: false),
                    Suffix = table.Column<string>(maxLength: 50, nullable: true),
                    Nickname = table.Column<string>(maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Room = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(maxLength: 150, nullable: true),
                    GradesTaught = table.Column<long>(nullable: false),
                    Subjects = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblStaffMembers", x => x.Id);
                    table.UniqueConstraint("AK_tblStaffMembers_Uid", x => x.Uid);
                });

            migrationBuilder.CreateTable(
                name: "tblAuthors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uid = table.Column<Guid>(nullable: false),
                    CatId = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(maxLength: 400, nullable: true),
                    OrdIndex = table.Column<int>(nullable: false),
                    Role = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblAuthors", x => x.Id);
                    table.UniqueConstraint("AK_tblAuthors_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_tblAuthors_tblCatalog_CatId",
                        column: x => x.CatId,
                        principalTable: "tblCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblGenres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uid = table.Column<Guid>(nullable: false),
                    CatId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblGenres", x => x.Id);
                    table.UniqueConstraint("AK_tblGenres_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_tblGenres_tblCatalog_CatId",
                        column: x => x.CatId,
                        principalTable: "tblCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblIdentifiers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uid = table.Column<Guid>(nullable: false),
                    CatId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblIdentifiers", x => x.Id);
                    table.UniqueConstraint("AK_tblIdentifiers_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_tblIdentifiers_tblCatalog_CatId",
                        column: x => x.CatId,
                        principalTable: "tblCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tblOwnedBooks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uid = table.Column<Guid>(nullable: false),
                    BookNumber = table.Column<int>(nullable: false),
                    CatId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblOwnedBooks", x => x.Id);
                    table.UniqueConstraint("AK_tblOwnedBooks_BookNumber", x => x.BookNumber);
                    table.UniqueConstraint("AK_tblOwnedBooks_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_tblOwnedBooks_tblCatalog_CatId",
                        column: x => x.CatId,
                        principalTable: "tblCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblPatrons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uid = table.Column<Guid>(nullable: false),
                    Ts = table.Column<DateTime>(nullable: false),
                    Ec = table.Column<int>(nullable: false),
                    Prefix = table.Column<string>(maxLength: 50, nullable: true),
                    First = table.Column<string>(maxLength: 50, nullable: false),
                    Middle = table.Column<string>(maxLength: 50, nullable: true),
                    Last = table.Column<string>(maxLength: 50, nullable: false),
                    Suffix = table.Column<string>(maxLength: 50, nullable: true),
                    Nickname = table.Column<string>(maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Age = table.Column<int>(nullable: true),
                    Grade = table.Column<long>(nullable: false),
                    MinAge = table.Column<int>(nullable: true),
                    MaxAge = table.Column<int>(nullable: true),
                    MinGrade = table.Column<long>(nullable: false),
                    MaxGrade = table.Column<long>(nullable: false),
                    Email = table.Column<string>(maxLength: 150, nullable: true),
                    TeacherId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPatrons", x => x.Id);
                    table.UniqueConstraint("AK_tblPatrons_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_tblPatrons_tblStaffMembers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "tblStaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tblBorrowingHx",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Uid = table.Column<Guid>(nullable: false),
                    BookNumber = table.Column<int>(nullable: false),
                    PatronId = table.Column<int>(nullable: false),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false),
                    CheckInDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblBorrowingHx", x => x.Id);
                    table.UniqueConstraint("AK_tblBorrowingHx_Uid", x => x.Uid);
                    table.ForeignKey(
                        name: "FK_tblBorrowingHx_tblOwnedBooks_BookNumber",
                        column: x => x.BookNumber,
                        principalTable: "tblOwnedBooks",
                        principalColumn: "BookNumber",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tblBorrowingHx_tblPatrons_PatronId",
                        column: x => x.PatronId,
                        principalTable: "tblPatrons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblAuthors_CatId_FullName",
                table: "tblAuthors",
                columns: new[] { "CatId", "FullName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblAuthors_CatId_OrdIndex",
                table: "tblAuthors",
                columns: new[] { "CatId", "OrdIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_tblBorrowingHx_PatronId",
                table: "tblBorrowingHx",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "IX_tblBorrowingHx_BookNumber_PatronId_DueDate",
                table: "tblBorrowingHx",
                columns: new[] { "BookNumber", "PatronId", "DueDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblCatalog_IsDeleted",
                table: "tblCatalog",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_tblCatalog_Ts",
                table: "tblCatalog",
                column: "Ts");

            migrationBuilder.CreateIndex(
                name: "IX_tblGenres_CatId_Value",
                table: "tblGenres",
                columns: new[] { "CatId", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblIdentifiers_CatId_Value",
                table: "tblIdentifiers",
                columns: new[] { "CatId", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblOwnedBooks_CatId",
                table: "tblOwnedBooks",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_tblOwnedBooks_IsDeleted",
                table: "tblOwnedBooks",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_tblPatrons_IsDeleted",
                table: "tblPatrons",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_tblPatrons_TeacherId",
                table: "tblPatrons",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_tblPatrons_Ts",
                table: "tblPatrons",
                column: "Ts");

            migrationBuilder.CreateIndex(
                name: "IX_tblStaffMembers_IsDeleted",
                table: "tblStaffMembers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_tblStaffMembers_Ts",
                table: "tblStaffMembers",
                column: "Ts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblAuthors");

            migrationBuilder.DropTable(
                name: "tblBorrowingHx");

            migrationBuilder.DropTable(
                name: "tblGenres");

            migrationBuilder.DropTable(
                name: "tblIdentifiers");

            migrationBuilder.DropTable(
                name: "tblOwnedBooks");

            migrationBuilder.DropTable(
                name: "tblPatrons");

            migrationBuilder.DropTable(
                name: "tblCatalog");

            migrationBuilder.DropTable(
                name: "tblStaffMembers");
        }
    }
}
