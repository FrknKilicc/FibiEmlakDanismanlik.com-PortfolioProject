(function () {
    "use strict";

    function updateNiceSelect(el) {
        if (!window.jQuery) return;
        var $el = window.jQuery(el);
        if ($el.next(".nice-select").length) {
            $el.niceSelect("update");
        }
    }

    async function fetchJson(url) {
        try {
            var res = await fetch(url);
            if (!res.ok) return [];
            return await res.json();
        } catch {
            return [];
        }
    }

    function initListingAlertForm() {
        var form = document.getElementById("listingAlertForm");
        if (!form) return;

        var apiUrl = form.getAttribute("data-api-url") || "";
        var listingTypeSelect = document.getElementById("alertListingType");
        var citySelect = document.getElementById("alertCity");
        var districtSelect = document.getElementById("alertDistrict");
        var roomsContainer = document.getElementById("alertRooms");

        // İlan türü değiştiğinde konut dışı ise oda kutularını gizle ve temizle
        function handleListingTypeChange() {
            if (!listingTypeSelect || !roomsContainer) return;
            var val = parseInt(listingTypeSelect.value, 10);
            var isHousing = val === 2 || val === 8;
            if (isHousing) {
                roomsContainer.style.display = "";
            } else {
                roomsContainer.style.display = "none";
                var checkboxes = roomsContainer.querySelectorAll('input[type="checkbox"]');
                checkboxes.forEach(function (cb) {
                    cb.checked = false;
                });
            }
        }

        if (listingTypeSelect) {
            listingTypeSelect.addEventListener("change", handleListingTypeChange);
            if (window.jQuery) {
                window.jQuery(listingTypeSelect).on("change", handleListingTypeChange);
            }
            handleListingTypeChange();
        }

        // İlçe listesini yükler
        async function loadDistricts(cityId, preselectedDistrictId) {
            if (!districtSelect) return;
            districtSelect.innerHTML = '<option value="">Tüm ilçeler</option>';
            updateNiceSelect(districtSelect);

            if (!cityId || !apiUrl) return;

            try {
                var url = apiUrl + "Location/districts?cityId=" + encodeURIComponent(cityId);
                var districts = await fetchJson(url);
                if (Array.isArray(districts)) {
                    districts.forEach(function (item) {
                        var opt = document.createElement("option");
                        opt.value = item.id;
                        opt.textContent = item.text;
                        if (preselectedDistrictId && String(item.id) === String(preselectedDistrictId)) {
                            opt.selected = true;
                        }
                        districtSelect.appendChild(opt);
                    });
                }
                updateNiceSelect(districtSelect);
            } catch {
                // Hata durumunda sessizce geçilir
            }
        }

        // İl ve başlangıç ilçe verilerini yükler
        async function initLocations() {
            if (!citySelect || !apiUrl) return;

            var selectedCityId = citySelect.getAttribute("data-selected-id");
            var selectedDistrictId = districtSelect ? districtSelect.getAttribute("data-selected-id") : null;

            try {
                var cities = await fetchJson(apiUrl + "Location/cities");
                if (Array.isArray(cities)) {
                    cities.forEach(function (item) {
                        var opt = document.createElement("option");
                        opt.value = item.id;
                        opt.textContent = item.text;
                        if (selectedCityId && String(item.id) === String(selectedCityId)) {
                            opt.selected = true;
                        }
                        citySelect.appendChild(opt);
                    });
                    updateNiceSelect(citySelect);
                }

                if (selectedCityId) {
                    await loadDistricts(selectedCityId, selectedDistrictId);
                }
            } catch {
                // Hata durumunda sessizce geçilir
            }
        }

        if (citySelect) {
            async function onCityChange() {
                var cId = citySelect.value;
                await loadDistricts(cId, null);
            }

            citySelect.addEventListener("change", onCityChange);
            if (window.jQuery) {
                window.jQuery(citySelect).on("change", onCityChange);
            }
        }

        initLocations();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initListingAlertForm);
    } else {
        initListingAlertForm();
    }
})();
