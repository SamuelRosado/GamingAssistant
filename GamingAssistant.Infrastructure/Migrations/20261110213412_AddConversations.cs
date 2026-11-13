using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamingAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConversations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Crear la tabla de conversaciones
            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),

                    Title = table.Column<string>(
                        type: "TEXT",
                        maxLength: 200,
                        nullable: false),

                    CreatedAt = table.Column<DateTime>(
                        type: "TEXT",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                });

            // 2. Crear una conversación para conservar el historial anterior
            migrationBuilder.Sql("""
                INSERT INTO Conversations (Id, Title, CreatedAt)
                VALUES (1, 'Primera conversación con SAMIRA',
                        strftime('%Y-%m-%d %H:%M:%f', 'now'));
                """);

            // 3. Agregar ConversationId a los mensajes existentes
            migrationBuilder.AddColumn<int>(
                name: "ConversationId",
                table: "ConversationMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            // 4. Crear el índice de la clave foránea
            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_ConversationId",
                table: "ConversationMessages",
                column: "ConversationId");

            // 5. Establecer la relación entre mensajes y conversaciones
            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessages_Conversations_ConversationId",
                table: "ConversationMessages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessages_Conversations_ConversationId",
                table: "ConversationMessages");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_ConversationMessages_ConversationId",
                table: "ConversationMessages");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "ConversationMessages");
        }
    }
}
