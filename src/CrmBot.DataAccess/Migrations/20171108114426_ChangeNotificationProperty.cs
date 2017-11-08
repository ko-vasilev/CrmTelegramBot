using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CrmBot.DataAccess.Migrations
{
    public partial class ChangeNotificationProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "NotificationSubscriptions");

            migrationBuilder.AddColumn<int>(
                name: "EventType",
                table: "NotificationSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventType",
                table: "NotificationSubscriptions");

            migrationBuilder.AddColumn<int>(
                name: "NotificationType",
                table: "NotificationSubscriptions",
                nullable: false,
                defaultValue: 0);
        }
    }
}
