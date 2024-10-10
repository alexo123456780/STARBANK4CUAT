const apiUrl = 'http://localhost:5267/api/Banco';
let numeroTarjetaActual = '';
let nombreUsuario = '';
let transaccionesRealizadas = 0;

function mostrarNotificacion(mensaje, tipo) {
    const notificacion = document.getElementById('notificacion');
    notificacion.textContent = mensaje;
    notificacion.className = `notification ${tipo}`;
    notificacion.style.display = 'block';
    setTimeout(() => {
        notificacion.style.display = 'none';
    }, 3000);
}

function verificarPin() {
    const numeroTarjeta = document.getElementById('numeroTarjeta').value;
    const pin = document.getElementById('pin').value;

    if (!numeroTarjeta || !pin || pin.length !== 4 || isNaN(pin)) {
        mostrarNotificacion("El PIN debe ser un número de 4 dígitos.", "error");
        return;
    }

    fetch(`${apiUrl}/verificarPin`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NumeroTarjeta: numeroTarjeta, Pin: pin })
    })
    .then(response => {
        if (response.ok) {
            numeroTarjetaActual = numeroTarjeta;
            obtenerNombreUsuario(numeroTarjeta);
            transaccionesRealizadas = 0;
        } else {
            return response.text().then(text => {
                throw new Error(text);
            });
        }
    })
    .catch(error => mostrarNotificacion(error.message, 'error'));
}

function obtenerNombreUsuario(numeroTarjeta) {
    fetch(`${apiUrl}/cuenta/${numeroTarjeta}`)
        .then(response => response.json())
        .then(data => {
            nombreUsuario = data.nombreUsuario;
            document.getElementById('nombreUsuario').textContent = nombreUsuario;
            mostrarSeccion('bienvenida');
        })
        .catch(error => mostrarNotificacion('Error al obtener el nombre del usuario', 'error'));
}

function cerrarSesion() {
    numeroTarjetaActual = '';
    nombreUsuario = '';
    transaccionesRealizadas = 0;
    mostrarSeccion('login');
    document.getElementById('numeroTarjeta').value = '';
    document.getElementById('pin').value = '';
    mostrarNotificacion('Sesión cerrada exitosamente', 'success');
}

function mostrarSaldo() {
    fetch(`${apiUrl}/cuenta/${numeroTarjetaActual}`)
        .then(response => response.json())
        .then(data => {
            document.getElementById('saldoActual').textContent = `$${data.saldo.toFixed(2)}`;
            mostrarSeccion('saldo');
        })
        .catch(error => mostrarNotificacion('Error al obtener el saldo', 'error'));
}

function realizarRetiro() {
    if (transaccionesRealizadas >= 5) {
        mostrarNotificacion("Has alcanzado el límite de 5 transacciones por sesión", "error");
        return;
    }

    const monto = document.getElementById('montoRetiro').value;

    if (!monto || isNaN(monto) || monto <= 0) {
        mostrarNotificacion("El monto debe ser un número positivo", "error");
        return;
    }

    if (monto > 9000) {
        mostrarNotificacion("El monto máximo de retiro es de 9000 pesos", "error");
        return;
    }

    if (![50, 100, 200, 500, 1000].includes(parseInt(monto))) {
        mostrarNotificacion("El monto debe ser en denominaciones de 50, 100, 200, 500 o 1000 pesos", "error");
        return;
    }

    realizarTransaccion('retiro', monto);
}

function realizarDeposito() {
    if (transaccionesRealizadas >= 5) {
        mostrarNotificacion("Has alcanzado el límite de 5 transacciones por sesión", "error");
        return;
    }

    const monto = document.getElementById('montoDeposito').value;

    if (!monto || isNaN(monto) || monto <= 0) {
        mostrarNotificacion("El monto del depósito debe ser mayor que cero", "error");
        return;
    }

    realizarTransaccion('deposito', monto);
}

function realizarPagoTarjeta() {
    if (transaccionesRealizadas >= 5) {
        mostrarNotificacion("Has alcanzado el límite de 5 transacciones por sesión", "error");
        return;
    }

    const monto = document.getElementById('montoPagoTarjeta').value;
    const esAbonoCapital = document.getElementById('esAbonoCapital').checked;

    if (!monto || isNaN(monto) || monto <= 0) {
        mostrarNotificacion("El monto del pago debe ser mayor que cero", "error");
        return;
    }

    fetch(`${apiUrl}/pagoTarjetaCredito`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NumeroTarjeta: numeroTarjetaActual, Monto: parseFloat(monto), EsAbonoCapital: esAbonoCapital })
    })
    .then(response => response.text())
    .then(result => {
        mostrarNotificacion(result, 'success');
        transaccionesRealizadas++;
        volverOperaciones();
    })
    .catch(error => mostrarNotificacion('Error al realizar el pago de tarjeta', 'error'));
}

