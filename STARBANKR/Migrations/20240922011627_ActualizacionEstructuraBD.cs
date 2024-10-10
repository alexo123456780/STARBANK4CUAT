using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STARBANKR.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionEstructuraBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cuenta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    numeroTarjeta = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombreUsuario = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pin = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    saldo = table.Column<double>(type: "double", nullable: false),
                    intentosFallidos = table.Column<int>(type: "int", nullable: false),
                    tipocuenta = table.Column<int>(name: "tipo_cuenta", type: "int", nullable: false),
                    limitecredito = table.Column<double>(name: "limite_credito", type: "double", nullable: true),
                    fechacorte = table.Column<DateTime>(name: "fecha_corte", type: "datetime(6)", nullable: true),
                    fechapago = table.Column<DateTime>(name: "fecha_pago", type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuenta", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "servicio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numeroreferencia = table.Column<string>(name: "numero_referencia", type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicio", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transaccion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cuentaid = table.Column<int>(name: "cuenta_id", type: "int", nullable: false),
                    tipoOperacion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    monto = table.Column<double>(type: "double", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaccion", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaccion_cuenta_cuenta_id",
                        column: x => x.cuentaid,
                        principalTable: "cuenta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pago_servicio",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    cuentaid = table.Column<int>(name: "cuenta_id", type: "int", nullable: false),
                    servicioid = table.Column<int>(name: "servicio_id", type: "int", nullable: false),
                    monto = table.Column<double>(type: "double", nullable: false),
                    fechapago = table.Column<DateTime>(name: "fecha_pago", type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pago_servicio", x => x.id);
                    table.ForeignKey(
                        name: "FK_pago_servicio_cuenta_cuenta_id",
                        column: x => x.cuentaid,
                        principalTable: "cuenta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pago_servicio_servicio_servicio_id",
                        column: x => x.servicioid,
                        principalTable: "servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "comprobante",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transaccionid = table.Column<int>(name: "transaccion_id", type: "int", nullable: false),
                    contenido = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fechageneracion = table.Column<DateTime>(name: "fecha_generacion", type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comprobante", x => x.id);
                    table.ForeignKey(
                        name: "FK_comprobante_transaccion_transaccion_id",
                        column: x => x.transaccionid,
                        principalTable: "transaccion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_comprobante_transaccion_id",
                table: "comprobante",
                column: "transaccion_id");

            migrationBuilder.CreateIndex(
                name: "IX_pago_servicio_cuenta_id",
                table: "pago_servicio",
                column: "cuenta_id");

            migrationBuilder.CreateIndex(
                name: "IX_pago_servicio_servicio_id",
                table: "pago_servicio",
                column: "servicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaccion_cuenta_id",
                table: "transaccion",
                column: "cuenta_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comprobante");

            migrationBuilder.DropTable(
                name: "pago_servicio");

            migrationBuilder.DropTable(
                name: "transaccion");

            migrationBuilder.DropTable(
                name: "servicio");

            migrationBuilder.DropTable(
                name: "cuenta");
        }
    }
}
