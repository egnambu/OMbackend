using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMbackend.Migrations
{
    /// <inheritdoc />
    public partial class addDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Heading = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Votes = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<int>(type: "int", nullable: true),
                    Badge1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Badge2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Badge3 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Badge4 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Badge5 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Item01 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Item02 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Item03 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Item04 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Item05 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Price = table.Column<float>(type: "real", nullable: false),
                    InStock = table.Column<int>(type: "int", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Heading = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Reviews = table.Column<int>(type: "int", nullable: false),
                    Trust = table.Column<int>(type: "int", nullable: true),
                    IconsIndex = table.Column<int>(type: "int", nullable: true),
                    Services = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Multiplier = table.Column<double>(type: "float", nullable: true),
                    Ratings = table.Column<double>(type: "float", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: true),
                    PaymentMethods = table.Column<int>(type: "int", nullable: true),
                    Users = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Color01 = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Color02 = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Color03 = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Image = table.Column<int>(type: "int", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecretQuestion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SecretAnswer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCommentID = table.Column<int>(type: "int", nullable: true),
                    PostID = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Votes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentID",
                        column: x => x.ParentCommentID,
                        principalTable: "Comments",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostID",
                        column: x => x.PostID,
                        principalTable: "Posts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentReviewID = table.Column<int>(type: "int", nullable: true),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Service = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Stars = table.Column<float>(type: "real", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reviews_Reviews_ParentReviewID",
                        column: x => x.ParentReviewID,
                        principalTable: "Reviews",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reviews_Shops_ShopID",
                        column: x => x.ShopID,
                        principalTable: "Shops",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAdminSeen = table.Column<bool>(type: "bit", nullable: false),
                    IsSeen = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Conversations_Shops_ShopID",
                        column: x => x.ShopID,
                        principalTable: "Shops",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversations_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    isPaid = table.Column<bool>(type: "bit", nullable: true),
                    State = table.Column<bool>(type: "bit", nullable: true),
                    Field1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Field2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Field3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Field4 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Field5 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Price = table.Column<float>(type: "real", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Orders_Shops_ShopID",
                        column: x => x.ShopID,
                        principalTable: "Shops",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ShopID = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsUser = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConversationID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Messages_Shops_ShopID",
                        column: x => x.ShopID,
                        principalTable: "Shops",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentID",
                table: "Comments",
                column: "ParentCommentID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostID",
                table: "Comments",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ShopID",
                table: "Conversations",
                column: "ShopID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_UserID",
                table: "Conversations",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationID",
                table: "Messages",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ShopID",
                table: "Messages",
                column: "ShopID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserID",
                table: "Messages",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShopID",
                table: "Orders",
                column: "ShopID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserID",
                table: "Orders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ParentReviewID",
                table: "Reviews",
                column: "ParentReviewID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ShopID",
                table: "Reviews",
                column: "ShopID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
