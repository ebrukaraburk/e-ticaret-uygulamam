(function () {
    "use strict";

    var wrapper = document.querySelector(".wrapper");
    var toggleBtn = document.getElementById("sidebarToggle");

    if (!wrapper || !toggleBtn) {
        return;
    }

    var STORAGE_KEY = "minib2b.sidebarCollapsed";
    var isMobile = function () {
        return window.matchMedia("(max-width: 991.98px)").matches;
    };

    // Masaüstünde son tercih hatırlansın
    if (!isMobile() && localStorage.getItem(STORAGE_KEY) === "1") {
        wrapper.classList.add("sidebar-collapsed");
    }

    function toggleSidebar() {
        if (isMobile()) {
            wrapper.classList.toggle("sidebar-open");
        } else {
            wrapper.classList.toggle("sidebar-collapsed");
            localStorage.setItem(
                STORAGE_KEY,
                wrapper.classList.contains("sidebar-collapsed") ? "1" : "0"
            );
        }
    }

    toggleBtn.addEventListener("click", toggleSidebar);

    // Mobilde bir menü linkine tıklayınca sidebar otomatik kapansın
    document.querySelectorAll("#sidebar .sidebar-link").forEach(function (link) {
        link.addEventListener("click", function () {
            if (isMobile()) {
                wrapper.classList.remove("sidebar-open");
            }
        });
    });

    // Karartılmış arka plana tıklayınca sidebar kapansın (mobil)
    var backdrop = document.querySelector(".sidebar-backdrop");
    if (backdrop) {
        backdrop.addEventListener("click", function () {
            wrapper.classList.remove("sidebar-open");
        });
    }

    // Ekran boyutu masaüstüne geçtiğinde mobil "open" durumunu temizle
    window.addEventListener("resize", function () {
        if (!isMobile()) {
            wrapper.classList.remove("sidebar-open");
        }
    });
})();