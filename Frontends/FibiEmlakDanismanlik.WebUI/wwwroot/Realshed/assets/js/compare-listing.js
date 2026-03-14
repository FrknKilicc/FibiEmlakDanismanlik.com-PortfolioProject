(function () {
    "use strict";

    const STORAGE_KEY = "fibi_compare_list";
    const COMPARE_PAGE_URL = "/Compare/Index";

    function getCompareState() {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            if (!raw) {
                return {
                    compareTypeKey: null,
                    items: []
                };
            }

            const parsed = JSON.parse(raw);

            return {
                compareTypeKey: parsed.compareTypeKey || null,
                items: Array.isArray(parsed.items) ? parsed.items : []
            };
        } catch (error) {
            console.error("Compare state parse error", error);
            return {
                compareTypeKey: null,
                items: []
            };
        }
    }

    function setCompareState(state) {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
        renderCompareBar();
        window.dispatchEvent(new CustomEvent("fibiCompareChanged", { detail: state }));
    }

    function clearCompareState() {
        localStorage.removeItem(STORAGE_KEY);
        renderCompareBar();
        window.dispatchEvent(new CustomEvent("fibiCompareChanged", {
            detail: {
                compareTypeKey: null,
                items: []
            }
        }));
    }

    function showToast(type, message) {
        if (typeof toastr !== "undefined") {
            toastr[type](message);
            return;
        }

        alert(message);
    }

    function isSameListingAlreadyAdded(items, listingId) {
        return items.some(function (x) {
            return Number(x.listingId) === Number(listingId);
        });
    }

    function ensureCompareBar() {
        let bar = document.getElementById("fibiCompareBar");
        if (bar) return bar;

        bar = document.createElement("div");
        bar.id = "fibiCompareBar";
        bar.className = "fibi-compare-bar";
        bar.innerHTML = `
            <div class="fibi-compare-bar__title">İlan Karşılaştırma</div>
            <div class="fibi-compare-bar__text" id="fibiCompareBarText">Henüz ilan seçilmedi</div>
            <div class="fibi-compare-bar__actions">
                <button type="button" class="fibi-compare-bar__btn fibi-compare-bar__btn--primary" id="fibiCompareGoBtn">
                    Karşılaştırmaya Git
                </button>
                <button type="button" class="fibi-compare-bar__btn fibi-compare-bar__btn--secondary" id="fibiCompareClearBtn">
                    Temizle
                </button>
            </div>
        `;

        document.body.appendChild(bar);

        const goBtn = document.getElementById("fibiCompareGoBtn");
        const clearBtn = document.getElementById("fibiCompareClearBtn");

        goBtn.addEventListener("click", function () {
            const state = getCompareState();

            if (!state.items || state.items.length < 2) {
                showToast("warning", "Karşılaştırma için 2 ilan seçmelisiniz");
                return;
            }

            const ids = state.items.map(function (x) { return x.listingId; }).join(",");
            const type = encodeURIComponent(state.compareTypeKey || "");
            window.location.href = `${COMPARE_PAGE_URL}?ids=${ids}&type=${type}`;
        });

        clearBtn.addEventListener("click", function () {
            clearCompareState();
            showToast("info", "Karşılaştırma listesi temizlendi");
        });

        return bar;
    }

    function renderCompareBar() {
        const bar = ensureCompareBar();
        const text = document.getElementById("fibiCompareBarText");
        const state = getCompareState();
        const count = state.items.length;

        if (count === 0) {
            bar.classList.remove("is-visible");
            text.textContent = "Henüz ilan seçilmedi";
            return;
        }

        bar.classList.add("is-visible");

        if (count === 1) {
            text.textContent = "1 ilan seçildi karşılaştırmak için bir ilan daha ekleyin";
            return;
        }

        text.textContent = `${count} ilan seçildi karşılaştırmaya geçebilirsiniz`;
    }

    function addCompareItem(item) {
        const state = getCompareState();

        if (!item.compareTypeKey || item.compareTypeKey === "unknown") {
            showToast("warning", "Bu ilan tipi karşılaştırma için uygun değil");
            return;
        }

        if (isSameListingAlreadyAdded(state.items, item.listingId)) {
            showToast("info", "Bu ilan zaten karşılaştırma listesine eklenmiş");
            return;
        }

        if (state.items.length > 0 && state.compareTypeKey !== item.compareTypeKey) {
            showToast("warning", "Karşılaştırma sadece aynı ilan tipleri arasında yapılabilir");
            return;
        }

        if (state.items.length >= 2) {
            showToast("warning", "En fazla 2 ilan karşılaştırabilirsiniz");
            return;
        }

        if (!state.compareTypeKey) {
            state.compareTypeKey = item.compareTypeKey;
        }

        state.items.push(item);
        setCompareState(state);

        showToast("success", "İlan karşılaştırma listesine eklendi");
    }

    function handleCompareClick(element) {
        const item = {
            listingId: Number(element.getAttribute("data-listing-id")),
            usageTypeId: Number(element.getAttribute("data-usage-type-id")),
            listingType: element.getAttribute("data-listing-type") || "",
            compareTypeKey: (element.getAttribute("data-compare-type-key") || "unknown").toLowerCase()
        };

        addCompareItem(item);
    }

    document.addEventListener("click", function (event) {
        const button = event.target.closest(".js-compare-btn");
        if (!button) return;

        event.preventDefault();
        handleCompareClick(button);
    });

    document.addEventListener("DOMContentLoaded", function () {
        renderCompareBar();
    });

    window.FibiCompare = {
        getState: getCompareState,
        setState: setCompareState,
        clear: clearCompareState,
        render: renderCompareBar
    };
})();