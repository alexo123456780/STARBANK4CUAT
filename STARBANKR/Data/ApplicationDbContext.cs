using Microsoft.EntityFrameworkCore;
using STARBANKR.Models;

namespace STARBANKR.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<PagoServicio> PagosServicios { get; set; }
        public DbSet<Comprobante> Comprobantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cuenta>().ToTable("cuenta");
            modelBuilder.Entity<Cuenta>().Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Cuenta>().Property(c => c.NumeroTarjeta).HasColumnName("numeroTarjeta");
            modelBuilder.Entity<Cuenta>().Property(c => c.NombreUsuario).HasColumnName("nombreUsuario");
            modelBuilder.Entity<Cuenta>().Property(c => c.Pin).HasColumnName("pin");
            modelBuilder.Entity<Cuenta>().Property(c => c.Saldo).HasColumnName("saldo");
            modelBuilder.Entity<Cuenta>().Property(c => c.IntentosFallidos).HasColumnName("intentosFallidos");
            modelBuilder.Entity<Cuenta>().Property(c => c.TipoCuenta).HasColumnName("tipo_cuenta");
            modelBuilder.Entity<Cuenta>().Property(c => c.LimiteCredito).HasColumnName("limite_credito");
            modelBuilder.Entity<Cuenta>().Property(c => c.FechaCorte).HasColumnName("fecha_corte");
            modelBuilder.Entity<Cuenta>().Property(c => c.FechaPago).HasColumnName("fecha_pago");

            modelBuilder.Entity<Transaccion>().ToTable("transaccion");
            modelBuilder.Entity<Transaccion>().Property(t => t.Id).HasColumnName("id");
            modelBuilder.Entity<Transaccion>().Property(t => t.CuentaId).HasColumnName("cuenta_id");
            modelBuilder.Entity<Transaccion>().Property(t => t.TipoOperacion).HasColumnName("tipoOperacion");
            modelBuilder.Entity<Transaccion>().Property(t => t.Monto).HasColumnName("monto");
            modelBuilder.Entity<Transaccion>().Property(t => t.Fecha).HasColumnName("fecha");

            modelBuilder.Entity<Servicio>().ToTable("servicio");
            modelBuilder.Entity<Servicio>().Property(s => s.Id).HasColumnName("id");
            modelBuilder.Entity<Servicio>().Property(s => s.Nombre).HasColumnName("nombre");
            modelBuilder.Entity<Servicio>().Property(s => s.NumeroReferencia).HasColumnName("numero_referencia");

            modelBuilder.Entity<PagoServicio>().ToTable("pago_servicio");
            modelBuilder.Entity<PagoServicio>().Property(p => p.Id).HasColumnName("id");
            modelBuilder.Entity<PagoServicio>().Property(p => p.CuentaId).HasColumnName("cuenta_id");
            modelBuilder.Entity<PagoServicio>().Property(p => p.ServicioId).HasColumnName("servicio_id");
            modelBuilder.Entity<PagoServicio>().Property(p => p.Monto).HasColumnName("monto");
            modelBuilder.Entity<PagoServicio>().Property(p => p.FechaPago).HasColumnName("fecha_pago");

            modelBuilder.Entity<Comprobante>().ToTable("comprobante");
            modelBuilder.Entity<Comprobante>().Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Comprobante>().Property(c => c.TransaccionId).HasColumnName("transaccion_id");
            modelBuilder.Entity<Comprobante>().Property(c => c.Contenido).HasColumnName("contenido");
            modelBuilder.Entity<Comprobante>().Property(c => c.FechaGeneracion).HasColumnName("fecha_generacion");
        }
    }
}