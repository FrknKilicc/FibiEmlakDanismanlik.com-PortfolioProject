(function () {
    "use strict";

    // Sayfa hazır olduğunda görünüm geçişini başlat
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initViewToggle);
    } else {
        initViewToggle();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initFilterToggle);
    } else {
        initFilterToggle();
    }

    function initViewToggle() {
        var toggleGroup = document.querySelector(".fibi-view-toggle");
        var mapRoot = document.getElementById("listingsMapRoot");
        var listView = document.querySelector("[data-list-view]");

        // Gerekli öğeler sayfada yoksa sonlandır
        if (!toggleGroup || !mapRoot || !listView) {
            return;
        }

        var buttons = toggleGroup.querySelectorAll(".fibi-view-btn");

        // Görünüm modunu ayarlayan fonksiyon ("list" veya "map")
        function setView(viewName, persist) {
            if (persist === undefined) {
                persist = true;
            }

            var isMap = viewName === "map";

            // Butonların aktiflik ve erişilebilirlik durumunu güncelle
            buttons.forEach(function (btn) {
                var btnView = btn.getAttribute("data-view");
                var active = btnView === (isMap ? "map" : "list");
                btn.classList.toggle("is-active", active);
                btn.setAttribute("aria-pressed", active ? "true" : "false");
            });

            // Liste ve Harita bölümlerinin görünürlüğünü güncelle
            if (isMap) {
                listView.hidden = true;
                mapRoot.hidden = false;
                // Harita bileşenine görünür olduğunu bildiren özel olayı tetikle
                mapRoot.dispatchEvent(new CustomEvent("lm:show"));
            } else {
                mapRoot.hidden = true;
                listView.hidden = false;
            }

            // Kullanıcı tercihini oturum boyunca sakla
            if (persist) {
                try {
                    sessionStorage.setItem("fibi-list-view", isMap ? "map" : "list");
                } catch (e) {
                    // Gizli sekme veya kısıtlı depolama modlarında hatayı yut
                }
            }
        }

        // Düğmelere tıklama olayı bağla
        buttons.forEach(function (btn) {
            btn.addEventListener("click", function () {
                var view = btn.getAttribute("data-view") || "list";
                setView(view, true);
            });
        });

        // Sayfa ilk yüklendiğinde saklanan tercihi uygula (varsayılan: liste)
        var savedView = null;
        try {
            savedView = sessionStorage.getItem("fibi-list-view");
        } catch (e) {
            savedView = null;
        }

        if (savedView === "map") {
            setView("map", false);
        } else {
            setView("list", false);
        }
    }

    // Mobilde filtre panelini aç/kapa
    function initFilterToggle() {
        var toggle = document.querySelector(".fibi-filter-toggle");
        var panel = toggle && document.getElementById(toggle.getAttribute("aria-controls"));
        if (!toggle || !panel) {
            return;
        }
        var label = toggle.querySelector("span");
        toggle.addEventListener("click", function () {
            var open = panel.classList.toggle("is-open");
            toggle.setAttribute("aria-expanded", open ? "true" : "false");
            if (label) {
                label.textContent = open ? "Filtreleri Gizle" : "Filtrele";
            }
        });
    }
})();
