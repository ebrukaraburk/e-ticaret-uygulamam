(function () {
    var searchInput = document.getElementById('orderSearch');
    var filterButtons = document.querySelectorAll('.filter-btn');
    var rows = document.querySelectorAll('#myOrdersTable tbody tr.order-row');
    var rowCountInfo = document.getElementById('rowCountInfo');
    var activeFilter = 'all';

    function applyFilters() {
        if (!rows.length) return;

        var term = (searchInput && searchInput.value || '').trim().toLowerCase();
        var visibleCount = 0;

        rows.forEach(function (row) {
            var matchesSearch = term === '' || row.dataset.search.indexOf(term) !== -1;
            var matchesFilter = activeFilter === 'all' || row.dataset.status === activeFilter;

            var visible = matchesSearch && matchesFilter;
            row.classList.toggle('hidden-row', !visible);
            if (visible) visibleCount++;
        });

        if (rowCountInfo) {
            rowCountInfo.textContent = rows.length
                ? visibleCount + ' / ' + rows.length + ' sipariş gösteriliyor'
                : '';
        }
    }

    if (searchInput) {
        searchInput.addEventListener('input', applyFilters);
    }

    filterButtons.forEach(function (btn) {
        btn.addEventListener('click', function () {
            filterButtons.forEach(function (b) { b.classList.remove('active'); });
            btn.classList.add('active');
            activeFilter = btn.dataset.filter;
            applyFilters();
        });
    });

    applyFilters();

    // --- Sipariş iptal onay modalı ---
    var overlay = document.getElementById('cancelConfirmOverlay');
    var message = document.getElementById('cancelConfirmMessage');
    var yesBtn = document.getElementById('cancelConfirmYes');
    var noBtn = document.getElementById('cancelConfirmNo');
    var pendingForm = null;

    document.querySelectorAll('.js-cancel-confirm').forEach(function (button) {
        button.addEventListener('click', function () {
            pendingForm = button.closest('form');
            var orderNumber = button.dataset.orderNumber || 'bu sipariş';
            message.textContent = '"' + orderNumber + '" numaralı siparişi iptal etmek istediğinize emin misiniz? Bu işlem geri alınamaz.';
            overlay.classList.add('show');
        });
    });

    function closeOverlay() {
        overlay.classList.remove('show');
        pendingForm = null;
    }

    if (noBtn) {
        noBtn.addEventListener('click', closeOverlay);
    }
    if (overlay) {
        overlay.addEventListener('click', function (e) {
            if (e.target === overlay) closeOverlay();
        });
    }
    if (yesBtn) {
        yesBtn.addEventListener('click', function () {
            if (pendingForm) {
                var formToSubmit = pendingForm;
                closeOverlay();
                formToSubmit.submit();
            } else {
                closeOverlay();
            }
        });
    }
})();