document.addEventListener('DOMContentLoaded', function () {
    var table = document.getElementById('umTable');
    if (!table) return;

    var tbody = table.querySelector('tbody');
    var countEl = document.getElementById('umCount');
    var noResult = document.getElementById('umNoResult');
    var search = document.getElementById('umSearch');
    var clearBtn = document.getElementById('umSearchClear');
    var chips = document.querySelectorAll('#umChips .um-chip');

    var currentFilter = 'all';

    function getRows() {
        return Array.prototype.slice.call(tbody.querySelectorAll('tr.um-row'));
    }

    // Türkçe karakterlere uygun küçük harfe çevirme
    function norm(s) {
        return (s || '').toLocaleLowerCase('tr').replace(/\s+/g, ' ').trim();
    }

    // ---------- Arama + rol filtresi ----------
    function apply() {
        var q = norm(search.value);
        var visible = 0;

        getRows().forEach(function (row) {
            var roleOk = currentFilter === 'all' || row.dataset.role === currentFilter;
            var textOk = !q || norm(row.textContent).indexOf(q) !== -1;
            var show = roleOk && textOk;

            row.hidden = !show;
            if (show) visible++;
        });

        countEl.textContent = visible + ' kullanıcı gösteriliyor';
        noResult.hidden = visible !== 0;
        clearBtn.hidden = search.value === '';
    }

    search.addEventListener('input', apply);

    clearBtn.addEventListener('click', function () {
        search.value = '';
        apply();
        search.focus();
    });

    chips.forEach(function (chip) {
        chip.addEventListener('click', function () {
            chips.forEach(function (c) { c.classList.remove('active'); });
            chip.classList.add('active');
            currentFilter = chip.dataset.filter;
            apply();
        });
    });

    // ---------- Sıralama ----------
    var headers = table.querySelectorAll('th.um-sortable');

    headers.forEach(function (th) {
        th.addEventListener('click', function () {
            var idx = th.cellIndex;
            var dir = th.classList.contains('asc') ? 'desc' : 'asc';

            headers.forEach(function (h) { h.classList.remove('asc', 'desc'); });
            th.classList.add(dir);

            var rows = getRows();
            rows.sort(function (a, b) {
                var va = a.cells[idx].dataset.value || '';
                var vb = b.cells[idx].dataset.value || '';
                var result = va.localeCompare(vb, 'tr');
                return dir === 'asc' ? result : -result;
            });

            rows.forEach(function (row) { tbody.insertBefore(row, noResult); });
        });
    });

    // ---------- E-posta kopyalama ----------
    tbody.addEventListener('click', function (e) {
        var btn = e.target.closest('.um-copy');
        if (!btn) return;

        var text = btn.dataset.copy;
        var icon = btn.querySelector('i');

        function done() {
            btn.classList.add('copied');
            icon.className = 'bi bi-check2';
            setTimeout(function () {
                btn.classList.remove('copied');
                icon.className = 'bi bi-clipboard';
            }, 1500);
        }

        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(text).then(done);
        } else {
            // HTTP gibi güvenli olmayan ortamlar için yedek yöntem
            var ta = document.createElement('textarea');
            ta.value = text;
            ta.style.position = 'fixed';
            ta.style.opacity = '0';
            document.body.appendChild(ta);
            ta.select();
            try { document.execCommand('copy'); done(); } catch (err) { }
            document.body.removeChild(ta);
        }
    });

    // ---------- "/" tuşu ile aramaya odaklan ----------
    document.addEventListener('keydown', function (e) {
        var tag = (e.target.tagName || '').toLowerCase();
        if (e.key === '/' && tag !== 'input' && tag !== 'textarea') {
            e.preventDefault();
            search.focus();
            search.select();
        }
    });
});