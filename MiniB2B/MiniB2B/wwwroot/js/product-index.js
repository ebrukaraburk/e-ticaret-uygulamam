document.addEventListener('DOMContentLoaded', function () {
    var table = document.getElementById('piTable');
    var tbody = table ? table.querySelector('tbody') : null;
    var countEl = document.getElementById('piCount');
    var noResult = document.getElementById('piNoResult');
    var chips = document.querySelectorAll('#piChips .pi-chip');
    var currentFilter = 'all';

    function getRows() {
        return tbody ? Array.prototype.slice.call(tbody.querySelectorAll('tr.pi-row')) : [];
    }

    // ---------- Stok filtresi ----------
    function applyFilter() {
        var visible = 0;
        getRows().forEach(function (row) {
            var show = currentFilter === 'all' || row.dataset.stock === currentFilter;
            row.hidden = !show;
            if (show) visible++;
        });
        if (countEl) countEl.textContent = visible + ' ürün gösteriliyor';
        if (noResult) noResult.hidden = visible !== 0;
    }

    chips.forEach(function (chip) {
        chip.addEventListener('click', function () {
            chips.forEach(function (c) { c.classList.remove('active'); });
            chip.classList.add('active');
            currentFilter = chip.dataset.filter;
            applyFilter();
        });
    });

    // ---------- Sütuna göre sıralama ----------
    if (table) {
        var headers = table.querySelectorAll('th.pi-sortable');

        headers.forEach(function (th) {
            th.addEventListener('click', function () {
                var idx = th.cellIndex;
                var isNumber = th.dataset.type === 'number';
                var dir = th.classList.contains('asc') ? 'desc' : 'asc';

                headers.forEach(function (h) { h.classList.remove('asc', 'desc'); });
                th.classList.add(dir);

                var rows = getRows();
                rows.sort(function (a, b) {
                    var va = a.cells[idx].dataset.value || '';
                    var vb = b.cells[idx].dataset.value || '';
                    var result;

                    if (isNumber) {
                        result = (parseFloat(va) || 0) - (parseFloat(vb) || 0);
                    } else {
                        result = va.localeCompare(vb, 'tr');
                    }
                    return dir === 'asc' ? result : -result;
                });

                rows.forEach(function (row) { tbody.insertBefore(row, noResult); });
            });
        });
    }

    // ---------- Görsel önizleme modalı ----------
    var imgModal = document.getElementById('imgModal');
    if (imgModal) {
        imgModal.addEventListener('show.bs.modal', function (e) {
            var btn = e.relatedTarget;
            if (!btn) return;
            document.getElementById('imgModalImg').src = btn.dataset.img;
            document.getElementById('imgModalImg').alt = btn.dataset.title || '';
            document.getElementById('imgModalTitle').textContent = btn.dataset.title || '';
        });
        imgModal.addEventListener('hidden.bs.modal', function () {
            document.getElementById('imgModalImg').src = '';
        });
    }

    // ---------- "Öne çıkarmadan kaldır" onay modalı (sadece admin) ----------
    var removeModal = document.getElementById('removeFeaturedModal');
    if (removeModal) {
        removeModal.addEventListener('show.bs.modal', function (e) {
            var btn = e.relatedTarget;
            if (!btn) return;
            document.getElementById('removeProductId').value = btn.dataset.productId;
            document.getElementById('removeProductName').textContent = btn.dataset.productName;
        });
    }

    // ---------- "/" tuşu ile aramaya odaklan ----------
    var search = document.getElementById('piSearch');
    document.addEventListener('keydown', function (e) {
        var tag = (e.target.tagName || '').toLowerCase();
        if (e.key === '/' && tag !== 'input' && tag !== 'textarea' && search) {
            e.preventDefault();
            search.focus();
            search.select();
        }
    });
});