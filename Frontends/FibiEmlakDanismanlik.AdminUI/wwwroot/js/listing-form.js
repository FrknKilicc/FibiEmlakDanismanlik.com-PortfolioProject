/**
 * Fibi Emlak Danışmanlık - İlan Ekleme ve Düzenleme Formu
 * Konum zinciri (İl -> İlçe -> Mahalle), m² birim fiyat hesaplama ve çift gönderim koruması.
 */
document.addEventListener('DOMContentLoaded', function () {
    initLocationDropdowns();
    initPricePerSquareMeterSuggestion();
    initFormSubmitHandler();
});

/**
 * İl, ilçe ve mahalle bağlı açılır liste zincirini yönetir.
 */
function initLocationDropdowns() {
    var citySelect = document.getElementById('citySelect');
    var districtSelect = document.getElementById('districtSelect');
    var neighborhoodSelect = document.getElementById('neighborhoodSelect');

    var cityInput = document.getElementById('cityInput');
    var districtInput = document.getElementById('districtInput');
    var neighborhoodInput = document.getElementById('neighborhoodInput');

    var cityIdInput = document.getElementById('cityIdInput');
    var districtIdInput = document.getElementById('districtIdInput');
    var neighborhoodIdInput = document.getElementById('neighborhoodIdInput');

    if (!citySelect || !districtSelect || !neighborhoodSelect) {
        return;
    }

    var isReadOnly = citySelect.disabled;

    var initialCityName = citySelect.getAttribute('data-current-name') || (cityInput ? cityInput.value : '');
    var initialCityId = citySelect.getAttribute('data-current-id') || (cityIdInput ? cityIdInput.value : '');
    var initialDistrictName = districtSelect.getAttribute('data-current-name') || (districtInput ? districtInput.value : '');
    var initialDistrictId = districtSelect.getAttribute('data-current-id') || (districtIdInput ? districtIdInput.value : '');
    var initialNeighborhoodName = neighborhoodSelect.getAttribute('data-current-name') || (neighborhoodInput ? neighborhoodInput.value : '');
    var initialNeighborhoodId = neighborhoodSelect.getAttribute('data-current-id') || (neighborhoodIdInput ? neighborhoodIdInput.value : '');

    // Sayfa açılışında illeri yükle
    loadCities(initialCityId, initialCityName, function (selectedCityId) {
        if (selectedCityId) {
            loadDistricts(selectedCityId, initialDistrictId, initialDistrictName, function (selectedDistrictId) {
                if (selectedDistrictId) {
                    loadNeighborhoods(selectedDistrictId, initialNeighborhoodId, initialNeighborhoodName);
                }
            });
        }
    });

    // İl değiştiğinde
    citySelect.addEventListener('change', function () {
        var selectedOption = citySelect.options[citySelect.selectedIndex];
        var cityId = citySelect.value;
        var cityName = selectedOption && cityId ? (selectedOption.getAttribute('data-text') || selectedOption.text) : '';

        if (cityInput) cityInput.value = cityName;
        if (cityIdInput) cityIdInput.value = cityId;

        // İlçe ve mahalleyi sıfırla
        resetSelect(districtSelect, 'Önce İl Seçiniz');
        resetSelect(neighborhoodSelect, 'Önce İlçe Seçiniz');
        if (districtInput) districtInput.value = '';
        if (districtIdInput) districtIdInput.value = '';
        if (neighborhoodInput) neighborhoodInput.value = '';
        if (neighborhoodIdInput) neighborhoodIdInput.value = '';

        if (cityId) {
            districtSelect.disabled = false;
            loadDistricts(cityId, null, null);
        } else {
            districtSelect.disabled = true;
            neighborhoodSelect.disabled = true;
        }
    });

    // İlçe değiştiğinde
    districtSelect.addEventListener('change', function () {
        var selectedOption = districtSelect.options[districtSelect.selectedIndex];
        var districtId = districtSelect.value;
        var districtName = selectedOption && districtId ? (selectedOption.getAttribute('data-text') || selectedOption.text) : '';

        if (districtInput) districtInput.value = districtName;
        if (districtIdInput) districtIdInput.value = districtId;

        // Mahalleyi sıfırla
        resetSelect(neighborhoodSelect, 'Önce İlçe Seçiniz');
        if (neighborhoodInput) neighborhoodInput.value = '';
        if (neighborhoodIdInput) neighborhoodIdInput.value = '';

        if (districtId) {
            neighborhoodSelect.disabled = false;
            loadNeighborhoods(districtId, null, null);
        } else {
            neighborhoodSelect.disabled = true;
        }
    });

    // Mahalle değiştiğinde
    neighborhoodSelect.addEventListener('change', function () {
        var selectedOption = neighborhoodSelect.options[neighborhoodSelect.selectedIndex];
        var neighborhoodId = neighborhoodSelect.value;
        var neighborhoodName = selectedOption && neighborhoodId ? (selectedOption.getAttribute('data-text') || selectedOption.text) : '';

        if (neighborhoodInput) neighborhoodInput.value = neighborhoodName;
        if (neighborhoodIdInput) neighborhoodIdInput.value = neighborhoodId;
    });

    function loadCities(currentId, currentName, onLoaded) {
        citySelect.innerHTML = '<option value="">Yükleniyor...</option>';
        fetch('/ilanlar/konum/iller')
            .then(function (res) { return res.ok ? res.json() : []; })
            .then(function (data) {
                citySelect.innerHTML = '<option value="">İl Seçiniz</option>';
                var matchedId = null;

                data.forEach(function (item) {
                    var opt = document.createElement('option');
                    opt.value = item.id;
                    opt.text = item.text;
                    opt.setAttribute('data-text', item.text);

                    var isMatch = false;
                    if (currentId && item.id.toString() === currentId.toString()) {
                        isMatch = true;
                    } else if (!currentId && currentName && item.text.trim().toLowerCase() === currentName.trim().toLowerCase()) {
                        isMatch = true;
                    }

                    if (isMatch) {
                        opt.selected = true;
                        matchedId = item.id;
                        if (cityInput) cityInput.value = item.text;
                        if (cityIdInput) cityIdInput.value = item.id;
                    }

                    citySelect.appendChild(opt);
                });

                if (isReadOnly) {
                    citySelect.disabled = true;
                }

                if (onLoaded) {
                    onLoaded(matchedId);
                }
            })
            .catch(function () {
                citySelect.innerHTML = '<option value="">İller yüklenemedi</option>';
            });
    }

    function loadDistricts(cityId, currentId, currentName, onLoaded) {
        districtSelect.innerHTML = '<option value="">Yükleniyor...</option>';
        districtSelect.disabled = false;
        fetch('/ilanlar/konum/ilceler?cityId=' + encodeURIComponent(cityId))
            .then(function (res) { return res.ok ? res.json() : []; })
            .then(function (data) {
                districtSelect.innerHTML = '<option value="">İlçe Seçiniz</option>';
                var matchedId = null;

                data.forEach(function (item) {
                    var opt = document.createElement('option');
                    opt.value = item.id;
                    opt.text = item.text;
                    opt.setAttribute('data-text', item.text);

                    var isMatch = false;
                    if (currentId && item.id.toString() === currentId.toString()) {
                        isMatch = true;
                    } else if (!currentId && currentName && item.text.trim().toLowerCase() === currentName.trim().toLowerCase()) {
                        isMatch = true;
                    }

                    if (isMatch) {
                        opt.selected = true;
                        matchedId = item.id;
                        if (districtInput) districtInput.value = item.text;
                        if (districtIdInput) districtIdInput.value = item.id;
                    }

                    districtSelect.appendChild(opt);
                });

                if (isReadOnly) {
                    districtSelect.disabled = true;
                }

                if (onLoaded) {
                    onLoaded(matchedId);
                }
            })
            .catch(function () {
                districtSelect.innerHTML = '<option value="">İlçeler yüklenemedi</option>';
            });
    }

    function loadNeighborhoods(districtId, currentId, currentName) {
        neighborhoodSelect.innerHTML = '<option value="">Yükleniyor...</option>';
        neighborhoodSelect.disabled = false;
        fetch('/ilanlar/konum/mahalleler?districtId=' + encodeURIComponent(districtId))
            .then(function (res) { return res.ok ? res.json() : []; })
            .then(function (data) {
                neighborhoodSelect.innerHTML = '<option value="">Mahalle Seçiniz</option>';

                data.forEach(function (item) {
                    var opt = document.createElement('option');
                    opt.value = item.id;
                    opt.text = item.text;
                    opt.setAttribute('data-text', item.text);

                    var isMatch = false;
                    if (currentId && item.id.toString() === currentId.toString()) {
                        isMatch = true;
                    } else if (!currentId && currentName && item.text.trim().toLowerCase() === currentName.trim().toLowerCase()) {
                        isMatch = true;
                    }

                    if (isMatch) {
                        opt.selected = true;
                        if (neighborhoodInput) neighborhoodInput.value = item.text;
                        if (neighborhoodIdInput) neighborhoodIdInput.value = item.id;
                    }

                    neighborhoodSelect.appendChild(opt);
                });

                if (isReadOnly) {
                    neighborhoodSelect.disabled = true;
                }
            })
            .catch(function () {
                neighborhoodSelect.innerHTML = '<option value="">Mahalleler yüklenemedi</option>';
            });
    }

    function resetSelect(selectEl, placeholder) {
        selectEl.innerHTML = '<option value="">' + placeholder + '</option>';
        selectEl.disabled = true;
    }
}

