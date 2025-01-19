﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMbackend.Migrations
{
    /// <inheritdoc />
    public partial class contactFieldAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "Shops",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contact",
                table: "Shops");
        }
    }
}