function realizarPagoServicio() {
    if (transaccionesRealizadas >= 5) {
        mostrarNotificacion("Has alcanzado el límite de 5 transacciones por sesión", "error");
        return;
    }

    const tipoServicio = document.getElementById('tipoServicio').value;
    const monto = document.getElementById('montoPagoServicio').value;

    if (!tipoServicio || !monto || isNaN(monto) || monto <= 0) {
        mostrarNotificacion("Seleccionar un servicio y el monto debe ser mayor que cero", "error");
        return;
    }

    fetch(`${apiUrl}/pagoServicio`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NumeroTarjeta: numeroTarjetaActual, TipoServicio: tipoServicio, Monto: parseFloat(monto) })
    })
    .then(response => response.text())
    .then(result => {
        mostrarNotificacion(result, 'success');
        transaccionesRealizadas++;
        volverOperaciones();
    })
    .catch(error => mostrarNotificacion('Error al realizar el pago del servicio', 'error'));
}

function solicitarCreditoEducativo() {
    if (transaccionesRealizadas >= 5) {
        mostrarNotificacion("Has alcanzado el límite de 5 transacciones por sesión", "error");
        return;
    }

    const monto = document.getElementById('montoCredito').value;
    const plazo = document.getElementById('plazoCredito').value;
    
    if (!monto || isNaN(monto) || monto <= 0 || !plazo || isNaN(plazo) || plazo <= 0) {
        mostrarNotificacion("El monto y el plazo deben ser mayores que cero", "error");
        return;
    }
    
    fetch(`${apiUrl}/creditoEducativo`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NumeroTarjeta: numeroTarjetaActual, Monto: parseFloat(monto), Plazo: parseInt(plazo) })
    })
    .then(response => response.text())
    .then(result => {
        mostrarNotificacion(result, 'success');
        transaccionesRealizadas++;
        volverOperaciones();
    })
    .catch(error => mostrarNotificacion('Error al solicitar el crédito educativo', 'error'));
}

function cargarTransacciones() {
    const ordenarPor = document.getElementById('ordenTransacciones').value;
    const ascendente = document.getElementById('ordenAscendente').checked;
    
    fetch(`${apiUrl}/transacciones/${numeroTarjetaActual}?ordenarPor=${ordenarPor}&ascendente=${ascendente}`)
        .then(response => response.json())
        .then(transacciones => {
            const lista = document.getElementById('listaTransacciones');
            lista.innerHTML = '';
            transacciones.forEach(t => {
                const li = document.createElement('li');
                li.textContent = `${new Date(t.fecha).toLocaleString()}: ${t.tipoOperacion} - $${t.monto.toFixed(2)}`;
                lista.appendChild(li);
            });
        })
        .catch(error => mostrarNotificacion('Error al cargar las transacciones', 'error'));
}

function realizarTransaccion(tipo, monto) {
    fetch(`${apiUrl}/${tipo}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NumeroTarjeta: numeroTarjetaActual, Monto: parseFloat(monto) })
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => {
                throw new Error(text);
            });
        }
        return response.text();
    })
    .then(result => {
        mostrarNotificacion(result, 'success');
        transaccionesRealizadas++;
        volverOperaciones();
    })
    .catch(error => mostrarNotificacion(`Error al realizar ${tipo}`, 'error'));
}

function imprimirComprobante() {
    if (transaccionesRealizadas >= 5) {
        mostrarNotificacion("Has alcanzado el límite de 5 transacciones por sesión", "error");
        return;
    }

    const tipoOperacion = document.getElementById('tipoOperacionComprobante').value;
    const monto = document.getElementById('montoComprobante').value;

    if (!tipoOperacion || !monto || isNaN(monto) || monto <= 0) {
        mostrarNotificacion("Seleccione un tipo de operación y un monto válido", "error");
        return;
    }

    fetch(`${apiUrl}/imprimirComprobante`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ NumeroTarjeta: numeroTarjetaActual, TipoOperacion: tipoOperacion, Monto: parseFloat(monto) })
    })
    .then(response => response.text())
    .then(result => {
        const ventanaImpresion = window.open('', '_blank');
        ventanaImpresion.document.write('<html><head><title>Comprobante</title></head><body>');
        ventanaImpresion.document.write('<pre>' + result + '</pre>');
        ventanaImpresion.document.write('</body></html>');
        ventanaImpresion.document.close();
        ventanaImpresion.print();
        mostrarNotificacion("Comprobante generado y enviado por correo electrónico", 'success');
        transaccionesRealizadas++;
    })
    .catch(error => mostrarNotificacion('Error al generar el comprobante', 'error'));
}

function mostrarSeccion(seccion) {
    const secciones = ['login', 'bienvenida', 'operaciones', 'saldo', 'retiro', 'deposito', 'pagoTarjeta', 'pagoServicio', 'creditoEducativo', 'transacciones', 'comprobante'];
    secciones.forEach(s => {
        document.getElementById(s).style.display = s === seccion ? 'block' : 'none';
    });
}

function volverOperaciones() {
    mostrarSeccion('operaciones');
}

// Funciones para mostrar secciones específicas
function mostrarRetiro() { mostrarSeccion('retiro'); }
function mostrarDeposito() { mostrarSeccion('deposito'); }
function mostrarPagoTarjeta() { mostrarSeccion('pagoTarjeta'); }
function mostrarPagoServicio() { mostrarSeccion('pagoServicio'); }
function mostrarCreditoEducativo() { mostrarSeccion('creditoEducativo'); }
function mostrarTransacciones() {
    mostrarSeccion('transacciones');
    cargarTransacciones();
}
function mostrarComprobante() { mostrarSeccion('comprobante'); }