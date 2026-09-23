(function () {
    "use strict";

    var dropArea = document.querySelector(".image-drop");
    var fileInput = document.querySelector('input[name="resimDosyasi"]');
    var previewBox = document.querySelector(".image-preview");
    var fileNameEl = document.querySelector(".image-drop-text .file-name");

    if (!dropArea || !fileInput || !previewBox) {
        return;
    }

    function showPreview(file) {
        if (!file) {
            return;
        }

        if (fileNameEl) {
            fileNameEl.textContent = file.name;
        }

        if (!file.type || file.type.indexOf("image/") !== 0) {
            return;
        }

        var reader = new FileReader();
        reader.onload = function (e) {
            previewBox.innerHTML = '<img src="' + e.target.result + '" alt="Seçilen görsel önizlemesi" />';
        };
        reader.readAsDataURL(file);
    }

    fileInput.addEventListener("change", function () {
        if (fileInput.files && fileInput.files[0]) {
            showPreview(fileInput.files[0]);
        }
    });

    // Sürükle-bırak desteği
    ["dragenter", "dragover"].forEach(function (evtName) {
        dropArea.addEventListener(evtName, function (e) {
            e.preventDefault();
            dropArea.classList.add("is-dragover");
        });
    });

    ["dragleave", "drop"].forEach(function (evtName) {
        dropArea.addEventListener(evtName, function (e) {
            e.preventDefault();
            dropArea.classList.remove("is-dragover");
        });
    });

    dropArea.addEventListener("drop", function (e) {
        var files = e.dataTransfer && e.dataTransfer.files;
        if (files && files[0]) {
            fileInput.files = files;
            showPreview(files[0]);
        }
    });

    // Kaydet butonuna basıldığında çift gönderimi engelle
    var form = document.querySelector("form[asp-action='Create'], .product-form-card form");
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