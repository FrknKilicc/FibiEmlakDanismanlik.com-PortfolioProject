(function () {
    'use strict';

    var F = window.FibiServices;
    if (!F) {
        console.error('[FibiNeighborhoodAnalysis] FibiServices modülü yüklenemedi.');
        return;
    }

    var regionChartInstance = null;

    function init() {
        var form = document.getElementById('regionForm');
        var submitBtn = document.getElementById('regionSubmitBtn');
        var resultPanel = document.getElementById('regionResultPanel');
        var placeholderEl = document.getElementById('regionPlaceholder');
        var resultEl = document.getElementById('regionResult');
        var neighSelect = document.getElementById('regionNeighborhood');
        var compareSelect = document.getElementById('regionCompare');
        var locationTitleEl = document.getElementById('regionLocationTitle');
        var gradeEl = document.getElementById('regionGrade');
        var gradeSubEl = document.getElementById('regionGradeSub');
        var bandBalancedEl = document.getElementById('regionBandBalanced');
        var bandSummaryEl = document.getElementById('regionBandSummary');
        var amortEl = document.getElementById('regionAmortization');
        var amortSubEl = document.getElementById('regionAmortizationSub');
        var growthEl = document.getElementById('regionGrowth');
        var growthSubEl = document.getElementById('regionGrowthSub');
        var bandMinEl = document.getElementById('regionBandMin');
        var bandMidEl = document.getElementById('regionBandMid');
        var bandMaxEl = document.getElementById('regionBandMax');
        var chartCanvas = document.getElementById('regionChart');
        var compareBlockEl = document.getElementById('regionCompareBlock');
        var compareHeadA = document.getElementById('regionCompareHeadA');
        var compareHeadB = document.getElementById('regionCompareHeadB');
        var compareBody = document.getElementById('regionCompareBody');
        var disclaimerTextEl = document.getElementById('regionDisclaimerText');
        var listingsLink = document.getElementById('regionListingsLink');
        var leadBtn = document.getElementById('regionLeadBtn');

        // 1. Konum kademeli seçim bağlamı
        F.bindLocationCascade({
            city: '#regionCity',
            district: '#regionDistrict',
            neighborhood: '#regionNeighborhood'
        }).catch(function (err) {
            console.error('[FibiNeighborhoodAnalysis] Konum listesi yüklenemedi:', err);
            if (window.toastr) {
                window.toastr.error('Konum listesi yüklenemedi.');
            }
        });

        // 2. Karşılaştırma konumu kademeli seçim bağlamı
        F.bindLocationCascade({
            city: '#regionCompareCity',
            district: '#regionCompareDistrict',
            neighborhood: '#regionCompare',
            excludeNeighborhoodId: function () {
                return neighSelect ? neighSelect.value : '';
            }
        }).catch(function (err) {
            console.error('[FibiNeighborhoodAnalysis] Konum listesi yüklenemedi:', err);
            if (window.toastr) {
                window.toastr.error('Konum listesi yüklenemedi.');
            }
        });

        // 3. Form gönderim olayı
        if (form) {
            form.addEventListener('submit', function (e) {
                e.preventDefault();

                var neighVal = neighSelect ? neighSelect.value : '';
                if (!neighVal) {
                    if (window.toastr) {
                        window.toastr.warning('Lütfen il, ilçe ve mahalle seçin.');
                    }
                    return;
                }

                var segmentRadio = form.querySelector('input[name="propertySegment"]:checked');
                var segmentVal = segmentRadio ? parseInt(segmentRadio.value, 10) : 1;

                var compVal = compareSelect ? compareSelect.value : '';
                var compareId = compVal ? parseInt(compVal, 10) : null;

                if (compareId && parseInt(neighVal, 10) === compareId) {
                    if (window.toastr) {
                        window.toastr.warning('Karşılaştırma için farklı bir mahalle seçin.');
                    }
                    return;
                }

                var body = {
                    neighborhoodId: parseInt(neighVal, 10),
                    compareWithNeighborhoodId: compareId,
                    propertySegment: segmentVal
                };

                if (submitBtn) {
                    submitBtn.classList.add('fibi-loading');
                    submitBtn.disabled = true;
                }
                if (resultPanel) {
                    resultPanel.classList.add('fibi-loading');
                }

                F.postJson('FibiServices/neighborhood-analysis', body)
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

        // 4. Analiz sonucunu ekrana basma
        function render(data, body) {
            if (!data || !data.primary) return;

            var primary = data.primary;
            var comp = data.comparison;
            var pLoc = primary.location || {};

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
            var titleText = (pLoc.neighborhoodName || '') + ', ' + (pLoc.districtName || '') + ' / ' + (pLoc.cityName || '');
            if (comp && comp.location && comp.location.neighborhoodName) {
                titleText += ' · ' + comp.location.neighborhoodName + ' ile karşılaştırma';
            }
            if (locationTitleEl) {
                locationTitleEl.textContent = titleText;
            }

            F.renderDataSource('#regionDataSource', primary.dataSource);

            // a) Bölge Yatırım Notu kartı
            if (gradeEl) {
                gradeEl.textContent = primary.investmentGrade || '-';
            }
            if (gradeSubEl) {
                var scoreStr = primary.investmentScore != null ? primary.investmentScore : '-';
                gradeSubEl.textContent = 'Yatırım skoru: ' + scoreStr + ' / 100';
            }

            // b) Piyasa Emsal Bandı kartı
            var pBand = primary.priceBand || {};
            if (bandBalancedEl) {
                bandBalancedEl.textContent = 'Dengeli: ' + F.formatTL(pBand.marketBalancedSqmPrice);
            }
            if (bandSummaryEl) {
                bandSummaryEl.textContent = F.formatTL(pBand.marketMinSqmPrice) + ' – ' + F.formatTL(pBand.premiumPortfolioSqmPrice);
            }

            // c) Amortisman Süresi kartı
            if (amortEl) {
                amortEl.textContent = F.formatNumber(primary.amortizationYears, 1) + ' yıl';
            }
            if (amortSubEl) {
                var amortSubText = primary.amortizationVerdict || '';
                if (primary.annualRentYieldPercentage != null) {
                    amortSubText += (amortSubText ? ' · ' : '') + 'Brüt kira verimi ' + F.formatPercent(primary.annualRentYieldPercentage);
                }
                amortSubEl.textContent = amortSubText;
            }

            // d) 12 Aylık Değer Beklentisi kartı
            if (growthEl) {
                growthEl.textContent = F.formatPercent(primary.expected12MonthGrowthRate);
            }
            if (growthSubEl) {
                growthSubEl.textContent = 'Muhafazakâr ' + F.formatPercent(primary.conservative12MonthGrowthRate) + ' · İyimser ' + F.formatPercent(primary.optimistic12MonthGrowthRate);
            }

            // Fiyat bandı görseli değerleri
            if (bandMinEl) {
                bandMinEl.textContent = F.formatTL(pBand.marketMinSqmPrice);
            }
            if (bandMidEl) {
                bandMidEl.textContent = F.formatTL(pBand.marketBalancedSqmPrice);
            }
            if (bandMaxEl) {
                bandMaxEl.textContent = F.formatTL(pBand.premiumPortfolioSqmPrice);
            }

            // Grafik
            var rootStyles = window.getComputedStyle(document.documentElement);
            function getCssColor(varName, fallback) {
                var v = rootStyles.getPropertyValue(varName);
                return (v && v.trim().length > 0) ? v.trim() : fallback;
            }

            var colorPrimary = getCssColor('--fibi-primary', '#2dbe6c');
            var colorDark = getCssColor('--fibi-dark', '#1b1d21');
            var colorText = getCssColor('--fibi-text', '#93959e');
            var colorWarning = getCssColor('--fibi-warning', '#f2b241');

            var hist = Array.isArray(primary.historicalPrices) ? primary.historicalPrices : [];
            var proj = Array.isArray(primary.projection) ? primary.projection : [];

            var labels = [];
            hist.forEach(function (h) {
                if (h && h.quarter) labels.push(h.quarter);
            });
            proj.forEach(function (p) {
                if (p && p.quarter) labels.push(p.quarter);
            });

            // Geçmiş Trend verisi (projeksiyon kısmında null)
            var histData = [];
            hist.forEach(function (h) {
                histData.push(h ? h.price : null);
            });
            for (var i = 0; i < proj.length; i++) {
                histData.push(null);
            }

            // Projeksiyon serileri: son geçmiş noktasından başlar
            var lastHistPrice = (hist.length > 0 && hist[hist.length - 1]) ? hist[hist.length - 1].price : null;

            function buildProjectionSeries(key) {
                var arr = [];
                for (var j = 0; j < hist.length - 1; j++) {
                    arr.push(null);
                }
                if (hist.length > 0) {
                    arr.push(lastHistPrice);
                }
                proj.forEach(function (p) {
                    arr.push(p ? p[key] : null);
                });
                return arr;
            }

            var conservativeData = buildProjectionSeries('conservative');
            var balancedData = buildProjectionSeries('balanced');
            var optimisticData = buildProjectionSeries('optimistic');

            var datasets = [
                {
                    label: 'Geçmiş Trend',
                    data: histData,
                    borderColor: colorPrimary,
                    backgroundColor: colorPrimary,
                    borderDash: [],
                    fill: false,
                    spanGaps: false
                },
                {
                    label: 'Muhafazakâr',
                    data: conservativeData,
                    borderColor: colorText,
                    backgroundColor: colorText,
                    borderDash: [6, 4],
                    fill: false,
                    spanGaps: false
                },
                {
                    label: 'Dengeli',
                    data: balancedData,
                    borderColor: colorDark,
                    backgroundColor: colorDark,
                    borderDash: [6, 4],
                    fill: false,
                    spanGaps: false
                },
                {
                    label: 'İyimser',
                    data: optimisticData,
                    borderColor: colorPrimary,
                    backgroundColor: colorPrimary,
                    borderDash: [6, 4],
                    fill: false,
                    spanGaps: false
                }
            ];

            // Karşılaştırma serileri (varsa)
            if (comp) {
                var compLocName = (comp.location && comp.location.neighborhoodName) || 'Karşılaştırma';

                var compHistMap = {};
                if (Array.isArray(comp.historicalPrices)) {
                    comp.historicalPrices.forEach(function (h) {
                        if (h && h.quarter) compHistMap[h.quarter] = h.price;
                    });
                }

                var compProjMap = {};
                if (Array.isArray(comp.projection)) {
                    comp.projection.forEach(function (p) {
                        if (p && p.quarter) compProjMap[p.quarter] = p.balanced;
                    });
                }

                var compHistData = [];
                hist.forEach(function (h) {
                    var q = h ? h.quarter : '';
                    compHistData.push(compHistMap[q] !== undefined ? compHistMap[q] : null);
                });
                for (var ci = 0; ci < proj.length; ci++) {
                    compHistData.push(null);
                }

                var compProjData = [];
                for (var cj = 0; cj < hist.length - 1; cj++) {
                    compProjData.push(null);
                }
                if (hist.length > 0) {
                    var lastQ = hist[hist.length - 1] ? hist[hist.length - 1].quarter : '';
                    var compLastPrice = compHistMap[lastQ] !== undefined ? compHistMap[lastQ] : null;
                    compProjData.push(compLastPrice);
                }
                proj.forEach(function (p) {
                    var pq = p ? p.quarter : '';
                    compProjData.push(compProjMap[pq] !== undefined ? compProjMap[pq] : null);
                });

                datasets.push({
                    label: compLocName + ' – Geçmiş',
                    data: compHistData,
                    borderColor: colorWarning,
                    backgroundColor: colorWarning,
                    borderDash: [],
                    fill: false,
                    spanGaps: false
                });

                datasets.push({
                    label: compLocName + ' – Dengeli Projeksiyon',
                    data: compProjData,
                    borderColor: colorWarning,
                    backgroundColor: colorWarning,
                    borderDash: [6, 4],
                    fill: false,
                    spanGaps: false
                });
            }

            if (regionChartInstance) {
                regionChartInstance.destroy();
                regionChartInstance = null;
            }

            if (chartCanvas && window.Chart) {
                regionChartInstance = new window.Chart(chartCanvas, {
                    type: 'line',
                    data: {
                        labels: labels,
                        datasets: datasets
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        spanGaps: false,
                        interaction: {
                            mode: 'index',
                            intersect: false
                        },
                        elements: {
                            line: {
                                tension: 0.3
                            },
                            point: {
                                radius: 3
                            }
                        },
                        plugins: {
                            legend: {
                                position: 'bottom'
                            },
                            tooltip: {
                                callbacks: {
                                    label: function (context) {
                                        var label = context.dataset.label || '';
                                        var val = context.parsed.y;
                                        if (val === null || typeof val === 'undefined') return '';
                                        return (label ? (label + ': ') : '') + F.formatTL(val) + ' / m²';
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                ticks: {
                                    callback: function (value) {
                                        return F.formatNumber(value / 1000, 0) + ' B ₺';
                                    }
                                }
                            }
                        }
                    }
                });
            }

            // Karşılaştırma tablosu
            if (comp) {
                if (compareBlockEl) {
                    compareBlockEl.hidden = false;
                    compareBlockEl.removeAttribute('hidden');
                    compareBlockEl.style.display = 'block';
                }

                var pName = (pLoc && pLoc.neighborhoodName) || 'Seçili Mahalle';
                var cName = (comp.location && comp.location.neighborhoodName) || 'Karşılaştırma';

                if (compareHeadA) compareHeadA.textContent = pName;
                if (compareHeadB) compareHeadB.textContent = cName;

                if (compareBody) {
                    while (compareBody.firstChild) {
                        compareBody.removeChild(compareBody.firstChild);
                    }

                    var cBand = comp.priceBand || {};

                    var compMetrics = [
                        {
                            name: 'Veri Kapsamı',
                            valA: primary.dataSource ? primary.dataSource.scopeLabel : '-',
                            valB: comp.dataSource ? comp.dataSource.scopeLabel : '-'
                        },
                        {
                            name: 'Yatırım Notu',
                            valA: (primary.investmentGrade || '-') + ' (' + (primary.investmentScore != null ? primary.investmentScore : '-') + ')',
                            valB: (comp.investmentGrade || '-') + ' (' + (comp.investmentScore != null ? comp.investmentScore : '-') + ')'
                        },
                        {
                            name: 'Alt Emsal Bandı',
                            valA: F.formatTL(pBand.marketMinSqmPrice),
                            valB: F.formatTL(cBand.marketMinSqmPrice)
                        },
                        {
                            name: 'Piyasa Dengesi',
                            valA: F.formatTL(pBand.marketBalancedSqmPrice),
                            valB: F.formatTL(cBand.marketBalancedSqmPrice)
                        },
                        {
                            name: 'Nitelikli Üst Bant',
                            valA: F.formatTL(pBand.premiumPortfolioSqmPrice),
                            valB: F.formatTL(cBand.premiumPortfolioSqmPrice)
                        },
                        {
                            name: 'Amortisman',
                            valA: F.formatNumber(primary.amortizationYears, 1) + ' yıl',
                            valB: F.formatNumber(comp.amortizationYears, 1) + ' yıl'
                        },
                        {
                            name: 'Brüt Kira Verimi',
                            valA: F.formatPercent(primary.annualRentYieldPercentage),
                            valB: F.formatPercent(comp.annualRentYieldPercentage)
                        },
                        {
                            name: 'Son 1 Yıl Artış',
                            valA: F.formatPercent(primary.last1YearGrowthRate),
                            valB: F.formatPercent(comp.last1YearGrowthRate)
                        },
                        {
                            name: '12 Ay Beklenti (Dengeli)',
                            valA: F.formatPercent(primary.expected12MonthGrowthRate),
                            valB: F.formatPercent(comp.expected12MonthGrowthRate)
                        }
                    ];

                    compMetrics.forEach(function (m) {
                        var tr = document.createElement('tr');

                        var tdMetric = document.createElement('td');
                        tdMetric.className = 'fibi-compare-metric';
                        tdMetric.textContent = m.name;

                        var tdA = document.createElement('td');
                        tdA.className = 'fibi-compare-val';
                        tdA.textContent = m.valA;

                        var tdB = document.createElement('td');
                        tdB.className = 'fibi-compare-val';
                        tdB.textContent = m.valB;

                        tr.appendChild(tdMetric);
                        tr.appendChild(tdA);
                        tr.appendChild(tdB);

                        compareBody.appendChild(tr);
                    });
                }
            } else {
                if (compareBlockEl) {
                    compareBlockEl.hidden = true;
                    compareBlockEl.setAttribute('hidden', '');
                    compareBlockEl.style.display = 'none';
                }
                if (compareBody) {
                    while (compareBody.firstChild) {
                        compareBody.removeChild(compareBody.firstChild);
                    }
                }
            }

            // Yasal uyarı metni
            if (disclaimerTextEl) {
                disclaimerTextEl.textContent = data.disclaimer || '';
            }

            // İlanlar bağlantısı
            if (listingsLink) {
                listingsLink.href = F.buildListingsUrl(primary.location);
            }

            // Lead formu bağlamını güncelle
            var targetNeighborhoodId = (pLoc && pLoc.neighborhoodId != null)
                ? pLoc.neighborhoodId
                : body.neighborhoodId;

            F.setLeadContext(targetNeighborhoodId, {
                module: 'NeighborhoodAnalysis',
                input: body,
                result: {
                    investmentGrade: primary.investmentGrade,
                    priceBand: primary.priceBand,
                    amortizationYears: primary.amortizationYears,
                    expected12MonthGrowthRate: primary.expected12MonthGrowthRate,
                    comparisonNeighborhood: (comp && comp.location) ? comp.location.neighborhoodName : null,
                    dataScope: primary.dataSource ? primary.dataSource.scopeCode : null
                }
            });

            // Küçük ekranlarda (992px altı) sonuç paneline yumuşak kaydır
            if (window.innerWidth < 992 && resultPanel) {
                resultPanel.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        }

        // 5. Lead Danışmanlık buton tıklaması
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
