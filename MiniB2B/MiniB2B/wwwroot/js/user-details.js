document.addEventListener('DOMContentLoaded', function () {
    var toast = document.getElementById('udToast');
    var timer = null;

    function showToast() {
        toast.classList.add('show');
        clearTimeout(timer);
        timer = setTimeout(function () { toast.classList.remove('show'); }, 1600);
    }

    function copyText(text, done) {
        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(text).then(done);
            return;
        }
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

    document.querySelectorAll('.ud-copy').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var icon = btn.querySelector('i');

            copyText(btn.dataset.copy, function () {
                btn.classList.add('copied');
                icon.className = 'bi bi-check2';
                showToast();

                setTimeout(function () {
                    btn.classList.remove('copied');
                    icon.className = 'bi bi-clipboard';
                }, 1500);
            });
        });
    });
});