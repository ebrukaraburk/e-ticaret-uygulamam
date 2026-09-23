document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('loginForm');
    if (!form) return;

    var shell = document.querySelector('.auth-shell');
    var userInput = document.getElementById('usernameOrEmail');
    var passInput = document.getElementById('password');
    var toggleBtn = document.getElementById('togglePassword');
    var capsWarn = document.getElementById('capsWarning');
    var loginBtn = document.getElementById('loginBtn');

    // Sayfa açılınca kullanıcı adına odaklan
    if (userInput) userInput.focus();

    // Şifre göster / gizle
    toggleBtn.addEventListener('click', function () {
        var show = passInput.type === 'password';
        passInput.type = show ? 'text' : 'password';
        toggleBtn.querySelector('i').className = show ? 'bi bi-eye-slash' : 'bi bi-eye';
        passInput.focus();
    });

    // Caps Lock uyarısı
    function checkCaps(e) {
        if (e.getModifierState) {
            capsWarn.classList.toggle('show', e.getModifierState('CapsLock'));
        }
    }
    passInput.addEventListener('keydown', checkCaps);
    passInput.addEventListener('keyup', checkCaps);
    passInput.addEventListener('blur', function () {
        capsWarn.classList.remove('show');
    });

    // Gönderim: doğrulama + yükleniyor durumu
    form.addEventListener('submit', function (e) {
        if (!form.checkValidity()) {
            e.preventDefault();
            form.classList.add('was-validated');

            // Kartı salla
            shell.classList.remove('shake');
            void shell.offsetWidth; // animasyonu yeniden başlatmak için
            shell.classList.add('shake');

            // İlk hatalı alana odaklan
            var firstInvalid = form.querySelector(':invalid');
            if (firstInvalid) firstInvalid.focus();
            return;
        }

        // Çift tıklamayı engelle
        loginBtn.classList.add('loading');
    });

    // Tarayıcıda "geri" ile dönülürse buton takılı kalmasın
    window.addEventListener('pageshow', function (e) {
        if (e.persisted) loginBtn.classList.remove('loading');
    });
});