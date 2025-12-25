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

    function fillSelect(selectEl, items, placeholder) {
        
        destroyNiceSelect(selectEl);

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

        initNiceSelect(selectEl);
    }

    function bindChangeFromNice(selectEl) {
        if (!window.jQuery) return;
        const $ = window.jQuery;
        const $ns = $(selectEl).next(".nice-select");
        if (!$ns.length) return;

        $ns.off("click.nicefix").on("click.nicefix", ".option", function () {
            setTimeout(() => {
                selectEl.dispatchEvent(new Event("change", { bubbles: true }));
            }, 0);
        });
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
        if (!room) return;

        destroyNiceSelect(room);

        const selected = room.getAttribute("data-selected");
        if (selected) room.value = selected;

        initNiceSelect(room);
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
        bindChangeFromNice(city);

        const cityId = city.getAttribute("data-selected-id");
        const distId = dist.getAttribute("data-selected-id");
        const neighId = neigh.getAttribute("data-selected-id");

       
        if (cityId) {
            city.value = cityId;
           

            const districts = await fetchOptions(api + "Location/districts?cityId=" + cityId);
            fillSelect(dist, districts, "İlçe seç");
            bindChangeFromNice(dist);

            if (distId) {
                dist.value = distId;
               

                const neighs = await fetchOptions(api + "Location/neighborhoods?districtId=" + distId);
                fillSelect(neigh, neighs, "Mahalle seç");

                if (neighId) {
                    neigh.value = neighId;
                    
                }
            }
        }

        city.addEventListener("change", async function () {
            fillSelect(dist, [], "İlçe seç");
            fillSelect(neigh, [], "Mahalle seç");

            if (!this.value) return;

            const districts = await fetchOptions(api + "Location/districts?cityId=" + this.value);
            fillSelect(dist, districts, "İlçe seç");
            bindChangeFromNice(dist);
        });

        dist.addEventListener("change", async function () {
            fillSelect(neigh, [], "Mahalle seç");

            if (!this.value) return;

            const neighs = await fetchOptions(api + "Location/neighborhoods?districtId=" + this.value);
            fillSelect(neigh, neighs, "Mahalle seç");
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

            const min = document.getElementById("minPrice")?.value;
            const max = document.getElementById("maxPrice")?.value;
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
    });

})();
