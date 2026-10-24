/**
 * Fibi Emlak Danışmanlık - İlan Formu Gelişmiş Kullanıcı Deneyimi (UX)
 * 
 * 1. Para alanları tr-TR maskeleme ve ham gönderim
 * 2. Karakter sayaçları
 * 3. Otomatik büyüyen metin alanları (autogrow)
 * 4. Canlı tamamlanma oranı ve eksik listesi
 * 5. Bölüm gezinme (section-nav) ve IntersectionObserver
 * 6. Canlı önizleme kartı
 * 7. Değişiklik göstergesi (dirty form), beforeunload ve Ctrl+S kısayolu
 * 8. Sihirbaz modu (wizard - yalnızca yeni ilanda)
 * 9. Erişilebilirlik (a11y)
 */
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var form = document.getElementById('listingForm');
        if (!form) {
            return;
        }

        var isWizard = form.getAttribute('data-wizard') === 'true';
        var typeIdInput = form.querySelector('input[name="ListingTypeId"]');
        var listingTypeId = typeIdInput ? parseInt(typeIdInput.value, 10) : 0;
        var isHousing = (listingTypeId === 2 || listingTypeId === 8);
        var isNew = isWizard || !document.getElementById('sec-photos');

        var wizardSections = isWizard ? Array.from(form.querySelectorAll('.listing-section')) : [];
        var goToWizardStepFn = null;

        /* ==========================================================================
           1. PARA ALANLARI ([data-money])
           ========================================================================== */
        var moneyInputs = Array.from(document.querySelectorAll('[data-money]'));

        function unformatMoney(val) {
            if (!val) return '';
            // Binlik noktaları sil, ondalık virgülü noktaya çevir
            return String(val).replace(/\./g, '').replace(',', '.');
        }

        function formatMoneyValue(val) {
            if (val === null || val === undefined) return '';
            var str = String(val).trim();
            if (!str) return '';

            var hasComma = str.indexOf(',') !== -1;
            var parts = str.split(',');
            var intRaw = parts[0].replace(/\D/g, '');
            var decRaw = parts.length > 1 ? parts.slice(1).join('').replace(/\D/g, '').slice(0, 2) : null;

            var formattedInt = '';
            if (intRaw) {
                if (intRaw.length > 1 && intRaw.startsWith('0')) {
                    intRaw = intRaw.replace(/^0+/, '') || '0';
                }
                formattedInt = intRaw.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
            } else if (hasComma) {
                formattedInt = '0';
            }

            var finalVal = formattedInt;
            if (hasComma) {
                finalVal += ',' + (decRaw !== null ? decRaw : '');
            }
            return finalVal;
        }

        function formatInitialMoney(val) {
            if (!val) return '';
            var str = String(val).trim();
            if (!str) return '';

            // Sunucudan InvariantCulture gelirse (örn. 9680000.00 veya 9680000)
            if (str.indexOf('.') !== -1 && str.indexOf(',') === -1) {
                var dotParts = str.split('.');
                var intStr = dotParts[0].replace(/\D/g, '');
                var decStr = dotParts[1] ? dotParts[1].replace(/\D/g, '').slice(0, 2) : '';
                if (/^0+$/.test(decStr)) {
                    decStr = '';
                }
                var formattedInt = intStr.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
                return decStr ? (formattedInt + ',' + decStr) : formattedInt;
            }

            return formatMoneyValue(str);
        }

        function handleMoneyInput(e) {
            var input = e.target;
            var val = input.value;
            var selStart = input.selectionStart || 0;

            var leftText = val.slice(0, selStart);
            var leftDigitsAndComma = leftText.replace(/[^\d,]/g, '');

            var formatted = formatMoneyValue(val);
            input.value = formatted;

            var targetCount = leftDigitsAndComma.length;
            var newCursorPos = 0;
            var counted = 0;

            for (var i = 0; i < formatted.length; i++) {
                var ch = formatted[i];
                if (/[\d,]/.test(ch)) {
                    counted++;
                }
                if (counted >= targetCount) {
                    newCursorPos = i + 1;
                    break;
                }
            }
            if (counted < targetCount) {
                newCursorPos = formatted.length;
            }

            input.setSelectionRange(newCursorPos, newCursorPos);
        }

        function handleMoneyKeyDown(e) {
            var input = e.target;
            // Backspace ile binlik ayırıcı nokta silinirken önceki rakamı atlama/silme kolaylığı
            if (e.key === 'Backspace') {
                var pos = input.selectionStart;
                if (pos === input.selectionEnd && pos > 0) {
                    if (input.value[pos - 1] === '.') {
                        e.preventDefault();
                        var before = input.value.slice(0, pos - 2);
                        var after = input.value.slice(pos);
                        input.value = before + after;
                        var newPos = Math.max(0, pos - 2);
                        input.setSelectionRange(newPos, newPos);
                        input.dispatchEvent(new Event('input', { bubbles: true }));
                    }
                }
            } else if (e.code === 'NumpadDecimal') {
                // Numpad ayırıcı tuşuna basıldığında virgül ekle
                e.preventDefault();
                var start = input.selectionStart;
                var end = input.selectionEnd;
                if (input.value.indexOf(',') === -1) {
                    var valBefore = input.value.slice(0, start);
                    var valAfter = input.value.slice(end);
                    input.value = valBefore + ',' + valAfter;
                    input.setSelectionRange(start + 1, start + 1);
                    input.dispatchEvent(new Event('input', { bubbles: true }));
                }
            }
        }

        moneyInputs.forEach(function (input) {
            if (input.value) {
                input.value = formatInitialMoney(input.value);
            }
            input.addEventListener('input', handleMoneyInput);
            input.addEventListener('keydown', handleMoneyKeyDown);
        });

        // Form gönderilmeden HEMEN önce capture: true ile ham biçime dönüştür
        form.addEventListener('submit', function () {
            moneyInputs.forEach(function (input) {
                input.dataset.formattedValue = input.value;
                input.value = unformatMoney(input.value);
            });

            // HTML5 doğrulaması başarısız olursa formatı hemen geri getir
            if (!form.checkValidity()) {
                moneyInputs.forEach(function (input) {
                    if (input.dataset.formattedValue !== undefined) {
                        input.value = input.dataset.formattedValue;
                    }
                });
            }
        }, true); // capture: true

        /* ==========================================================================
           2. KARAKTER SAYACI ([data-counter])
           ========================================================================== */
        var counterInputs = Array.from(document.querySelectorAll('[data-counter]'));
        counterInputs.forEach(function (input) {
            var id = input.id;
            if (!id) return;

            var counter = document.querySelector('.char-counter[data-for="' + id + '"]');
            if (!counter) return;

            counter.setAttribute('aria-live', 'polite');
            var max = parseInt(input.getAttribute('maxlength'), 10) || 0;
            if (!max) return;

            function updateCounter() {
                var len = input.value ? input.value.length : 0;
                counter.textContent = len + ' / ' + max;
                counter.classList.remove('text-warning', 'text-danger');
                if (len >= max) {
                    counter.classList.add('text-danger');
                } else if (len >= max * 0.9) {
                    counter.classList.add('text-warning');
                }
            }

            input.addEventListener('input', updateCounter);
            updateCounter();
        });

        /* ==========================================================================
           3. OTOMATİK BÜYÜYEN METİN ALANLARI ([data-autogrow])
           ========================================================================== */
        var autogrowTextareas = Array.from(document.querySelectorAll('textarea[data-autogrow]'));
        autogrowTextareas.forEach(function (textarea) {
            var computed = window.getComputedStyle(textarea);
            var lineHeight = parseFloat(computed.lineHeight) || 24;
            var minHeight = lineHeight * 6;

            function adjustHeight() {
                textarea.style.height = 'auto';
                var targetHeight = Math.max(textarea.scrollHeight, minHeight);
                textarea.style.height = targetHeight + 'px';
            }

            textarea.addEventListener('input', adjustHeight);
            adjustHeight();
        });

        /* ==========================================================================
           4. TAMAMLANMA ORANI VE EKSİK LİSTESİ
           ========================================================================== */
        var completenessContainer = document.getElementById('completeness');
        var completenessValEl = document.getElementById('completenessValue');
        var completenessBar = document.querySelector('.completeness-bar');
        var completenessList = document.getElementById('completenessList');

        if (completenessValEl) {
            completenessValEl.setAttribute('aria-live', 'polite');
        }

        function getChecklistItems() {
            var items = [];

            // 1. Başlık (≥ 20 karakter)
            var titleEl = document.getElementById('propertyName');
            if (titleEl) {
                items.push({
                    id: 'propertyName',
                    label: 'Başlık (en az 20 karakter)',
                    isComplete: function () {
                        return (titleEl.value || '').trim().length >= 20;
                    },
                    targetEl: titleEl
                });
            }

            // 2. Açıklama (≥ 300 karakter)
            var descEl = document.getElementById('propertyDescription');
            if (descEl) {
                items.push({
                    id: 'propertyDescription',
                    label: 'Açıklama (en az 300 karakter)',
                    isComplete: function () {
                        return (descEl.value || '').trim().length >= 300;
                    },
                    targetEl: descEl
                });
            }

            // 3. Fiyat veya Kira dolu
            var priceEl = document.getElementById('priceInput');
            var rentEl = document.getElementById('rentInput');
            if (priceEl || rentEl) {
                items.push({
                    id: priceEl ? 'priceInput' : 'rentInput',
                    label: priceEl ? 'Satış fiyatı' : 'Kira bedeli',
                    isComplete: function () {
                        if (priceEl) {
                            var pVal = parseFloat(unformatMoney(priceEl.value));
                            if (!isNaN(pVal) && pVal > 0) return true;
                        }
                        if (rentEl) {
                            var rVal = parseFloat(unformatMoney(rentEl.value));
                            if (!isNaN(rVal) && rVal > 0) return true;
                        }
                        return false;
                    },
                    targetEl: priceEl || rentEl
                });
            }

            // 4. Brüt alan (konut/işyeri) ya da Alan (arsa) dolu
            var grossAreaEl = document.getElementById('grossArea');
            var areaEl = document.getElementById('areaInput');
            if (grossAreaEl || areaEl) {
                items.push({
                    id: grossAreaEl ? 'grossArea' : 'areaInput',
                    label: grossAreaEl ? 'Brüt alan' : 'Alan (m²)',
                    isComplete: function () {
                        if (grossAreaEl) {
                            var gVal = parseFloat(grossAreaEl.value);
                            if (!isNaN(gVal) && gVal > 0) return true;
                        }
                        if (areaEl) {
                            var aVal = parseFloat(areaEl.value);
                            if (!isNaN(aVal) && aVal > 0) return true;
                        }
                        return false;
                    },
                    targetEl: grossAreaEl || areaEl
                });
            }

            // 5. Oda sayısı (yalnızca konut: tür 2 ve 8)
            var roomEl = document.getElementById('numberOfRoom');
            if (isHousing && roomEl) {
                items.push({
                    id: 'numberOfRoom',
                    label: 'Oda sayısı seçimi',
                    isComplete: function () {
                        return !!(roomEl.value && roomEl.value.trim());
                    },
                    targetEl: roomEl
                });
            }

            // 6. İl + ilçe + mahalle seçili
            var citySelect = document.getElementById('citySelect');
            var districtSelect = document.getElementById('districtSelect');
            var neighborhoodSelect = document.getElementById('neighborhoodSelect');
            var cityInput = document.getElementById('cityInput');
            var districtInput = document.getElementById('districtInput');
            var neighborhoodInput = document.getElementById('neighborhoodInput');

            if (citySelect || cityInput) {
                items.push({
                    id: 'sec-location',
                    label: 'İl, ilçe ve mahalle seçimi',
                    isComplete: function () {
                        var hasCity = (cityInput && cityInput.value.trim()) || (citySelect && citySelect.value);
                        var hasDistrict = (districtInput && districtInput.value.trim()) || (districtSelect && districtSelect.value);
                        var hasNeighborhood = (neighborhoodInput && neighborhoodInput.value.trim()) || (neighborhoodSelect && neighborhoodSelect.value);
                        return !!(hasCity && hasDistrict && hasNeighborhood);
                    },
                    targetEl: (!citySelect || !citySelect.value) ? citySelect : ((!districtSelect || !districtSelect.value) ? districtSelect : (neighborhoodSelect || citySelect))
                });
            }

            // 7. En az 5 fotoğraf (yalnızca düzenlemede; #photoGrid .photo-item sayısı, MutationObserver ile canlı)
            var photoGrid = document.getElementById('photoGrid');
            if (!isNew && photoGrid) {
                items.push({
                    id: 'sec-photos',
                    label: 'En az 5 fotoğraf yüklenmeli',
                    isComplete: function () {
                        return photoGrid.querySelectorAll('.photo-item').length >= 5;
                    },
                    targetEl: document.getElementById('sec-photos') || photoGrid
                });
            }

            // 8. En az 1 kat planı/kroki (yalnızca konut ve düzenlemede; .photo-item .kind-select değeri ≠ 0)
            if (!isNew && isHousing && photoGrid) {
                items.push({
                    id: 'sec-photos-floorplan',
                    label: 'En az 1 kat planı veya kroki',
                    isComplete: function () {
                        var kindSelects = Array.from(photoGrid.querySelectorAll('.photo-item .kind-select'));
                        return kindSelects.some(function (s) {
                            return s.value && s.value !== '0';
                        });
                    },
                    targetEl: document.getElementById('sec-photos') || photoGrid
                });
            }

            // 9. En az 3 olanak işaretli (olanak kartı varsa)
            var amenitiesSec = document.getElementById('sec-amenities');
            if (amenitiesSec) {
                items.push({
                    id: 'sec-amenities',
                    label: 'En az 3 olanak seçilmeli',
                    isComplete: function () {
                        return amenitiesSec.querySelectorAll('input[name="AmenityIds"]:checked').length >= 3;
                    },
                    targetEl: amenitiesSec
                });
            }

            return items;
        }

        function updateCompleteness() {
            var items = getChecklistItems();
            if (items.length === 0) return;

            var completedCount = 0;
            var missingItems = [];

            items.forEach(function (item) {
                if (item.isComplete()) {
                    completedCount++;
                } else {
                    missingItems.push(item);
                }
            });

            var percent = Math.round((completedCount / items.length) * 100);

            if (completenessContainer) {
                completenessContainer.setAttribute('data-percent', percent);
            }
            if (completenessValEl) {
                completenessValEl.textContent = percent;
            }
            if (completenessBar) {
                var totalCircumference = 138.2;
                var offset = totalCircumference - (totalCircumference * percent / 100);
                completenessBar.style.strokeDashoffset = offset;
            }

            if (completenessList) {
                completenessList.innerHTML = '';
                if (missingItems.length === 0) {
                    var liDone = document.createElement('li');
                    liDone.className = 'dropdown-item-text text-success small fw-semibold';
                    liDone.innerHTML = '<i class="bi bi-check-circle-fill me-1"></i> Her şey tamam';
                    completenessList.appendChild(liDone);
                } else {
                    missingItems.forEach(function (item) {
                        var li = document.createElement('li');
                        var btn = document.createElement('button');
                        btn.type = 'button';
                        btn.className = 'dropdown-item small text-danger d-flex align-items-center gap-2';
                        btn.innerHTML = '<i class="bi bi-exclamation-circle flex-shrink-0"></i> <span>' + item.label + '</span>';
                        btn.addEventListener('click', function () {
                            var target = item.targetEl;
                            if (target) {
                                if (isWizard && typeof goToWizardStepFn === 'function') {
                                    var sec = target.closest('.listing-section');
                                    if (sec) {
                                        var idx = wizardSections.indexOf(sec);
                                        if (idx !== -1) {
                                            goToWizardStepFn(idx);
                                        }
                                    }
                                }
                                var prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
                                target.scrollIntoView({
                                    behavior: prefersReducedMotion ? 'auto' : 'smooth',
                                    block: 'center'
                                });
                                if (typeof target.focus === 'function') {
                                    target.focus();
                                }
                            }
                        });
                        li.appendChild(btn);
                        completenessList.appendChild(li);
                    });
                }
            }
        }

        var completenessDebounceTimer = null;
        function debouncedUpdateCompleteness() {
            clearTimeout(completenessDebounceTimer);
            completenessDebounceTimer = setTimeout(updateCompleteness, 150);
        }

        form.addEventListener('input', debouncedUpdateCompleteness);
        form.addEventListener('change', debouncedUpdateCompleteness);

        // Fotoğraf ızgarası değişikliklerini canlı izleme (MutationObserver)
        var photoGridEl = document.getElementById('photoGrid');
        if (photoGridEl) {
            var photoObserver = new MutationObserver(debouncedUpdateCompleteness);
            photoObserver.observe(photoGridEl, { childList: true, subtree: true });
            photoGridEl.addEventListener('change', debouncedUpdateCompleteness);
        }

        debouncedUpdateCompleteness();

        /* ==========================================================================
           5. BÖLÜM GEZİNME (SECTION NAV)
           ========================================================================== */
        var sectionNav = document.getElementById('sectionNav');
        if (sectionNav && !isWizard) {
            var chips = Array.from(sectionNav.querySelectorAll('.section-chip[data-section]'));
            var allSections = Array.from(document.querySelectorAll('.listing-section'));

            function updateSectionDoneStates() {
                chips.forEach(function (chip) {
                    var secId = chip.getAttribute('data-section');
                    var sec = document.getElementById(secId);
                    if (!sec) return;

                    var requiredFields = Array.from(sec.querySelectorAll('[required]'));
                    var isDone = false;
                    if (requiredFields.length > 0) {
                        isDone = requiredFields.every(function (field) {
                            return field.checkValidity() && field.value.trim() !== '';
                        });
                    } else {
                        var allFields = Array.from(sec.querySelectorAll('input, select, textarea'));
                        isDone = allFields.length === 0 || allFields.every(function (f) {
                            return f.checkValidity();
                        });
                    }
                    chip.classList.toggle('done', isDone);
                });
            }

            if ('IntersectionObserver' in window && allSections.length > 0) {
                var observer = new IntersectionObserver(function (entries) {
                    var bestEntry = null;
                    entries.forEach(function (entry) {
                        if (entry.isIntersecting) {
                            if (!bestEntry || entry.intersectionRatio > bestEntry.intersectionRatio) {
                                bestEntry = entry;
                            }
                        }
                    });

                    if (bestEntry && bestEntry.target) {
                        chips.forEach(function (chip) {
                            chip.classList.toggle('active', chip.getAttribute('data-section') === bestEntry.target.id);
                        });
                    }
                }, {
                    root: null,
                    threshold: [0.1, 0.3, 0.6]
                });

                allSections.forEach(function (sec) {
                    observer.observe(sec);
                });
            }

            chips.forEach(function (chip) {
                chip.addEventListener('click', function (e) {
                    e.preventDefault();
                    var secId = this.getAttribute('data-section');
                    var targetSec = document.getElementById(secId);
                    if (targetSec) {
                        var prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
                        targetSec.scrollIntoView({
                            behavior: prefersReducedMotion ? 'auto' : 'smooth',
                            block: 'start'
                        });
                    }
                });
            });

            form.addEventListener('input', updateSectionDoneStates);
            form.addEventListener('change', updateSectionDoneStates);
            updateSectionDoneStates();
        }

        /* ==========================================================================
           6. CANLI ÖNİZLEME (PREVIEW)
           ========================================================================== */
        var previewContainer = document.getElementById('listingPreview');
        function updatePreview() {
            if (!previewContainer) return;

            // Başlık
            var titleEl = document.getElementById('propertyName');
            var previewTitle = document.getElementById('previewTitle');
            if (previewTitle) {
                var val = titleEl && titleEl.value.trim() ? titleEl.value.trim() : 'İlan Başlığı';
                previewTitle.textContent = val;
                previewTitle.setAttribute('title', val);
            }

            // Konum
            var previewLocation = document.getElementById('previewLocation');
            if (previewLocation) {
                var citySelect = document.getElementById('citySelect');
                var districtSelect = document.getElementById('districtSelect');
                var neighborhoodSelect = document.getElementById('neighborhoodSelect');
                var cityInput = document.getElementById('cityInput');
                var districtInput = document.getElementById('districtInput');
                var neighborhoodInput = document.getElementById('neighborhoodInput');

                function getSelectText(sel, inp) {
                    if (sel && sel.selectedIndex > 0) {
                        var opt = sel.options[sel.selectedIndex];
                        var txt = opt.getAttribute('data-text') || opt.text;
                        if (txt && !txt.includes('Seçiniz') && !txt.includes('Yükleniyor')) return txt;
                    }
                    if (inp && inp.value && inp.value.trim()) return inp.value.trim();
                    return '';
                }

                var city = getSelectText(citySelect, cityInput);
                var district = getSelectText(districtSelect, districtInput);
                var neighborhood = getSelectText(neighborhoodSelect, neighborhoodInput);

                var locParts = [];
                if (neighborhood) locParts.push(neighborhood);
                if (district) locParts.push(district);
                if (city) locParts.push(city);

                var locText = locParts.length > 0 ? locParts.join(', ') : 'Konum belirtilmedi';
                var span = previewLocation.querySelector('span');
                if (span) {
                    span.textContent = locText;
                }
            }

            // Fiyat / Kira
            var previewPrice = document.getElementById('previewPrice');
            if (previewPrice) {
                var priceInput = document.getElementById('priceInput');
                var rentInput = document.getElementById('rentInput');
                var priceText = 'Fiyat belirtilmedi';

                if (priceInput && priceInput.value.trim()) {
                    priceText = priceInput.value.trim() + ' ₺';
                } else if (rentInput && rentInput.value.trim()) {
                    priceText = rentInput.value.trim() + ' ₺';
                }
                previewPrice.textContent = priceText;
            }

            // Meta (Oda + Alan + Kat)
            var previewMeta = document.getElementById('previewMeta');
            if (previewMeta) {
                var metaParts = [];
                var roomEl = document.getElementById('numberOfRoom');
                if (roomEl && roomEl.value.trim()) {
                    metaParts.push(roomEl.value.trim());
                }

                var grossAreaEl = document.getElementById('grossArea');
                var netAreaEl = document.getElementById('netArea');
                var areaEl = document.getElementById('areaInput');

                if (grossAreaEl && grossAreaEl.value.trim() && parseFloat(grossAreaEl.value) > 0) {
                    metaParts.push(grossAreaEl.value.trim() + ' m²');
                } else if (netAreaEl && netAreaEl.value.trim() && parseFloat(netAreaEl.value) > 0) {
                    metaParts.push(netAreaEl.value.trim() + ' m²');
                } else if (areaEl && areaEl.value.trim() && parseFloat(areaEl.value) > 0) {
                    metaParts.push(areaEl.value.trim() + ' m²');
                }

                var floorEl = document.getElementById('numberOfFloors');
                if (floorEl && floorEl.value.trim()) {
                    metaParts.push(floorEl.value.trim() + '. Kat');
                }

                previewMeta.textContent = metaParts.length > 0 ? metaParts.join(' · ') : 'Özellikler girilmedi';
            }
        }

        form.addEventListener('input', updatePreview);
        form.addEventListener('change', updatePreview);
        updatePreview();

        /* ==========================================================================
           7. KAYDEDİLMEMİŞ DEĞİŞİKLİKLER, BEFOREUNLOAD VE CTRL+S KISAYOLU
           ========================================================================== */
        var dirtyIndicator = document.getElementById('dirtyIndicator');
        if (dirtyIndicator) {
            dirtyIndicator.setAttribute('aria-live', 'polite');
        }

        var isFormSubmitting = false;
        function getFormSnapshot() {
            var formData = new FormData(form);
            formData.delete('__RequestVerificationToken');
            var entries = [];
            formData.forEach(function (val, key) {
                entries.push(key + '=' + val);
            });
            return entries.sort().join('&');
        }

        var initialFormSnapshot = null;
        // Konum ve diğer başlangıç AJAX yüklemelerinden sonra ilk snapshot'ı al
        setTimeout(function () {
            initialFormSnapshot = getFormSnapshot();
        }, 800);

        function checkDirty() {
            if (!initialFormSnapshot || isFormSubmitting) return;
            var currentSnapshot = getFormSnapshot();
            var isDirty = (currentSnapshot !== initialFormSnapshot);
            if (dirtyIndicator) {
                dirtyIndicator.classList.toggle('d-none', !isDirty);
            }
        }

        form.addEventListener('input', checkDirty);
        form.addEventListener('change', checkDirty);

        window.addEventListener('beforeunload', function (e) {
            if (isFormSubmitting || !initialFormSnapshot) return;
            if (getFormSnapshot() !== initialFormSnapshot) {
                e.preventDefault();
                e.returnValue = '';
                return '';
            }
        });

        form.addEventListener('submit', function () {
            isFormSubmitting = true;
            if (dirtyIndicator) {
                dirtyIndicator.classList.add('d-none');
            }
        }, true);

        // Ctrl+S / Cmd+S kısayolu
        document.addEventListener('keydown', function (e) {
            if ((e.ctrlKey || e.metaKey) && (e.key === 's' || e.key === 'S')) {
                e.preventDefault();
                if (typeof form.requestSubmit === 'function') {
                    form.requestSubmit();
                } else {
                    form.submit();
                }
            }
        });

        /* ==========================================================================
           8. SİHİRBAZ MODU (WIZARD - YALNIZCA YENİ İLAN)
           ========================================================================== */
        if (isWizard) {
            var wizardSteps = document.getElementById('wizardSteps');
            var stepPrev = document.getElementById('stepPrev');
            var stepNext = document.getElementById('stepNext');
            var submitBtn = document.getElementById('submitBtn');

            if (wizardSteps && stepPrev && stepNext && submitBtn && wizardSections.length > 0) {
                if (sectionNav) {
                    sectionNav.classList.add('d-none');
                }

                var stepItems = Array.from(wizardSteps.querySelectorAll('li[data-step-for]'));

                wizardSteps.classList.remove('d-none');
                wizardSteps.setAttribute('role', 'list');
                wizardSteps.setAttribute('aria-label', 'Yeni İlan Adımları');
                stepPrev.classList.remove('d-none');
                stepNext.classList.remove('d-none');
                submitBtn.classList.add('d-none');

                var currentStep = 0;

                // Sunucu doğrulama hatası (.is-invalid) kontrolü
                var firstInvalid = form.querySelector('.is-invalid');
                if (firstInvalid) {
                    var errorSec = firstInvalid.closest('.listing-section');
                    var errorIdx = wizardSections.indexOf(errorSec);
                    if (errorIdx !== -1) {
                        currentStep = errorIdx;
                        for (var i = 0; i < currentStep; i++) {
                            if (stepItems[i]) stepItems[i].classList.add('done');
                        }
                    }
                }

                function goToStep(index) {
                    if (index < 0 || index >= wizardSections.length) return;
                    currentStep = index;

                    wizardSections.forEach(function (sec, idx) {
                        sec.classList.toggle('d-none', idx !== currentStep);
                    });

                    stepItems.forEach(function (li, idx) {
                        li.classList.toggle('active', idx === currentStep);
                        if (idx === currentStep) {
                            li.setAttribute('aria-current', 'step');
                        } else {
                            li.removeAttribute('aria-current');
                        }

                        var isClickable = (idx < currentStep || li.classList.contains('done'));
                        if (isClickable) {
                            li.setAttribute('tabindex', '0');
                            li.setAttribute('role', 'button');
                            li.style.cursor = 'pointer';
                        } else {
                            li.removeAttribute('tabindex');
                            li.removeAttribute('role');
                            li.style.cursor = 'default';
                        }
                    });

                    stepPrev.disabled = (currentStep === 0);
                    var isLastStep = (currentStep === wizardSections.length - 1);
                    stepNext.classList.toggle('d-none', isLastStep);
                    submitBtn.classList.toggle('d-none', !isLastStep);

                    // Son adımda önizleme kartı görünür
                    if (previewContainer) {
                        previewContainer.classList.toggle('d-none', !isLastStep);
                    }

                    var prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
                    window.scrollTo({ top: 0, behavior: prefersReducedMotion ? 'auto' : 'smooth' });
                }

                goToWizardStepFn = goToStep;

                function findFieldLabel(el) {
                    if (!el) return null;
                    if (el.id) {
                        var escapedId = (window.CSS && CSS.escape) ? CSS.escape(el.id) : el.id.replace(/["\\]/g, '\\$&');
                        var labelFor = document.querySelector('label[for="' + escapedId + '"]');
                        if (labelFor) return labelFor;
                    }
                    var parent = el.closest('.form-group, .mb-3, .col, [class*="col-"], .form-floating, .form-check, div');
                    if (parent) {
                        var labelInParent = parent.querySelector('label');
                        if (labelInParent) return labelInParent;
                    }
                    return null;
                }

                function getFieldLabelText(field) {
                    var label = findFieldLabel(field);
                    var text = '';
                    if (label) {
                        var clone = label.cloneNode(true);
                        clone.querySelectorAll('.req-star, .help-tip, .text-danger, script, style').forEach(function (n) {
                            n.remove();
                        });
                        text = (clone.textContent || '').trim().replace(/\s*\*$/, '').trim();
                    }
                    if (!text) {
                        text = field.getAttribute('placeholder') || field.getAttribute('name') || field.id || 'Zorunlu alan';
                    }
                    return text;
                }

                stepNext.addEventListener('click', function () {
                    var curSec = wizardSections[currentStep];
                    var fields = Array.from(curSec.querySelectorAll('input, select, textarea'));
                    var invalidFields = [];
                    var firstInvalid = null;

                    for (var i = 0; i < fields.length; i++) {
                        var field = fields[i];
                        if (typeof field.checkValidity === 'function' && !field.checkValidity()) {
                            invalidFields.push(field);
                            if (!firstInvalid) {
                                firstInvalid = field;
                            }
                        }
                    }

                    if (firstInvalid) {
                        if (typeof firstInvalid.reportValidity === 'function') {
                            firstInvalid.reportValidity();
                        }
                        if (typeof window.fibiToast === 'function') {
                            var items = invalidFields.map(function (f) {
                                return getFieldLabelText(f);
                            }).filter(function (t) {
                                return !!t;
                            });
                            window.fibiToast('Bu adımda eksik bilgi var.', 'warning', {
                                title: 'Eksik bilgi',
                                items: items
                            });
                        }
                        return;
                    }

                    if (currentStep < wizardSections.length - 1) {
                        if (stepItems[currentStep]) {
                            stepItems[currentStep].classList.add('done');
                        }
                        goToStep(currentStep + 1);
                    }
                });

                stepPrev.addEventListener('click', function () {
                    if (currentStep > 0) {
                        goToStep(currentStep - 1);
                    }
                });

                stepItems.forEach(function (li, idx) {
                    function handleStepTrigger() {
                        if (idx < currentStep || li.classList.contains('done')) {
                            goToStep(idx);
                        }
                    }

                    li.addEventListener('click', handleStepTrigger);
                    li.addEventListener('keydown', function (e) {
                        if (e.key === 'Enter' || e.key === ' ') {
                            e.preventDefault();
                            handleStepTrigger();
                        }
                    });
                });

                goToStep(currentStep);
            }
        }

        /* ==========================================================================
           8.1 FİBİ:REVEAL DİNLİYİCİSİ (SİHİRBAZ / DÜZENLEME BÖLÜM GEÇİŞİ)
           ========================================================================== */
        document.addEventListener('fibi:reveal', function (e) {
            var targetEl = (e && e.detail && e.detail.element) || (e && e.detail && e.detail.nodeType ? e.detail : null);
            if (!targetEl) return;

            var sec = (targetEl.matches && targetEl.matches('.listing-section'))
                ? targetEl
                : (targetEl.closest ? targetEl.closest('.listing-section') : null);
            if (!sec) return;

            if (isWizard) {
                var idx = wizardSections.indexOf(sec);
                if (idx !== -1 && typeof goToWizardStepFn === 'function') {
                    goToWizardStepFn(idx);
                }
            } else {
                var prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
                sec.scrollIntoView({
                    behavior: prefersReducedMotion ? 'auto' : 'smooth',
                    block: 'start'
                });
            }
        });

        /* ==========================================================================
           M² FİYAT ÖNERİSİ TAMAMLAYICI (listing-form.js entegrasyonu)
           ========================================================================== */
        var priceInputForM2 = document.getElementById('priceInput');
        var rentInputForM2 = document.getElementById('rentInput');
        var areaInputForM2 = document.getElementById('areaInput');
        var m2PriceInput = document.getElementById('m2PriceInput');

        if (m2PriceInput && areaInputForM2 && (priceInputForM2 || rentInputForM2)) {
            var activePriceInput = priceInputForM2 || rentInputForM2;
            var userChangedM2 = !!m2PriceInput.value;

            m2PriceInput.addEventListener('input', function () {
                userChangedM2 = true;
            });

            function recalculateM2() {
                if (userChangedM2) return;
                var rawPrice = parseFloat(unformatMoney(activePriceInput.value));
                var rawArea = parseFloat(areaInputForM2.value);
                if (!isNaN(rawPrice) && rawPrice > 0 && !isNaN(rawArea) && rawArea > 0) {
                    var unit = Math.round(rawPrice / rawArea);
                    m2PriceInput.value = formatMoneyValue(unit.toString());
                    m2PriceInput.dispatchEvent(new Event('input', { bubbles: true }));
                }
            }

            activePriceInput.addEventListener('input', recalculateM2);
            areaInputForM2.addEventListener('input', recalculateM2);
        }
    });
})();
