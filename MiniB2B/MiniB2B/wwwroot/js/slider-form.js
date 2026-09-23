(function () {
    "use strict";

    var dropArea = document.querySelector(".image-drop");
    var fileInput = document.querySelector('input[name="imageFile"]');
    var previewWrap = document.querySelector(".slide-preview");
    var fileNameEl = document.querySelector(".image-drop-text .file-name");
    var titleInput = document.getElementById("Title");
    var descInput = document.getElementById("Description");
    var previewTitle = document.querySelector(".slide-preview-caption h3");
    var previewDesc = document.querySelector(".slide-preview-caption p");

    function renderImage(url) {
        if (!previewWrap) return;
        previewWrap.innerHTML =
            '<div class="slide-preview-blur" style="background-image:url(\'' + url + '\')"></div>' +
            '<img src="' + url + '" class="slide-preview-img" alt="Önizleme" />' +
            '<div class="slide-preview-caption">' +
            '<h3></h3><p></p>' +
            '</div>';
        syncCaption();
    }

    function syncCaption() {
        var h3 = document.querySelector(".slide-preview-caption h3");
        var p = document.querySelector(".slide-preview-caption p");
        if (h3) h3.textContent = (titleInput && titleInput.value.trim()) || "Başlık burada görünecek";
        if (p) p.textContent = (descInput && descInput.value.trim()) || "";
    }

    // Sayfa açıldığında mevcut görsel varsa (düzenleme senaryosu) önizlemeyi kur
    var existingImg = previewWrap && previewWrap.querySelector(".slide-preview-img");
    if (existingImg) {
        syncCaption();
    }

    if (fileInput) {
        fileInput.addEventListener("change", function () {
            var file = fileInput.files && fileInput.files[0];
            if (!file) return;

            if (fileNameEl) fileNameEl.textContent = file.name;

            if (file.type && file.type.indexOf("image/") === 0) {
                var reader = new FileReader();
                reader.onload = function (e) { renderImage(e.target.result); };
                reader.readAsDataURL(file);
            }
        });
    }

    if (dropArea) {
        ["dragenter", "dragover"].forEach(function (evt) {
            dropArea.addEventListener(evt, function (e) {
                e.preventDefault();
                dropArea.classList.add("is-dragover");
            });
        });

        ["dragleave", "drop"].forEach(function (evt) {
            dropArea.addEventListener(evt, function (e) {
                e.preventDefault();
                dropArea.classList.remove("is-dragover");
            });
        });

        dropArea.addEventListener("drop", function (e) {
            var files = e.dataTransfer && e.dataTransfer.files;
            if (files && files[0] && fileInput) {
                fileInput.files = files;
                fileInput.dispatchEvent(new Event("change"));
            }
        });
    }

    if (titleInput) titleInput.addEventListener("input", syncCaption);
    if (descInput) descInput.addEventListener("input", syncCaption);

    // Kaydet butonuna basıldığında çift gönderimi engelle
    var form = document.querySelector(".slider-form-card form");
    var saveBtn = document.querySelector(".btn-save");
    if (form && saveBtn) {
        form.addEventListener("submit", function () {
            if (form.checkValidity && !form.checkValidity()) {
                return;
            }
            saveBtn.setAttribute("disabled", "disabled");
            saveBtn.textContent = "Kaydediliyor...";
        });
    }
})();