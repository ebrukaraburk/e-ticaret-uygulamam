document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('editForm');
    if (!form) return;

    var card = document.getElementById('ueCard');
    var first = document.getElementById('FirstName');
    var last = document.getElementById('LastName');
    var username = document.getElementById('Username');
    var avatar = document.getElementById('ueAvatar');
    var liveName = document.getElementById('ueLiveName');

    var pass = document.getElementById('newPassword');
    var confirmInput = document.getElementById('newPasswordConfirm');
    var confirmError = document.getElementById('confirmError');
    var toggleBtn = document.getElementById('togglePassword');
    var strength = document.getElementById('strength');
    var strengthText = document.getElementById('strengthText');

    var roleWarn = document.getElementById('roleWarn');
    var saveBtn = document.getElementById('saveBtn');
    var initialRole = form.dataset.initialRole;

    var dirty = false;
    var submitting = false;

    // ---------- Başlıktaki avatar ve isim canlı güncellensin ----------
    function updateHeader() {
        var f = first.value.trim();
        var l = last.value.trim();
        var name = (f + ' ' + l).trim() || username.value.trim() || 'Kullanıcı';
        var ini = (f.charAt(0) + l.charAt(0)) || username.value.trim().charAt(0) || '?';

        liveName.textContent = name;
        avatar.textContent = ini.toLocaleUpperCase('tr');
    }
    [first, last, username].forEach(function (el) {
        el.addEventListener('input', updateHeader);
    });

    // ---------- Şifre göster / gizle (iki alan birlikte) ----------
    toggleBtn.addEventListener('click', function () {
        var show = pass.type === 'password';
        pass.type = show ? 'text' : 'password';
        confirmInput.type = show ? 'text' : 'password';
        toggleBtn.querySelector('i').className = show ? 'bi bi-eye-slash' : 'bi bi-eye';
        pass.focus();
    });

    // ---------- Şifre gücü ----------
    var labels = ['', 'Zayıf', 'Orta', 'İyi', 'Güçlü'];
    function updateStrength() {
        var v = pass.value;
        var score = 0;
        if (v.length >= 8) score++;
        if (/[a-z]/.test(v) && /[A-Z]/.test(v)) score++;
        if (/\d/.test(v)) score++;
        if (/[^A-Za-z0-9]/.test(v)) score++;
        if (v.length === 0) score = 0;
        else if (score === 0) score = 1;

        strength.setAttribute('data-level', score);
        strengthText.textContent = score ? 'Şifre gücü: ' + labels[score] : '';
    }

    // ---------- Şifre eşleşmesi (sadece yeni şifre yazıldıysa zorunlu) ----------
    function checkMatch() {
        if (pass.value === '' && confirmInput.value === '') {
            confirmInput.setCustomValidity('');
        } else if (confirmInput.value !== pass.value) {
            confirmInput.setCustomValidity('Şifreler eşleşmiyor.');
            confirmError.textContent = 'Şifreler eşleşmiyor.';
        } else {
            confirmInput.setCustomValidity('');
        }
    }
    pass.addEventListener('input', function () { updateStrength(); checkMatch(); });
    confirmInput.addEventListener('input', checkMatch);

    // ---------- Yönetici yetkisi kaldırma uyarısı ----------
    function checkRole() {
        var selected = form.querySelector('input[name="Role"]:checked');
        var removing = initialRole === 'admin' && selected && selected.value !== 'Admin';
        roleWarn.hidden = !removing;
    }
    form.querySelectorAll('input[name="Role"]').forEach(function (r) {
        r.addEventListener('change', checkRole);
    });
    checkRole();

    // ---------- Kaydedilmemiş değişiklik uyarısı ----------
    form.addEventListener('input', function () { dirty = true; });
    form.addEventListener('change', function () { dirty = true; });

    window.addEventListener('beforeunload', function (e) {
        if (dirty && !submitting) {
            e.preventDefault();
            e.returnValue = '';
        }
    });

    // ---------- Gönderim ----------
    form.addEventListener('submit', function (e) {
        checkMatch();

        if (!form.checkValidity()) {
            e.preventDefault();
            form.classList.add('was-validated');

            card.classList.remove('shake');
            void card.offsetWidth; // animasyonu yeniden başlatmak için
            card.classList.add('shake');

            var firstInvalid = form.querySelector(':invalid');
            if (firstInvalid) firstInvalid.focus();
            return;
        }

        submitting = true;
        saveBtn.classList.add('loading');
    });

    // Tarayıcıda "geri" ile dönülürse buton takılı kalmasın
    window.addEventListener('pageshow', function (e) {
        if (e.persisted) {
            submitting = false;
            saveBtn.classList.remove('loading');
        }
    });
});