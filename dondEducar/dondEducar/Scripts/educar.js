/* ===================================================
 * educar.js 
 * =================================================== */
var EDUCAR = {
    /**
     * Verifica si el objeto es undefined, array vacío o string vacía.
     */
    emptyResult: function (data) {
        return (typeof data == "undefined" || data.length == 0 || data == "");
    },

    /**
     * Recarga la pagina actual
     */
    actualizarPagina: function () {
        location.reload();
    },

    /**
     * Cargar partials en modals
     */
    abrirUrlEnModal: function (url, data) {
        $.ajax({
            url: url,
            data: (typeof data !== "undefined") ? data : {},
            cache: false,
        }).done(function (response) {
            EDUCAR.abrirEnModal(response);
        }).fail(function (jqXhr, errorCode, textStatus) {
            alert("Error al hacer el pedido: " + textStatus + " (" + errorCode + ")");
        });
    },

    /**
     * Carga un html en el modal base que esta en el layout.
     */
    abrirEnModal: function (data) {
        $("#educarModal").html(data).modal("show");
    },

    enableBackToTop: function () {
        var backToTop = $('<a>', { id: 'back-to-top', href: '#top' });
        var icon = $('<i>', { class: 'icon-chevron-up' });

        backToTop.appendTo('body');
        icon.appendTo(backToTop);

        backToTop.hide();

        $(window).scroll(function () {
            if ($(this).scrollTop() > 150) {
                backToTop.fadeIn();
            } else {
                backToTop.fadeOut();
            }
        });

        backToTop.click(function (e) {
            e.preventDefault();

            $('body, html').animate({
                scrollTop: 0
            }, 600);
        });
    },

    init: function () {
        EDUCAR.enableBackToTop();
    }
};

/**
 * Manejo del LocalStorage
 */
EDUCAR.Storage = {

    disponible: (typeof (Storage) !== 'undefined'),

    limpiar: function () {
        if (EDUCAR.Storage.disponible)
            localStorage.clear();
    },

    set: function (key, val) {
        if (EDUCAR.Storage.disponible)
            localStorage.setItem(key, val);
    },

    get: function (key) {
        if (EDUCAR.Storage.disponible)
            return localStorage.getItem(key);
    }
};

/**
 * Mapeo de los tipos de operación
 */
EDUCAR.TipoDeEscuela = {
    Inicial: 1,
    Primaria: 2,
    Secundaria: 3
};

/**
 * Manejo de opciones generales de slickgrid
 */
EDUCAR.Grids = {
    Formatters: {
        baseFormatter: function (row, cell, value, columnDef, dataContext) {
            return value;
        },

        buttonFormatter: function (row, cell, value, columnDef, dataContext) {
            return '<a class="btn btn-small btn-info" href="#">' + value + '</a>';
        },

        checkFormatter: function (row, cell, value, columnDef, dataContext) {
            if (value == "false")
                return '<i class="icon-check-minus"></i>';
            return '<i class="icon-check"></i>';
        }
    },

    SlickGridOptions: {
        editable: false,
        enableAddRow: false,
        enableCellNavigation: false,
        autoHeight: true,
        forceFitColumns: true
    }
};

EDUCAR.Grids.ColumnasEscuelas = [
    { id: "grid-codigo", name: "Numero", field: "Codigo", sortable: true },
    { id: "grid-nombre", name: "Nombre", field: "Nombre", sortable: true },
    { id: "grid-imagen", name: "Imagen", field: "Imagen" },
    { id: "grid-direccion", name: "Direccion", field: "Direccion" },
    { id: "grid-telefonos", name: "Telefonos", field: "Telefonos" },
    { id: "grid-email", name: "Email", field: "Email" },
    { id: "grid-latitud", name: "Latitud", field: "Latitud" },
    { id: "grid-longitud", name: "Longitud", field: "Longitud" }
];


EDUCAR.Modals = {
    error: function (mensaje, titulo) {
        EDUCAR.abrirEnModal(EDUCAR.Modals.baseHTML.error.replace("@MENSAJE@", mensaje).replace("@TITULO@", titulo || "Error"));
    },
    baseHTML: {
        error: '<div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button><h3><span><i class="icon-exclamation-sign"></i> @TITULO@</span></h3></div><div class="modal-body"><p>@MENSAJE@</p></div><div class="modal-footer"><button class="btn pull-right" data-dismiss="modal" aria-hidden="true">Cerrar</button></div>'
    }
};


EDUCAR.init();
