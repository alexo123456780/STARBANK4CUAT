using Microsoft.EntityFrameworkCore;
using STARBANKR.Data;
using STARBANKR.Models;
using System.Net.Mail;
using System.Net;

namespace STARBANKR.Services
{
    public class BancoService
    {
        private readonly ApplicationDbContext _context;

        public BancoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cuenta> ObtenerCuentaPorNumeroTarjeta(string numeroTarjeta)
        {
            return await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroTarjeta == numeroTarjeta);
        }

        public async Task<ResultadoVerificacionPin> VerificarPin(string numeroTarjeta, string pin)
        {
            var cuenta = await ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            if (cuenta == null)
                return new ResultadoVerificacionPin { Exitoso = false, IntentosRestantes = 0 };

            if (cuenta.Pin == pin)
            {
                cuenta.IntentosFallidos = 0;
                await _context.SaveChangesAsync();
                return new ResultadoVerificacionPin { Exitoso = true, IntentosRestantes = 3 };
            }
            else
            {
                cuenta.IntentosFallidos++;
                await _context.SaveChangesAsync();
                return new ResultadoVerificacionPin { Exitoso = false, IntentosRestantes = 3 - cuenta.IntentosFallidos };
            }
        }

        public async Task<bool> RealizarRetiro(string numeroTarjeta, double monto)
        {
            var cuenta = await ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            if (cuenta == null || cuenta.Saldo < monto)
                return false;

            cuenta.Saldo -= monto;
            await RegistrarTransaccion(cuenta.Id, "Retiro", monto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RealizarDeposito(string numeroTarjeta, double monto)
        {
            var cuenta = await ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            if (cuenta == null)
                return false;

            cuenta.Saldo += monto;
            await RegistrarTransaccion(cuenta.Id, "Deposito", monto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RealizarPagoTarjetaCredito(string numeroTarjeta, double monto, bool esAbonoCapital)
        {
            var cuenta = await ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            if (cuenta == null || cuenta.TipoCuenta != TipoCuenta.Credito || cuenta.Saldo < monto)
                return false;

            cuenta.Saldo -= monto;
            string tipoOperacion = esAbonoCapital ? "Abono a Capital" : "Pago Tarjeta Crédito";
            await RegistrarTransaccion(cuenta.Id, tipoOperacion, monto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RealizarPagoServicio(string numeroTarjeta, string tipoServicio, double monto)
        {
            var cuenta = await ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Nombre == tipoServicio);
            if (cuenta == null || servicio == null || cuenta.Saldo < monto)
                return false;

            cuenta.Saldo -= monto;
            await RegistrarTransaccion(cuenta.Id, $"Pago Servicio {tipoServicio}", monto);
            await _context.PagosServicios.AddAsync(new PagoServicio
            {
                CuentaId = cuenta.Id,
                ServicioId = servicio.Id,
                Monto = monto,
                FechaPago = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SolicitarCreditoEducativo(string numeroTarjeta, double monto, int plazo)
        {
            var cuenta = await ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            if (cuenta == null)
                return false;

            // Aquí iría la lógica para evaluar la solicitud de crédito
            // Por ahora, simplemente registramos la solicitud como una transacción
            await RegistrarTransaccion(cuenta.Id, "Solicitud Crédito Educativo", monto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Transaccion>> ObtenerTransacciones(string numeroTarjeta, string ordenarPor = "fecha", bool ascendente = true)
        {
            var cuenta = await ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            if (cuenta == null)
                return null;

            var query = _context.Transacciones.Where(t => t.CuentaId == cuenta.Id);

            query = ordenarPor.ToLower() switch
            {
                "monto" => ascendente ? query.OrderBy(t => t.Monto) : query.OrderByDescending(t => t.Monto),
                _ => ascendente ? query.OrderBy(t => t.Fecha) : query.OrderByDescending(t => t.Fecha),
            };

            return await query.ToListAsync();
        }

        public async Task<string> GenerarComprobante(string numeroTarjeta, string tipoOperacion, double monto)
        {
            var cuenta = await ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            if (cuenta == null)
                return null;

            var transaccion = await RegistrarTransaccion(cuenta.Id, tipoOperacion, monto);
            var contenido = $"Comprobante de {tipoOperacion}\n" +
                            $"Fecha: {DateTime.Now}\n" +
                            $"Número de Tarjeta: {numeroTarjeta}\n" +
                            $"Monto: ${monto}\n" +
                            $"Nuevo Saldo: ${cuenta.Saldo}";

            var comprobante = new Comprobante
            {
                TransaccionId = transaccion.Id,
                Contenido = contenido,
                FechaGeneracion = DateTime.Now
            };

            await _context.Comprobantes.AddAsync(comprobante);
            await _context.SaveChangesAsync();

            await EnviarComprobanteEmail(cuenta.NombreUsuario, contenido);

            return contenido;
        }

        private async Task<Transaccion> RegistrarTransaccion(int cuentaId, string tipoOperacion, double monto)
        {
            var transaccion = new Transaccion
            {
                CuentaId = cuentaId,
                TipoOperacion = tipoOperacion,
                Monto = monto,
                Fecha = DateTime.Now
            };

            await _context.Transacciones.AddAsync(transaccion);
            await _context.SaveChangesAsync();
            return transaccion;
        }

        private async Task EnviarComprobanteEmail(string nombreUsuario, string contenidoComprobante)
        {
            var fromAddress = new MailAddress("alexitolope152004@gmail.com", "STARBANKR");
            var toAddress = new MailAddress("alexitolope152004@gmail.com", nombreUsuario);
            const string fromPassword = "evuw cpct zdbz auay";
            const string subject = "Comprobante de transacción";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = contenidoComprobante
            })
            {
                await smtp.SendMailAsync(message);
            }
        }
    }

    public class ResultadoVerificacionPin
    {
        public bool Exitoso { get; set; }
        public int IntentosRestantes { get; set; }
    }
}