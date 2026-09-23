(function () {
    var searchInput = document.getElementById('orderSearch');
    var filterButtons = document.querySelectorAll('.filter-btn');
    var rows = document.querySelectorAll('#ordersTable tbody tr.order-row');
    var rowCountInfo = document.getElementById('rowCountInfo');
    var activeFilter = 'all';

    function applyFilters() {
        var term = (searchInput.value || '').trim().toLowerCase();
        var visibleCount = 0;

        rows.forEach(function (row) {
            var matchesSearch = term === '' || row.dataset.search.indexOf(term) !== -1;
            var status = row.dataset.status;
            var matchesFilter =
                activeFilter === 'all' ||
                (activeFilter === 'Beklemede' && status !== 'Onaylandı' && status !== 'Reddedildi') ||
                status === activeFilter;

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

    // Onay/reddet işlemleri için özel onay kutusu
    var overlay = document.getElementById('confirmOverlay');
    var confirmText = document.getElementById('confirmText');
    var confirmOk = document.getElementById('confirmOk');
    var confirmCancel = document.getElementById('confirmCancel');
    var pendingForm = null;
    var pendingButtonName = null;
    var pendingButtonValue = null;

    document.querySelectorAll('.js-confirm').forEach(function (button) {
        button.addEventListener('click', function (e) {
            e.preventDefault();
            pendingForm = button.closest('form');
            pendingButtonName = button.name;
            pendingButtonValue = button.value;
            confirmText.textContent = button.dataset.confirmText || 'Emin misiniz?';
            overlay.classList.add('show');
        });
    });

    function closeOverlay() {
        overlay.classList.remove('show');
        pendingForm = null;
        pendingButtonName = null;
        pendingButtonValue = null;
    }

    confirmCancel.addEventListener('click', closeOverlay);
    overlay.addEventListener('click', function (e) {
        if (e.target === overlay) closeOverlay();
    });

    confirmOk.addEventListener('click', function () {
        if (pendingForm) {
            var hidden = document.createElement('input');
            hidden.type = 'hidden';
            hidden.name = pendingButtonName;
            hidden.value = pendingButtonValue;
            pendingForm.appendChild(hidden);
            pendingForm.submit();
        }
        closeOverlay();
    });
})();