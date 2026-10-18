(function () {
    'use strict';

    var F = window.FibiServices;
    if (!F) {
        console.error('[FibiRiskAnalysis] FibiServices modülü yüklenemedi.');
        return;
    }

    var riskChartInstance = null;

    function init() {
        var form = document.getElementById('riskForm');
        var submitBtn = document.getElementById('riskSubmitBtn');
        var resultPanel = document.getElementById('riskResultPanel');
        var totalFloorsEl = document.getElementById('riskTotalFloors');
        var currentFloorEl = document.getElementById('riskCurrentFloor');
        var totalFloorsValEl = document.getElementById('riskTotalFloorsValue');
        var currentFloorValEl = document.getElementById('riskCurrentFloorValue');
        var placeholderEl = document.getElementById('riskPlaceholder');
        var resultEl = document.getElementById('riskResult');
        var locationTitleEl = document.getElementById('riskLocationTitle');
        var scoreEl = document.getElementById('riskScore');
        var badgeEl = document.getElementById('riskLevelBadge');
        var transScoreEl = document.getElementById('transformationScore');
        var transBarEl = document.getElementById('transformationBar');
        var transVerdictEl = document.getElementById('transformationVerdict');
        var insightsList = document.getElementById('riskInsights');
        var disclaimerTextEl = document.getElementById('riskDisclaimerText');
        var listingsLink = document.getElementById('riskListingsLink');
        var leadBtn = document.getElementById('riskLeadBtn');

        // 1. Konum kademeli seçim bağlamı
        F.bindLocationCascade({
            city: '#riskCity',
            district: '#riskDistrict',
            neighborhood: '#riskNeighborhood'
        }).catch(function (err) {
            console.error('[FibiRiskAnalysis] Konum listesi yüklenemedi:', err);
            if (window.toastr) {
                window.toastr.error('Konum listesi yüklenemedi.');
            }
        });

        // 2. Kat bilgisi range formatlayıcıları
        function formatCurrentFloor(val) {
            var v = parseInt(val, 10);
            if (isNaN(v)) return val;
            if (v < 0) {
                return 'Bodrum (' + v + ')';
            }
            if (v === 0) {
                return 'Zemin Kat';
            }
            return v + '. Kat';
        }

        function formatTotalFloors(val) {
            var v = parseInt(val, 10);
            if (isNaN(v)) return val;
            return v + ' Kat';
        }

        F.initRange(totalFloorsEl, totalFloorsValEl, formatTotalFloors);
        F.initRange(currentFloorEl, currentFloorValEl, formatCurrentFloor);

        // 3. Toplam kat değiştiğinde mevcut kat maksimumunu dinamik güncelleme
        function syncCurrentFloorMax() {
            if (!totalFloorsEl || !currentFloorEl) return;
            var total = parseInt(totalFloorsEl.value, 10);
            if (isNaN(total)) return;

            currentFloorEl.max = String(total);
            var current = parseInt(currentFloorEl.value, 10);
            if (current > total) {
                currentFloorEl.value = String(total);
                currentFloorEl.dispatchEvent(new Event('input', { bubbles: true }));
            }
        }

        if (totalFloorsEl) {
            totalFloorsEl.addEventListener('input', syncCurrentFloorMax);
            totalFloorsEl.addEventListener('change', syncCurrentFloorMax);
            syncCurrentFloorMax();
        }

        // 4. Form gönderim olayı
        if (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                var neighSelect = document.getElementById('riskNeighborhood');
                var neighVal = neighSelect ? neighSelect.value : '';
                if (!neighVal) {
                    if (window.toastr) {
                        window.toastr.warning('Lütfen il, ilçe ve mahalle seçin.');
                    }
                    return;
                }

                var yearRadio = form.querySelector('input[name="constructionYearRange"]:checked');
                if (!yearRadio) {
                    if (window.toastr) {
                        window.toastr.warning('Lütfen binanın yapım yılı aralığını seçin.');
                    }
                    return;
                }

                var structureRadio = form.querySelector('input[name="structureType"]:checked');
                var commercialCheck = form.querySelector('input[name="hasGroundFloorCommercial"]');
                var adjacentCheck = form.querySelector('input[name="isAdjacentBuilding"]');
                var basementCheck = form.querySelector('input[name="hasBasementOrParking"]');

                var body = {
                    neighborhoodId: parseInt(neighVal, 10),
                    constructionYearRange: parseInt(yearRadio.value, 10),
                    totalFloors: totalFloorsEl ? parseInt(totalFloorsEl.value, 10) : 5,
                    currentFloor: currentFloorEl ? parseInt(currentFloorEl.value, 10) : 1,
                    structureType: structureRadio ? parseInt(structureRadio.value, 10) : 1,
                    hasGroundFloorCommercial: commercialCheck ? commercialCheck.checked : false,
                    isAdjacentBuilding: adjacentCheck ? adjacentCheck.checked : false,
                    hasBasementOrParking: basementCheck ? basementCheck.checked : false
                };

                if (submitBtn) {
                    submitBtn.classList.add('fibi-loading');
                    submitBtn.disabled = true;
                }
                if (resultPanel) {
                    resultPanel.classList.add('fibi-loading');
                }

                F.postJson('FibiServices/risk-analysis', body)
                    .then(function (res) {
                        if (res && res.ok && res.data) {
                            render(res.data, body);
                        }
                    })
                    .finally(function () {
                        if (submitBtn) {
                            submitBtn.classList.remove('fibi-loading');
                            submitBtn.disabled = false;
                        }
                        if (resultPanel) {
                            resultPanel.classList.remove('fibi-loading');
                        }
                    });
            });
        }

        // 5. Analiz sonucu görselleştirme fonksiyonu
        function render(data, body) {
            if (!data) return;

            // Bekleme alanını gizle, sonuç alanını göster
            if (placeholderEl) {
                placeholderEl.hidden = true;
                placeholderEl.setAttribute('hidden', '');
                placeholderEl.style.display = 'none';
            }
            if (resultEl) {
                resultEl.hidden = false;
                resultEl.removeAttribute('hidden');
                resultEl.style.display = 'block';
            }

            // Lokasyon başlığı
            var loc = data.location || {};
            var neighborhoodName = loc.neighborhoodName || '';
            var districtName = loc.districtName || '';
            var cityName = loc.cityName || '';
            if (locationTitleEl) {
                locationTitleEl.textContent = neighborhoodName + ', ' + districtName + ' / ' + cityName;
            }

            // Gauge (Chart.js doughnut yarım daire)
            var rootStyles = window.getComputedStyle(document.documentElement);
            function getCssColor(varName, fallback) {
                var v = rootStyles.getPropertyValue(varName);
                return (v && v.trim().length > 0) ? v.trim() : fallback;
            }

            var colorPrimary = getCssColor('--fibi-primary', '#2dbe6c');
            var colorWarning = getCssColor('--fibi-warning', '#f2b241');
            var colorOrange = getCssColor('--fibi-orange', '#f28c28');
            var colorDanger = getCssColor('--fibi-danger', '#f94c4c');
            var colorBorder = getCssColor('--fibi-border', '#e5e7eb');

            var score = typeof data.structuralRiskScore === 'number'
                ? data.structuralRiskScore
                : parseInt(data.structuralRiskScore, 10);
            if (isNaN(score)) score = 0;

            var code = (data.riskLevelCode || 'low').toLowerCase();
            var gaugeColor = colorPrimary;
            if (code === 'medium') {
                gaugeColor = colorWarning;
            } else if (code === 'high') {
                gaugeColor = colorOrange;
            } else if (code === 'critical') {
                gaugeColor = colorDanger;
            }

            if (riskChartInstance) {
                riskChartInstance.destroy();
                riskChartInstance = null;
            }

            var canvas = document.getElementById('riskGauge');
            if (canvas && window.Chart) {
                riskChartInstance = new window.Chart(canvas, {
                    type: 'doughnut',
                    data: {
                        datasets: [{
                            data: [score, Math.max(0, 100 - score)],
                            backgroundColor: [gaugeColor, colorBorder],
                            borderWidth: 0
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        rotation: -90,
                        circumference: 180,
                        cutout: '75%',
                        animation: {
                            duration: 800
                        },
                        plugins: {
                            legend: {
                                display: false
                            },
                            tooltip: {
                                enabled: false
                            }
                        }
                    }
                });
            }

            // Skor ve Risk Seviyesi Rozeti
            if (scoreEl) {
                scoreEl.textContent = String(score);
            }
            if (badgeEl) {
                badgeEl.textContent = data.riskLevel || '';
                badgeEl.className = 'fibi-badge fibi-badge--' + code;
            }

            // Kentsel Dönüşüm Potansiyeli
            var transScore = typeof data.urbanTransformationPotentialScore === 'number'
                ? data.urbanTransformationPotentialScore
                : parseInt(data.urbanTransformationPotentialScore, 10);
            if (isNaN(transScore)) transScore = 0;

            if (transScoreEl) {
                transScoreEl.textContent = String(transScore);
            }
            if (transBarEl) {
                transBarEl.style.width = transScore + '%';
            }
            if (transVerdictEl) {
                transVerdictEl.textContent = data.urbanTransformationVerdict || '';
            }

            // Uzman Bulguları listesi
            if (insightsList) {
                while (insightsList.firstChild) {
                    insightsList.removeChild(insightsList.firstChild);
                }

                var iconMap = {
                    info: 'fas fa-info-circle',
                    success: 'fas fa-check-circle',
                    warning: 'fas fa-exclamation-triangle',
                    danger: 'fas fa-times-circle'
                };

                var insights = Array.isArray(data.actionableInsights) ? data.actionableInsights : [];
                insights.forEach(function (item) {
                    if (!item) return;
                    var rawSev = (item.severity || '').toLowerCase();
                    var sev = Object.prototype.hasOwnProperty.call(iconMap, rawSev) ? rawSev : 'info';
                    var iconClass = iconMap[sev];

                    var li = document.createElement('li');
                    li.className = 'fibi-insight fibi-insight--' + sev;

                    var icon = document.createElement('i');
                    icon.className = iconClass;

                    var span = document.createElement('span');
                    span.textContent = item.message || '';

                    li.appendChild(icon);
                    li.appendChild(span);
                    insightsList.appendChild(li);
                });
            }

            // Yasal Uyarı Metni
            if (disclaimerTextEl) {
                disclaimerTextEl.textContent = data.legalDisclaimer || '';
            }

            // İlanlar Bağlantısı
            if (listingsLink) {
                listingsLink.href = F.buildListingsUrl(data.location);
            }

            // Lead formu bağlamını güncelle
            var targetNeighborhoodId = (data.location && data.location.neighborhoodId != null)
                ? data.location.neighborhoodId
                : body.neighborhoodId;

            F.setLeadContext(targetNeighborhoodId, {
                module: 'RiskAnalysis',
                input: body,
                result: {
                    structuralRiskScore: data.structuralRiskScore,
                    riskLevel: data.riskLevel,
                    urbanTransformationPotentialScore: data.urbanTransformationPotentialScore,
                    urbanTransformationVerdict: data.urbanTransformationVerdict
                }
            });

            // Küçük ekranlarda (992px altı) sonuç paneline yumuşak kaydır
            if (window.innerWidth < 992 && resultPanel) {
                resultPanel.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        }

        // 6. Lead Danışmanlık buton tıklaması
        if (leadBtn) {
            leadBtn.addEventListener('click', function () {
                F.scrollToLead();
            });
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