/**
 * Arsa vb. ilanlar için Alan ve Fiyat/Kira değerine göre m² birim fiyatını otomatik hesaplar ve önerir.
 */
function initPricePerSquareMeterSuggestion() {
    var priceInput = document.getElementById('priceInput');
    var rentInput = document.getElementById('rentInput');
    var areaInput = document.getElementById('areaInput');
    var m2PriceInput = document.getElementById('m2PriceInput');

    if (!m2PriceInput || !areaInput || (!priceInput && !rentInput)) {
        return;
    }

    var mainPriceInput = priceInput || rentInput;
    var userManuallyChangedM2Price = !!m2PriceInput.value;

    m2PriceInput.addEventListener('input', function () {
        userManuallyChangedM2Price = true;
    });

    function calculateM2Price() {
        if (userManuallyChangedM2Price) {
            return;
        }

        var priceVal = parseFloat(mainPriceInput.value);
        var areaVal = parseFloat(areaInput.value);

        if (!isNaN(priceVal) && priceVal > 0 && !isNaN(areaVal) && areaVal > 0) {
            var unitPrice = Math.round(priceVal / areaVal);
            m2PriceInput.value = unitPrice;
        }
    }

    mainPriceInput.addEventListener('input', calculateM2Price);
    areaInput.addEventListener('input', calculateM2Price);
}

