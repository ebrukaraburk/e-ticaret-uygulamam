(function () {
    var overlay = document.getElementById('confirmOverlay');
    var confirmIcon = document.getElementById('confirmIcon');
    var confirmMessage = document.getElementById('confirmMessage');
    var confirmOk = document.getElementById('confirmOk');
    var confirmCancel = document.getElementById('confirmCancel');

    var pendingAction = null;
    var pendingForm = null;

    var orderForm = document.getElementById('confirmOrderForm');
    var orderBtn = document.getElementById('confirmOrderBtn');
    var grandTotalEl = document.getElementById('basketGrandTotal');

    var removeButtons = document.querySelectorAll('.js-remove-confirm');

    function openOverlay(message, iconClass) {
        confirmMessage.textContent = message;
        confirmIcon.className = 'bi ' + iconClass + ' display-6';
        overlay.classList.add('show');
    }

    function closeOverlay() {
        overlay.classList.remove('show');
        pendingAction = null;
        pendingForm = null;
    }

    removeButtons.forEach(function (button) {
        button.addEventListener('click', function () {
            pendingForm = button.closest('form');
            pendingAction = 'remove';
            var productName = button.dataset.productName || 'bu ürün';
            openOverlay('"' + productName + '" ürününü sepetten kaldırmak istediğinize emin misiniz?', 'bi-trash text-danger');
        });
    });

    if (orderForm && orderBtn) {
        orderBtn.addEventListener('click', function (e) {
            e.preventDefault();
            pendingForm = orderForm;
            pendingAction = 'order';
            var totalText = grandTotalEl ? grandTotalEl.textContent.trim() : '';
            openOverlay('Siparişinizi oluşturmak istediğinize emin misiniz? Genel toplam: ' + totalText, 'bi-bag-check text-success');
        });
    }

    if (confirmCancel) {
        confirmCancel.addEventListener('click', closeOverlay);
    }
    if (overlay) {
        overlay.addEventListener('click', function (e) {
            if (e.target === overlay) closeOverlay();
        });
    }

    if (confirmOk) {
        confirmOk.addEventListener('click', function () {
            if (!pendingForm) {
                closeOverlay();
                return;
            }

            // closeOverlay() pendingForm'u null'ladığı için önce yerel değişkene al
            var formToSubmit = pendingForm;
            var action = pendingAction;
            closeOverlay();

            if (action === 'order') {
                orderBtn.classList.add('loading');
                orderBtn.querySelector('.btn-text').textContent = 'İşleniyor...';
                var spinner = document.createElement('span');
                spinner.className = 'spinner-border spinner-border-sm';
                spinner.setAttribute('role', 'status');
                orderBtn.prepend(spinner);
            }

            if (action === 'order' || action === 'remove') {
                formToSubmit.submit();
            }
        });
    }
})();