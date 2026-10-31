(function () {
    "use strict";

    function getApiUrl() {
        const el = document.getElementById("forRentalFilterRoot");
        return el ? (el.getAttribute("data-api-url") || "") : "";
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

    function findIdByName(items, name) {
        if (!name || !items || !items.length) return "";
        const target = name.trim().toLocaleUpperCase("tr-TR");
        const found = items.find(x => x && x.text && x.text.trim().toLocaleUpperCase("tr-TR") === target);
        return found ? String(found.id) : "";
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
            if (item) {
                item.classList.toggle("active");
                loadAdvancedFilters(false);
            }
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

    let advFiltersReqSeq = 0;

    function resetAdvancedFilters() {
        advFiltersReqSeq++;
        setupAdvancedSelect("buildingAgeSelect", false, [], "");
        setupAdvancedSelect("heatingSelect", false, [], "");
        setupAdvancedSelect("titleDeedSelect", false, [], "");
    }

    function setupAdvancedSelect(selectId, isVisible, options, selectedVal) {
        const selectEl = document.getElementById(selectId);
        if (!selectEl) return;

        const widget = selectEl.closest(".filter-widget") || selectEl;

        if (!isVisible) {
            widget.style.display = "none";
            selectEl.value = "";
            selectEl.innerHTML = '<option value="">Tümü</option>';
            if (window.jQuery) {
                forceDestroyNiceSelect(selectEl);
                const $sel = window.jQuery(selectEl);
                if ($sel.data("select2")) {
                    $sel.val("").trigger("change.select2");
                }
            }
            return;
        }

        widget.style.display = "";
        selectEl.innerHTML = "";

        const opt0 = document.createElement("option");
        opt0.value = "";
        opt0.textContent = "Tümü";
        selectEl.appendChild(opt0);

        (options || []).forEach(item => {
            const val = item.id != null ? item.id : (item.value != null ? item.value : item);
            const txt = item.text != null ? item.text : (item.name != null ? item.name : val);
            const opt = document.createElement("option");
            opt.value = String(val);
            opt.textContent = String(txt);
            selectEl.appendChild(opt);
        });

        const targetVal = selectedVal || selectEl.getAttribute("data-selected") || "";
        if (targetVal) {
            selectEl.value = targetVal;
        }

        if (window.jQuery) {
            forceDestroyNiceSelect(selectEl);
            const $sel = window.jQuery(selectEl);
            if ($sel.data("select2")) $sel.select2("destroy");
            $sel.select2({
                width: "100%",
                placeholder: "Tümü",
                allowClear: true,
                dropdownParent: $sel.closest(".filter-widget")
            });
            $sel.val(selectEl.value).trigger("change.select2");
        }
    }

    async function loadAdvancedFilters(isInitial) {
        const activeOptions = document.querySelectorAll("#listingTypeFilter .filter-option.active");
        if (activeOptions.length !== 1) {
            resetAdvancedFilters();
            return;
        }

        const listingTypeId = activeOptions[0].getAttribute("data-id");
        if (!listingTypeId) {
            resetAdvancedFilters();
            return;
        }

        let api = getApiUrl() || "";
        if (!api) return;
        if (!api.endsWith("/")) api += "/";

        const seq = ++advFiltersReqSeq;
        const ut = 2; // 2 for rental
        const url = api + "listingtypes/advanced-filters?listingTypeId=" + encodeURIComponent(listingTypeId) + "&usageType=" + ut;

        const data = await fetchOptions(url);
        if (seq !== advFiltersReqSeq) return;
        if (!data || typeof data !== "object" || Array.isArray(data)) {
            resetAdvancedFilters();
            return;
        }

        const urlParams = new URLSearchParams(window.location.search);
        const selBuildingAge = isInitial ? (urlParams.get("buildingAge") || "") : "";
        const selHeating = isInitial ? (urlParams.get("heating") || "") : "";
        const selTitleDeed = isInitial ? (urlParams.get("titleDeedStatus") || "") : "";

        setupAdvancedSelect("buildingAgeSelect", !!data.showBuildingAge, data.buildingAgeOptions, selBuildingAge);
        setupAdvancedSelect("heatingSelect", !!data.showHeating, data.heatingOptions, selHeating);
        setupAdvancedSelect("titleDeedSelect", !!data.showTitleDeed, data.titleDeedOptions, selTitleDeed);

        if (isInitial) {
            document.getElementById("buildingAgeSelect")?.removeAttribute("data-selected");
            document.getElementById("heatingSelect")?.removeAttribute("data-selected");
            document.getElementById("titleDeedSelect")?.removeAttribute("data-selected");
        }
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

        let cityId = city.getAttribute("data-selected-id");
        let distId = dist.getAttribute("data-selected-id");
        const neighId = neigh.getAttribute("data-selected-id");

        if (!cityId) {
            cityId = findIdByName(cities, city.getAttribute("data-selected-name"));
        }

        if (cityId) {
            city.value = cityId;
            window.jQuery(city).trigger("change.select2");
            const districts = await fetchOptions(api + "Location/districts?cityId=" + cityId);
            fillSelect(dist, districts, "İlçe seç");
            setSelectDisabled(dist, false);

            if (!distId) {
                distId = findIdByName(districts, dist.getAttribute("data-selected-name"));
            }

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
        return digits.replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    }

    function attachPriceFormatter(inputEl) {
        if (!inputEl) return;

        inputEl.addEventListener("input", function () {
            const caretAtEnd = this.selectionStart === this.value.length;

            const formatted = formatThousandsTR(this.value);
            this.value = formatted;

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
            url.pathname = "/kiralik";

            [
                "listingTypeIds",
                "listingTypeName",
                "cityId", "districtId", "neighborhoodId",
                "city", "district", "neighborhood",
                "minPrice", "maxPrice", "numberOfRoom",
                "buildingAge", "heating", "titleDeedStatus",
                "minArea", "maxArea", "numberOfBathRoom", "floor", "numberOfSection", "agentId", "zoningStatus", "searchTerm",
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

            const buildingAge = document.getElementById("buildingAgeSelect")?.value;
            const heating = document.getElementById("heatingSelect")?.value;
            const titleDeed = document.getElementById("titleDeedSelect")?.value;

            if (buildingAge) url.searchParams.set("buildingAge", buildingAge);
            if (heating) url.searchParams.set("heating", heating);
            if (titleDeed) url.searchParams.set("titleDeedStatus", titleDeed);

            url.searchParams.set("page", "1");
            window.location.href = url.toString();
        });
    }

    window.addEventListener("load", function () {
        initListingTypeClicks();
        initRoomSelect();
        initLocationDropdowns();
        loadAdvancedFilters(true);
        initApplyButton();

        attachPriceFormatter(document.getElementById("minPrice"));
        attachPriceFormatter(document.getElementById("maxPrice"));
    });
})();
