document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('registerForm');
    if (!form) return;

    var shell = document.querySelector('.auth-shell');
    var pass = document.getElementById('password');
    var confirmInput = document.getElementById('passwordConfirm');
    var confirmError = document.getElementById('confirmError');
    var toggleBtn = document.getElementById('togglePassword');
    var strength = document.getElementById('strength');
    var strengthText = document.getElementById('strengthText');
    var registerBtn = document.getElementById('registerBtn');

    var firstField = form.querySelector('input');
    if (firstField) firstField.focus();

    // Şifre göster / gizle (iki alan birlikte)
    toggleBtn.addEventListener('click', function () {
        var show = pass.type === 'password';
        pass.type = show ? 'text' : 'password';
        confirmInput.type = show ? 'text' : 'password';
        toggleBtn.querySelector('i').className = show ? 'bi bi-eye-slash' : 'bi bi-eye';
        pass.focus();
    });

    // Şifre gücü
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

    // Şifre eşleşme kontrolü
    function checkMatch() {
        if (confirmInput.value === '') {
            confirmInput.setCustomValidity('Şifreyi tekrar girin.');
            confirmError.textContent = 'Şifreyi tekrar girin.';
        } else if (confirmInput.value !== pass.value) {
            confirmInput.setCustomValidity('Şifreler eşleşmiyor.');
            confirmError.textContent = 'Şifreler eşleşmiyor.';
        } else {
            confirmInput.setCustomValidity('');
        }
    }

    pass.addEventListener('input', function () {
        updateStrength();
        checkMatch();
    });
    confirmInput.addEventListener('input', checkMatch);
    checkMatch();

    // Gönderim
    form.addEventListener('submit', function (e) {
        checkMatch();

        if (!form.checkValidity()) {
            e.preventDefault();
            form.classList.add('was-validated');

            shell.classList.remove('shake');
            void shell.offsetWidth;
            shell.classList.add('shake');

            var firstInvalid = form.querySelector(':invalid');
            if (firstInvalid) firstInvalid.focus();
            return;
        }

        registerBtn.classList.add('loading');
    });

    window.addEventListener('pageshow', function (e) {
        if (e.persisted) registerBtn.classList.remove('loading');
    });
});