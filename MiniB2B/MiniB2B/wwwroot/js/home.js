(function () {
    "use strict";

    // ---------- Slider: klavye kontrolü + hover'da durdurma ----------
    var sliderEl = document.getElementById("b2bMainSlider");
    if (sliderEl && window.bootstrap) {
        var carousel = bootstrap.Carousel.getOrCreateInstance(sliderEl);

        sliderEl.setAttribute("tabindex", "0");
        sliderEl.addEventListener("keydown", function (e) {
            if (e.key === "ArrowLeft") carousel.prev();
            if (e.key === "ArrowRight") carousel.next();
        });

        sliderEl.addEventListener("mouseenter", function () { carousel.pause(); });
        sliderEl.addEventListener("mouseleave", function () { carousel.cycle(); });
    }

    // ---------- Ürün kartları: ekrana girince kademeli belirme ----------
    var cards = document.querySelectorAll(".product-card");
    if ("IntersectionObserver" in window && cards.length) {
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry, i) {
                if (entry.isIntersecting) {
                    var target = entry.target;
                    target.style.transitionDelay = (i % 8) * 30 + "ms";
                    requestAnimationFrame(function () {
                        target.style.opacity = "1";
                        target.style.transform = "none";
                    });
                    observer.unobserve(target);
                }
            });
        }, { threshold: 0.1 });

        cards.forEach(function (card) {
            card.style.opacity = "0";
            card.style.transform = "translateY(10px)";
            card.style.transition = "opacity 0.35s ease, transform 0.35s ease";
            observer.observe(card);
        });
    }
})();