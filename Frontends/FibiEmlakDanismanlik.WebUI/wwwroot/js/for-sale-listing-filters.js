(function () {
    "use strict";

    function getApiUrl() {
        const el = document.getElementById("forSaleFilterRoot");
        return el ? el.getAttribute("data-api") : null;
    }

    async function fetchOptions(url) {
        try {
            const res = await fetch(url);
            if (!res.ok) return [];
            return await res.json();
        } catch {
            return [];
        }
    }

    function destroyNiceSelect(selectEl) {
        if (!window.jQuery) return;
        const $ = window.jQuery;
        const $ns = $(selectEl).next(".nice-select");
        if ($ns.length) $ns.remove();
        $(selectEl).removeAttr("style");
    }

    function initNiceSelect(selectEl) {
        if (!window.jQuery) return;
        window.jQuery(selectEl).niceSelect();
    }

    function forceDestroyNiceSelect(selectEl) {
        if (!window.jQuery) return;
        const $sel = window.jQuery(selectEl);

        const $nice = $sel.next(".nice-select");
        if ($nice.length) $nice.remove();

        $sel.css("display", "");

        $sel.removeClass("nice-select");
    }

    function fillSelect(selectEl, items, placeholder) {
        if (!window.jQuery) return;

        const $sel = window.jQuery(selectEl);

        const id = (selectEl.id || "").toLowerCase();
        const isLocation =
            id === "cityselect" ||
            id === "districtselect" ||
            id === "neighborhoodselect";

        if (isLocation) {
            forceDestroyNiceSelect(selectEl);

            if ($sel.data("select2")) {
                $sel.select2("destroy");
            }
        } else {
            destroyNiceSelect(selectEl);
        }

        selectEl.innerHTML = "";

        const opt0 = document.createElement("option");
        opt0.value = "";
        opt0.textContent = placeholder;
        selectEl.appendChild(opt0);

        items.forEach(x => {
            const opt = document.createElement("option");
            opt.value = x.id;
            opt.textContent = x.text;
            selectEl.appendChild(opt);
        });

        if (isLocation) {
            $sel.select2({
                width: "100%",
                placeholder: placeholder,
                allowClear: true,
                dropdownParent: $sel.closest(".filter-widget")
            });
        } else {
            initNiceSelect(selectEl);
        }
       
    }
    function setSelectDisabled(selectEl, isDisabled) {
        window.jQuery(selectEl).prop("disabled", isDisabled);

        if (window.jQuery(selectEl).data("select2")) {
            window.jQuery(selectEl).trigger("change.select2");
        }
    }

    function setSelectLoading(selectEl, placeholderText) {
        fillSelect(selectEl, [], placeholderText);
        setSelectDisabled(selectEl, true);
    }

    function initListingTypeClicks() {
        const root = document.getElementById("listingTypeFilter");
        if (!root) return;

        root.addEventListener("click", e => {
            const item = e.target.closest(".filter-option");
            if (item) item.classList.toggle("active");
        });
    }

    function initRoomSelect() {
        const room = document.getElementById("roomSelect");
        if (!room || !window.jQuery) return;

        forceDestroyNiceSelect(room);

        const $room = window.jQuery(room);
        if ($room.data("select2")) $room.select2("destroy");

        const selected = room.getAttribute("data-selected");
        if (selected) room.value = selected;

        $room.select2({
            width: "100%",
            placeholder: "Oda seç",
            allowClear: true,
            dropdownParent: $room.closest(".filter-widget")
        });

        $room.trigger("change.select2");
    }


    async function initLocationDropdowns() {
        const api = getApiUrl();
        if (!api) return;

        const city = document.getElementById("citySelect");
        const dist = document.getElementById("districtSelect");
        const neigh = document.getElementById("neighborhoodSelect");
        if (!city || !dist || !neigh) return;

        const cities = await fetchOptions(api + "Location/cities");
        fillSelect(city, cities, "Şehir seç");
        setSelectDisabled(dist, true);
        setSelectDisabled(neigh, true);

        const cityId = city.getAttribute("data-selected-id");
        const distId = dist.getAttribute("data-selected-id");
        const neighId = neigh.getAttribute("data-selected-id");

        if (cityId) {
            city.value = cityId;
            window.jQuery(city).trigger("change.select2"); 
            const districts = await fetchOptions(api + "Location/districts?cityId=" + cityId);
            fillSelect(dist, districts, "İlçe seç");
            setSelectDisabled(dist, false);

            if (distId) {
                dist.value = distId;
                window.jQuery(dist).trigger("change.select2");

                const neighs = await fetchOptions(api + "Location/neighborhoods?districtId=" + distId);
                fillSelect(neigh, neighs, "Mahalle seç");
                setSelectDisabled(neigh, false);

                if (neighId) {
                    neigh.value = neighId;
                    window.jQuery(neigh).trigger("change.select2");
                }
            } else {
                fillSelect(neigh, [], "Mahalle seç");
            }
        } else {
            fillSelect(dist, [], "İlçe seç");
            fillSelect(neigh, [], "Mahalle seç");
        }
        window.jQuery(city).off(".loc").on("select2:select.loc select2:clear.loc", async function () {
            const cityIdVal = window.jQuery(this).val();

            setSelectLoading(dist, "İl Seçiniz.");
            setSelectLoading(neigh, "Mahalle seç");

            if (!cityIdVal) return;

            const districts = await fetchOptions(api + "Location/districts?cityId=" + cityIdVal);
            fillSelect(dist, districts, "İlçe seç");
            setSelectDisabled(dist, false);
        });


        window.jQuery(dist).off(".loc").on("select2:select.loc select2:clear.loc", async function () {
            const distIdVal = window.jQuery(this).val();

            setSelectLoading(neigh, "İlçe Seçiniz.");

            if (!distIdVal) return;

            const neighs = await fetchOptions(api + "Location/neighborhoods?districtId=" + distIdVal);

            fillSelect(neigh, neighs, "Mahalle seç");
            setSelectDisabled(neigh, false);
        });
        
    }

    function formatThousandsTR(raw) {
        const digits = (raw || "").toString().replace(/\D/g, "");
        if (!digits) return "";

        // 1000000 -> 1.000.000
        return digits.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    }

    function attachPriceFormatter(inputEl) {
        if (!inputEl) return;

        inputEl.addEventListener("input", function () {
            const caretAtEnd = this.selectionStart === this.value.length;

            const formatted = formatThousandsTR(this.value);
            this.value = formatted;

            // caret çok zıplamasın diye basit iyileştirme:
            // kullanıcı genelde sona yazar, onu koruyoruz
            if (caretAtEnd) {
                this.setSelectionRange(this.value.length, this.value.length);
            }
        });
    }

    function initApplyButton() {
        const btn = document.getElementById("applyFilters");
        if (!btn) return;

        btn.addEventListener("click", function () {
            const url = new URL(window.location.href);

            [
                "listingTypeIds",
                "cityId", "districtId", "neighborhoodId",
                "city", "district", "neighborhood",
                "minPrice", "maxPrice", "numberOfRoom",
                "page"
            ].forEach(p => url.searchParams.delete(p));

            document
                .querySelectorAll("#listingTypeFilter .filter-option.active")
                .forEach(x => url.searchParams.append("listingTypeIds", x.dataset.id));

            const city = document.getElementById("citySelect");
            const dist = document.getElementById("districtSelect");
            const neigh = document.getElementById("neighborhoodSelect");

            if (city.value) {
                url.searchParams.set("cityId", city.value);
                url.searchParams.set("city", city.options[city.selectedIndex].text);
            }
            if (dist.value) {
                url.searchParams.set("districtId", dist.value);
                url.searchParams.set("district", dist.options[dist.selectedIndex].text);
            }
            if (neigh.value) {
                url.searchParams.set("neighborhoodId", neigh.value);
                url.searchParams.set("neighborhood", neigh.options[neigh.selectedIndex].text);
            }

            const min = document.getElementById("minPrice")?.value?.replace(/\./g, "");
            const max = document.getElementById("maxPrice")?.value?.replace(/\./g, "");
            const room = document.getElementById("roomSelect")?.value;

            if (min) url.searchParams.set("minPrice", min);
            if (max) url.searchParams.set("maxPrice", max);
            if (room) url.searchParams.set("numberOfRoom", room);

            url.searchParams.set("page", "1");
            window.location.href = url.toString();
        });
    }

    window.addEventListener("load", function () {
        initListingTypeClicks();
        initRoomSelect();
        initLocationDropdowns();
        initApplyButton();

        attachPriceFormatter(document.getElementById("minPrice"));
        attachPriceFormatter(document.getElementById("maxPrice"));
    });

})();
