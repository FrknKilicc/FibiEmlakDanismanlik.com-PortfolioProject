(function () {
  'use strict';

  var dataEl = document.getElementById('dashboard-data');
  if (!dataEl) {
    return;
  }

  var data = null;
  try {
    data = JSON.parse(dataEl.textContent || '{}');
  } catch (e) {
    data = null;
  }

  if (!data) {
    return;
  }

  // --- 1. SAYAÇ ANİMASYONLARI (requestAnimationFrame, easeOutCubic) ---
  var prefersReducedMotion = false;
  if (window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    prefersReducedMotion = true;
  }

  function easeOutCubic(t) {
    return 1 - Math.pow(1 - t, 3);
  }

  var counterElements = document.querySelectorAll('.js-counter');
  var duration = 900;

  counterElements.forEach(function (el) {
    var rawTarget = el.getAttribute('data-count');
    var isMoney = el.getAttribute('data-money') === '1';
    var target = parseFloat(rawTarget);

    if (isNaN(target)) {
      return;
    }

    if (prefersReducedMotion) {
      if (isMoney) {
        el.textContent = Math.round(target).toLocaleString('tr-TR') + ' ₺';
      } else {
        el.textContent = Math.round(target).toLocaleString('tr-TR');
      }
      return;
    }

    var startTime = null;

    function step(timestamp) {
      if (!startTime) {
        startTime = timestamp;
      }
      var elapsed = timestamp - startTime;
      var progress = Math.min(elapsed / duration, 1);
      var current = target * easeOutCubic(progress);

      if (isMoney) {
        el.textContent = Math.round(current).toLocaleString('tr-TR') + ' ₺';
      } else {
        el.textContent = Math.round(current).toLocaleString('tr-TR');
      }

      if (progress < 1) {
        window.requestAnimationFrame(step);
      } else {
        if (isMoney) {
          el.textContent = Math.round(target).toLocaleString('tr-TR') + ' ₺';
        } else {
          el.textContent = Math.round(target).toLocaleString('tr-TR');
        }
      }
    }

    window.requestAnimationFrame(step);
  });

  // --- 2. GRAFİKLER (Chart.js) ---
  if (!window.Chart) {
    return;
  }

  // Ortak ayarlar
  try {
    Chart.defaults.font.family = window.getComputedStyle(document.body).fontFamily || "'Nunito', sans-serif";
  } catch (err) {
    // default font kalır
  }
  Chart.defaults.maintainAspectRatio = false;

  function getThemeColors() {
    var rootStyle = window.getComputedStyle(document.documentElement);
    return {
      muted: rootStyle.getPropertyValue('--fibi-muted').trim() || '#6b7280',
      border: rootStyle.getPropertyValue('--fibi-border').trim() || '#e6e9f0',
      surface: rootStyle.getPropertyValue('--fibi-surface').trim() || '#ffffff'
    };
  }

  function applyChartDefaults() {
    var colors = getThemeColors();
    Chart.defaults.color = colors.muted;
    Chart.defaults.borderColor = colors.border;
  }

  applyChartDefaults();

  var createdCharts = [];

  var palette = ['#4154f1', '#2eca6a', '#ff771d', '#0dcaf0', '#6f42c1', '#adb5bd'];
  var monthNames = ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Ağu', 'Eyl', 'Eki', 'Kas', 'Ara'];

  function showEmptyMessage(canvas) {
    if (canvas && canvas.parentElement) {
      canvas.parentElement.innerHTML =
        '<div class="d-flex align-items-center justify-content-center h-100">' +
        '<p class="text-muted small mb-0">Bu aralıkta veri yok.</p>' +
        '</div>';
    }
  }

  // A. Aylık İşlem ve Komisyon Grafiği (Son 12 ay)
  var monthlyCanvas = document.getElementById('chartMonthly');
  if (monthlyCanvas) {
    var monthlyData = (data.finance && Array.isArray(data.finance.monthly)) ? data.finance.monthly : [];
    var hasMonthlyData = monthlyData.length > 0 && monthlyData.some(function (m) {
      return (m.commission > 0) || (m.completedCount > 0);
    });

    if (!hasMonthlyData) {
      showEmptyMessage(monthlyCanvas);
    } else {
      var monthlyLabels = [];
      var commissions = [];
      var completedCounts = [];

      monthlyData.forEach(function (m) {
        var mIdx = (m.month >= 1 && m.month <= 12) ? m.month - 1 : 0;
        monthlyLabels.push(monthNames[mIdx] + ' ' + m.year);
        commissions.push(m.commission || 0);
        completedCounts.push(m.completedCount || 0);
      });

      var chartMonthly = new Chart(monthlyCanvas, {
        type: 'line',
        data: {
          labels: monthlyLabels,
          datasets: [
            {
              label: 'Komisyon',
              data: commissions,
              borderColor: '#4154f1',
              backgroundColor: 'rgba(65, 84, 241, 0.1)',
              fill: true,
              tension: 0.3,
              yAxisID: 'y'
            },
            {
              label: 'Tamamlanan İşlem',
              data: completedCounts,
              type: 'bar',
              backgroundColor: 'rgba(46, 202, 106, 0.7)',
              borderColor: '#2eca6a',
              borderWidth: 1,
              borderRadius: 4,
              yAxisID: 'y1'
            }
          ]
        },
        options: {
          responsive: true,
          interaction: {
            mode: 'index',
            intersect: false
          },
          plugins: {
            legend: {
              position: 'top'
            },
            tooltip: {
              callbacks: {
                label: function (context) {
                  var raw = context.raw || 0;
                  if (context.datasetIndex === 0) {
                    return context.dataset.label + ': ' + Number(raw).toLocaleString('tr-TR') + ' ₺';
                  }
                  return context.dataset.label + ': ' + raw + ' adet';
                }
              }
            }
          },
          scales: {
            y: {
              type: 'linear',
              display: true,
              position: 'left',
              beginAtZero: true,
              ticks: {
                callback: function (val) {
                  return Number(val).toLocaleString('tr-TR') + ' ₺';
                }
              }
            },
            y1: {
              type: 'linear',
              display: true,
              position: 'right',
              beginAtZero: true,
              grid: {
                drawOnChartArea: false
              },
              ticks: {
                precision: 0,
                callback: function (val) {
                  return val + ' adet';
                }
              }
            }
          }
        }
      });
      createdCharts.push(chartMonthly);
    }
  }

  // B. İlan Dağılımı Grafiği (Halka)
  var listingsCanvas = document.getElementById('chartListings');
  if (listingsCanvas) {
    var categoryLabels = {
      housing: 'Konut',
      land: 'Arsa',
      commercial: 'İşyeri'
    };

    var ringLabels = [];
    var ringValues = [];

    if (data.listings && data.listings.byCategorySale) {
      Object.keys(data.listings.byCategorySale).forEach(function (k) {
        var count = data.listings.byCategorySale[k] || 0;
        if (count > 0) {
          var name = categoryLabels[k.toLowerCase()] || k;
          ringLabels.push('Satılık ' + name);
          ringValues.push(count);
        }
      });
    }

    if (data.listings && data.listings.byCategoryRent) {
      Object.keys(data.listings.byCategoryRent).forEach(function (k) {
        var count = data.listings.byCategoryRent[k] || 0;
        if (count > 0) {
          var name = categoryLabels[k.toLowerCase()] || k;
          ringLabels.push('Kiralık ' + name);
          ringValues.push(count);
        }
      });
    }

    var totalListingsCount = ringValues.reduce(function (acc, v) { return acc + v; }, 0);

    if (totalListingsCount === 0) {
      showEmptyMessage(listingsCanvas);
    } else {
      var chartListings = new Chart(listingsCanvas, {
        type: 'doughnut',
        data: {
          labels: ringLabels,
          datasets: [
            {
              data: ringValues,
              backgroundColor: palette.slice(0, ringLabels.length),
              borderWidth: 2,
              borderColor: getThemeColors().surface
            }
          ]
        },
        options: {
          responsive: true,
          plugins: {
            legend: {
              position: 'bottom',
              labels: {
                boxWidth: 12
              }
            },
            tooltip: {
              callbacks: {
                label: function (context) {
                  var raw = context.raw || 0;
                  var percentage = Math.round((raw / totalListingsCount) * 100);
                  return context.label + ': ' + raw + ' (%' + percentage + ')';
                }
              }
            }
          },
          cutout: '62%'
        }
      });
      createdCharts.push(chartListings);
    }
  }

  // C. Talep Hunisi Grafiği (Yatay Çubuk)
  var funnelCanvas = document.getElementById('chartFunnel');
  if (funnelCanvas) {
    var funnelOrder = [
      { key: 'New', name: 'Yeni', color: '#0d6efd' },
      { key: 'Contacted', name: 'Arandı', color: '#0dcaf0' },
      { key: 'Appointment', name: 'Randevu', color: '#ffc107' },
      { key: 'Offer', name: 'Teklif', color: '#6c757d' },
      { key: 'Deposit', name: 'Kapora', color: '#212529' },
      { key: 'Won', name: 'Kazanıldı', color: '#198754' },
      { key: 'Lost', name: 'Kaybedildi', color: '#dc3545' }
    ];

    var funnelLabels = [];
    var funnelValues = [];
    var funnelColors = [];

    var statsMap = (data.leadStats && data.leadStats.byStatusInRange) ? data.leadStats.byStatusInRange : {};
    var normalizedMap = {};
    Object.keys(statsMap).forEach(function (k) {
      normalizedMap[k.toLowerCase()] = statsMap[k];
    });

    funnelOrder.forEach(function (stepDef) {
      var val = normalizedMap[stepDef.key.toLowerCase()] || 0;
      funnelLabels.push(stepDef.name);
      funnelValues.push(val);
      funnelColors.push(stepDef.color);
    });

    var hasFunnelData = funnelValues.some(function (v) { return v > 0; });

    if (!hasFunnelData) {
      showEmptyMessage(funnelCanvas);
    } else {
      var chartFunnel = new Chart(funnelCanvas, {
        type: 'bar',
        data: {
          labels: funnelLabels,
          datasets: [
            {
              label: 'Talep',
              data: funnelValues,
              backgroundColor: funnelColors,
              borderRadius: 4
            }
          ]
        },
        options: {
          indexAxis: 'y',
          responsive: true,
          plugins: {
            legend: {
              display: false
            },
            tooltip: {
              callbacks: {
                label: function (context) {
                  return context.dataset.label + ': ' + context.raw + ' adet';
                }
              }
            }
          },
          scales: {
            x: {
              beginAtZero: true,
              ticks: {
                precision: 0
              }
            }
          }
        }
      });
      createdCharts.push(chartFunnel);
    }
  }

  // D. Danışman Performansı Grafiği (Çubuk)
  var agentsCanvas = document.getElementById('chartAgents');
  if (agentsCanvas) {
    var agents = Array.isArray(data.agents) ? data.agents : [];

    if (agents.length === 0) {
      showEmptyMessage(agentsCanvas);
    } else {
      var agentLabels = [];
      var wonData = [];
      var shareData = [];

      agents.forEach(function (a) {
        agentLabels.push(a.agentName || 'Danışman #' + a.agentId);
        wonData.push(a.wonLeads || 0);
        shareData.push(a.shareTotal || 0);
      });

      var datasets = [
        {
          label: 'Kazanılan Talep',
          data: wonData,
          backgroundColor: '#2eca6a',
          borderRadius: 4,
          yAxisID: 'y'
        }
      ];

      var hasFinance = Boolean(data.canSeeFinance);
      if (hasFinance) {
        datasets.push({
          label: 'Toplam Prim (₺)',
          data: shareData,
          backgroundColor: '#ff771d',
          borderRadius: 4,
          yAxisID: 'y1'
        });
      }

      var agentScales = {
        y: {
          beginAtZero: true,
          ticks: {
            precision: 0,
            callback: function (val) {
              return val + ' adet';
            }
          }
        }
      };

      if (hasFinance) {
        agentScales.y1 = {
          position: 'right',
          beginAtZero: true,
          grid: {
            drawOnChartArea: false
          },
          ticks: {
            callback: function (val) {
              return Number(val).toLocaleString('tr-TR') + ' ₺';
            }
          }
        };
      }

      var chartAgents = new Chart(agentsCanvas, {
        type: 'bar',
        data: {
          labels: agentLabels,
          datasets: datasets
        },
        options: {
          responsive: true,
          plugins: {
            legend: {
              position: 'top'
            },
            tooltip: {
              callbacks: {
                label: function (context) {
                  var raw = context.raw || 0;
                  if (context.datasetIndex === 1) {
                    return context.dataset.label + ': ' + Number(raw).toLocaleString('tr-TR') + ' ₺';
                  }
                  return context.dataset.label + ': ' + raw + ' adet';
                }
              }
            }
          },
          scales: agentScales
        }
      });
      createdCharts.push(chartAgents);
    }
  }

  // --- 3. TEMA DEĞİŞİKLİĞİ DİNLEYİCİSİ (fibi:themechange) ---
  document.addEventListener('fibi:themechange', function () {
    applyChartDefaults();
    var colors = getThemeColors();

    var instances = [];
    if (Chart.instances) {
      if (typeof Chart.instances.forEach === 'function') {
        Chart.instances.forEach(function (c) {
          instances.push(c);
        });
      } else {
        Object.keys(Chart.instances).forEach(function (k) {
          if (Chart.instances[k]) {
            instances.push(Chart.instances[k]);
          }
        });
      }
    }
    createdCharts.forEach(function (c) {
      if (c && instances.indexOf(c) === -1) {
        instances.push(c);
      }
    });

    instances.forEach(function (chart) {
      if (!chart) {
        return;
      }
      if (chart.config && chart.config.type === 'doughnut') {
        if (chart.data && chart.data.datasets) {
          chart.data.datasets.forEach(function (ds) {
            ds.borderColor = colors.surface;
          });
        }
      }
      if (chart.options && chart.options.scales) {
        Object.keys(chart.options.scales).forEach(function (scaleKey) {
          var scale = chart.options.scales[scaleKey];
          if (scale && scale.grid) {
            scale.grid.color = colors.border;
          }
        });
      }
      chart.update();
    });
  });
})();
