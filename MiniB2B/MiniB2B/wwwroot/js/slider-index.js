(function () {
    "use strict";

    // Silme linklerine tıklandığında onay kutusu göster
    document.querySelectorAll(".btn-delete-slide").forEach(function (link) {
        link.addEventListener("click", function (e) {
            var title = link.getAttribute("data-title") || "bu kaydı";
            var confirmed = window.confirm('"' + title + '" silinsin mi? Bu işlem geri alınamaz.');
            if (!confirmed) {
                e.preventDefault();
            }
        });
    });
})();