/**
 * Form gönderimi sırasında çift tıklamayı önler ve submit butonunu devre dışı bırakır.
 */
function initFormSubmitHandler() {
    var form = document.getElementById('listingForm');
    var submitBtn = document.getElementById('submitBtn');

    if (!form || !submitBtn) {
        return;
    }

    var citySelect = document.getElementById('citySelect');
    var districtSelect = document.getElementById('districtSelect');
    var neighborhoodSelect = document.getElementById('neighborhoodSelect');

    var cityInput = document.getElementById('cityInput');
    var districtInput = document.getElementById('districtInput');
    var neighborhoodInput = document.getElementById('neighborhoodInput');

    var cityIdInput = document.getElementById('cityIdInput');
    var districtIdInput = document.getElementById('districtIdInput');
    var neighborhoodIdInput = document.getElementById('neighborhoodIdInput');

    form.addEventListener('submit', function (e) {
        // Gönderim öncesinde gizli konum alanlarının en güncel seçili metinlerle dolu olmasını sağla
        if (citySelect && citySelect.selectedIndex > 0 && cityInput) {
            var selectedCity = citySelect.options[citySelect.selectedIndex];
            cityInput.value = selectedCity.getAttribute('data-text') || selectedCity.text;
            if (cityIdInput) cityIdInput.value = citySelect.value;
        }

        if (districtSelect && districtSelect.selectedIndex > 0 && districtInput) {
            var selectedDistrict = districtSelect.options[districtSelect.selectedIndex];
            districtInput.value = selectedDistrict.getAttribute('data-text') || selectedDistrict.text;
            if (districtIdInput) districtIdInput.value = districtSelect.value;
        }

        if (neighborhoodSelect && neighborhoodSelect.selectedIndex > 0 && neighborhoodInput) {
            var selectedNeighborhood = neighborhoodSelect.options[neighborhoodSelect.selectedIndex];
            neighborhoodInput.value = selectedNeighborhood.getAttribute('data-text') || selectedNeighborhood.text;
            if (neighborhoodIdInput) neighborhoodIdInput.value = neighborhoodSelect.value;
        }

        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return;
        }

        // Çift tıklamayı / mükerrer gönderimi engelle
        submitBtn.disabled = true;
        var originalHtml = submitBtn.innerHTML;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Kaydediliyor...';

        // Güvenlik: 10 saniye sonra form bir sebeple takılırsa butonu tekrar aç
        setTimeout(function () {
            if (submitBtn.disabled) {
                submitBtn.disabled = false;
                submitBtn.innerHTML = originalHtml;
            }
        }, 10000);
    });
}
