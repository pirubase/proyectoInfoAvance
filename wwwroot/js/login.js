$(document).ready(function () {

    // ==================== RECUPERACION DE CLAVE ==================== //
    $('#openModal').on('click', function () {
        abrirModal();
    });
    $('#close').on('click', function () {
        cerrarModal();
    });
    $('#botonAceptar').on('click', async function () {
        enviarCredencialesAlCorreo();
    });
    $('#botonCancelar').on('click', async function () {
        cerrarModal();
    });
    // ============================================================== //

    // ==================== SUBCRIPCION ============================= //
    $('#openModalSub').on('click', function () {
        abrirModalSub();
    });
    $('#closeSub').on('click', function () {
        cerrarModalSub();
    });

    // Deprecado
    $('#botonAceptarSub').on('click', async function () {
        //================
        // EN CONSTRUCCION
        //================

        let correo = document.getElementById('correo').value;

        // Validacion de campos obligatorios
        if (validaCamposObligatorios()) {
            alert('Formulario correctamente diligenciado.');
            if (validarCorreoElectronico(correo)) {
                alert('Correo electronico valido.');
            }
            else {
                alert('Correo electronico invalido.');
            }
        }


    });
    $('#botonCancelarSub').on('click', async function () {
        cerrarModalSub();
    });
    // ============================================================== //

    /**
     * Funcion que valida los campos obligatorios de la vista.
     * Campos obligatorios:
     * - nit
     * - razonSocial
     * - tipoDocumento
     * - identificacion
     * - correo
     * - celular
     */
    function validaCamposObligatorios() {
        let nit = document.getElementById('nit').value;
        let razonSocial = document.getElementById('razonSocial').value;
        let tipoDocumento = document.getElementById('tipoDocumento').value;
        let identificacion = document.getElementById('identificacion').value;
        let correo = document.getElementById('correo').value;
        let celular = document.getElementById('celular').value;

        if (nit == '' || razonSocial == '' || tipoDocumento == '' || identificacion == '' || correo == '' || celular == '') {
            alert('Todos los campos son obligatorios.');
            return false;
        }
        return true;
    }

    function abrirModal() {
        document.getElementById('modalContainer').style.display = 'block';
    }
    function cerrarModal() {
        document.getElementById('modalContainer').style.display = 'none';
    }

    function abrirModalSub() {
        document.getElementById('modalContainerSub').style.display = 'block';
    }
    function cerrarModalSub() {
        document.getElementById('modalContainerSub').style.display = 'none';
    }

    function enviarCredencialesAlCorreo() {
        let correo = document.getElementById('correo_electronico').value;

        // Validacion campo obligatorio
        if (correo != undefined && correo != '') {
            if (validarCorreoElectronico(correo)) {
                let datos = {
                    "destinatario": correo,
                    "asunto": "Recuperacion de clave Doctor plus",
                    "mensaje": ``,
                }

                let resultadoEnvioCorreo = "";

                // Llamado a la API
                $.ajax({
                    url: '/api/APIGenerica/enviarCorreoRecuperacionClave',
                    type: 'post',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: JSON.stringify(datos),
                    success: function (response) {
                        if (response.success) {
                            bootbox.alert(response.message);
                        } else {
                            bootbox.alert("Error: " + response.message);
                        }
                    },
                    error: function (xhr) {
                        const message = xhr.responseJSON?.message || "Error inesperado al procesar la solicitud.";
                        bootbox.alert(message);
                    }
                });

                setTimeout(function () {
                    cerrarModal();
                }, 4000);
            } else {
                bootbox.alert({
                    message: "No es un correo válido.",
                    backdrop: true
                });
            }
        } else {
            bootbox.alert({
                message: "El campo correo electrónico es obligatorio para recuperar clave.",
                backdrop: true
            });
        }
    }

    // Funcion valida que un texto sea realmente un correo electronico
    function validarCorreoElectronico(correo) {
        let expresion = /\w+@\w+\.+[a-z]/;
        let resultado = expresion.test(correo);
        console.log("validarCorreoElectronico", resultado);
        return resultado;
    }

}
);





//document.addEventListener('DOMContentLoaded', function () {
//    var btn = document.getElementById('openModal');
//    if (btn) {
//        btn.addEventListener('click', function () {
//            document.getElementById('modalBody').innerHTML = "<p>prueba..</p>";
//            document.getElementById('modalContainer').style.display = 'block';
//        });
//    }

//    var closeBtn = document.getElementsByClassName('close')[0];
//    if (closeBtn) {
//        closeBtn.addEventListener('click', function () {
//            document.getElementById('modalContainer').style.display = 'none';
//        });
//    }

//    window.addEventListener('click', function (event) {
//        var modal = document.getElementById('modalContainer');
//        if (event.target === modal) {
//            modal.style.display = 'none';
//        }
//    });
//});

//function recuperarClave() {



//}





