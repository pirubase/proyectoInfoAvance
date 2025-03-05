// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ajaxError(function(event, jqXHR, ajaxSettings, thrownError) {
    if (jqXHR.status === 401) {
        window.location.href = '/Login/Index';
    }
});
