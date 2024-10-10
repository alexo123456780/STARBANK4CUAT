using Microsoft.AspNetCore.Mvc;
using STARBANKR.Models;
using STARBANKR.Services;

namespace STARBANKR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BancoController : ControllerBase
    {
        private readonly BancoService _bancoService;

        public BancoController(BancoService bancoService)
        {
            _bancoService = bancoService;
        }

        [HttpGet("cuenta/{numeroTarjeta}")]
        public async Task<ActionResult<Cuenta>> ObtenerCuenta(string numeroTarjeta)
        {
            var cuenta = await _bancoService.ObtenerCuentaPorNumeroTarjeta(numeroTarjeta);
            if (cuenta == null)
                return NotFound("Cuenta no encontrada");
            return cuenta;
        }

        [HttpPost("verificarPin")]
        public async Task<IActionResult> VerificarPin([FromBody] VerificacionPinDTO datos)
        {
            if (datos.Pin.Length != 4 || !int.TryParse(datos.Pin, out _))
                return BadRequest("El PIN debe ser de 4 dígitos");

            var resultado = await _bancoService.VerificarPin(datos.NumeroTarjeta, datos.Pin);
            if (resultado.Exitoso)
                return Ok("PIN correcto");
            else if (resultado.IntentosRestantes > 0)
                return BadRequest($"PIN incorrecto. Intentos restantes: {resultado.IntentosRestantes}");
            else
                return BadRequest("Cuenta bloqueada. Por favor, contacte con atención al cliente.");
        }

        [HttpPost("retiro")]
        public async Task<IActionResult> RealizarRetiro([FromBody] TransaccionDTO transaccion)
        {
            if (!EsDenominacionValida(transaccion.Monto))
                return BadRequest("El monto debe ser en denominaciones de 50, 100, 200, 500 o 1000 pesos");

            if (transaccion.Monto > 9000)
                return BadRequest("El monto máximo de retiro es de 9000 pesos");

            var resultado = await _bancoService.RealizarRetiro(transaccion.NumeroTarjeta, transaccion.Monto);
            if (!resultado)
                return BadRequest("No se pudo realizar el retiro. Verifique su saldo.");
            return Ok("Retiro realizado con éxito");
        }

        [HttpPost("deposito")]
        public async Task<IActionResult> RealizarDeposito([FromBody] TransaccionDTO transaccion)
        {
            if (transaccion.Monto <= 0)
                return BadRequest("El monto del depósito debe ser mayor que cero");

            var resultado = await _bancoService.RealizarDeposito(transaccion.NumeroTarjeta, transaccion.Monto);
            if (!resultado)
                return BadRequest("No se pudo realizar el depósito");
            return Ok("Depósito realizado con éxito");
        }

        [HttpPost("pagoTarjetaCredito")]
        public async Task<IActionResult> PagoTarjetaCredito([FromBody] PagoTarjetaCreditoDTO pago)
        {
            if (pago.Monto <= 0)
                return BadRequest("El monto del pago debe ser mayor que cero");

            var resultado = await _bancoService.RealizarPagoTarjetaCredito(pago.NumeroTarjeta, pago.Monto, pago.EsAbonoCapital);
            if (!resultado)
                return BadRequest("No se pudo realizar el pago. Verifique su saldo.");
            return Ok("Pago realizado con éxito");
        }

        [HttpPost("pagoServicio")]
        public async Task<IActionResult> PagoServicio([FromBody] PagoServicioDTO pago)
        {
            if (pago.Monto <= 0)
                return BadRequest("El monto del pago debe ser mayor que cero");

            var resultado = await _bancoService.RealizarPagoServicio(pago.NumeroTarjeta, pago.TipoServicio, pago.Monto);
            if (!resultado)
                return BadRequest("No se pudo realizar el pago del servicio. Verifique su saldo.");
            return Ok("Pago de servicio realizado con éxito");
        }

        [HttpPost("creditoEducativo")]
        public async Task<IActionResult> CreditoEducativo([FromBody] CreditoEducativoDTO credito)
        {
            if (credito.Monto <= 0 || credito.Plazo <= 0)
                return BadRequest("El monto y el plazo deben ser mayores que cero");

            var resultado = await _bancoService.SolicitarCreditoEducativo(credito.NumeroTarjeta, credito.Monto, credito.Plazo);
            if (!resultado)
                return BadRequest("No se pudo procesar la solicitud de crédito educativo");
            return Ok("Solicitud de crédito educativo procesada con éxito");
        }

        [HttpGet("transacciones/{numeroTarjeta}")]
        public async Task<ActionResult<List<Transaccion>>> ObtenerTransacciones(string numeroTarjeta, [FromQuery] string ordenarPor = "fecha", [FromQuery] bool ascendente = true)
        {
            var transacciones = await _bancoService.ObtenerTransacciones(numeroTarjeta, ordenarPor, ascendente);
            if (transacciones == null || !transacciones.Any())
                return NotFound("No se encontraron transacciones");
            return transacciones;
        }

        [HttpPost("imprimirComprobante")]
        public async Task<IActionResult> ImprimirComprobante([FromBody] ComprobanteDTO comprobante)
        {
            var resultado = await _bancoService.GenerarComprobante(comprobante.NumeroTarjeta, comprobante.TipoOperacion, comprobante.Monto);
            if (resultado == null)
                return BadRequest("No se pudo generar el comprobante");
            return Ok(resultado);
        }

        private bool EsDenominacionValida(double monto)
        {
            int[] denominacionesValidas = { 50, 100, 200, 500, 1000 };
            return denominacionesValidas.Contains((int)monto);
        }
    }
}