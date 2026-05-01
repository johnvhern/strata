// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(() => {
    const sidebarCloseButton = document.querySelector('.sidebar-offcanvas-close');

    if (!sidebarCloseButton) {
        return;
    }

    sidebarCloseButton.addEventListener('click', () => {
        const sidebar = document.getElementById('appSidebar');

        if (!sidebar || !window.bootstrap?.Offcanvas) {
            return;
        }

        window.bootstrap.Offcanvas.getOrCreateInstance(sidebar).hide();
    });
})();
