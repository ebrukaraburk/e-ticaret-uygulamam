document.addEventListener('DOMContentLoaded', function () {
    var input = document.getElementById('qtyInput');

    var unitPrice, max, totalEl;
    if (input) {
        unitPrice = parseFloat(input.dataset.unitPrice);
        max = parseInt(input.max) || 1;
        totalEl = document.getElementById('totalPrice');
    }

    (function () {
        var toastEl = document.getElementById('mesajToast');
        if (toastEl) {
            requestAnimationFrame(function () {
                toastEl.classList.add('show');
            });

            setTimeout(function () {
                toastEl.classList.remove('show');
                setTimeout(function () {
                    toastEl.remove();
                }, 250); // CSS transition süresi kadar bekle, sonra DOM'dan kaldır
            }, 4000);
        }
    })();

    if (!input) return; // Admin ya da stokta yoksa adet formu yok

    function clamp() {
        var v = parseInt(input.value);
        if (isNaN(v) || v < 1) v = 1;
        if (v > max) v = max;
        input.value = v;

        totalEl.textContent = (v * unitPrice).toLocaleString('tr-TR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }) + ' ₺';
    }

    document.getElementById('qtyMinus').addEventListener('click', function () {
        input.value = (parseInt(input.value) || 1) - 1;
        clamp();
    });

    document.getElementById('qtyPlus').addEventListener('click', function () {
        input.value = (parseInt(input.value) || 1) + 1;
        clamp();
    });

    input.addEventListener('input', clamp);
    input.addEventListener('change', clamp);
});