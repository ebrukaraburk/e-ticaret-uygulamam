(function () {
    var printBtn = document.getElementById('printOrderBtn');
    if (printBtn) {
        printBtn.addEventListener('click', function () {
            window.print();
        });
    }
})();