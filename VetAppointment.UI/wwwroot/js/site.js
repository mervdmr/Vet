// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function HandleReturnModel(response) {
    alert(response.Message);
    if (response.RedirectURL) {
        window.location.href = response.RedirectURL;
    }
    if (response.RefreshPage) {
        window.location.reload();
    }
}