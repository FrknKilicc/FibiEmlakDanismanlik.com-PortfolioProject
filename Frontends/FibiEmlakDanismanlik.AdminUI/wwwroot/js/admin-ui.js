(function () {
  'use strict';

  /* ==============================================================
     # Global Yardımcı Fonksiyonlar
     ============================================================== */

  function findFieldLabel(el) {
    if (!el) return null;
    if (el.id) {
      var labelFor = document.querySelector('label[for="' + CSS.escape(el.id) + '"]');
      if (labelFor) return labelFor;
    }
    var wrappingLabel = el.closest('label');
    if (wrappingLabel) return wrappingLabel;

    var parent = el.closest('.form-group, .mb-3, .form-floating, [class*="col-"], .col');
    if (parent) {
      var labelInParent = parent.querySelector('label');
      if (labelInParent) return labelInParent;
    }
    return null;
  }

  function getCleanLabelText(field) {
    var label = findFieldLabel(field);
    var text = '';
    if (label) {
      var clone = label.cloneNode(true);
      clone.querySelectorAll('.req-star, .help-tip, .text-danger, button, script, style, svg, i').forEach(function (n) {
        n.remove();
      });
      text = (clone.textContent || '');
      // Yıldız (*), "Açıklama" ve gereksiz boşlukları temizle
      text = text.replace(/[*]/g, '')
                 .replace(/\bAçıklama\b/gi, '')
                 .replace(/\s+/g, ' ')
                 .trim();
    }
    if (!text) {
      text = field.getAttribute('data-label') || field.getAttribute('placeholder') || field.getAttribute('name') || field.id || 'Zorunlu alan';
    }
    return text;
  }

  function revealAndFocus(element) {
    if (!element) return;

    // 1. Gizli sihirbaz adımındaysa olayı tetikle ve görünür yap
    var listingSec = element.closest('.listing-section');
    if (listingSec && listingSec.classList.contains('d-none')) {
      document.dispatchEvent(new CustomEvent('fibi:reveal', { detail: { element: element } }));
      listingSec.classList.remove('d-none');
    }

    // 2. Kapalı sekme (tab) içindeyse aç
    var tabPane = element.closest('.tab-pane');
    if (tabPane && !tabPane.classList.contains('active') && !tabPane.classList.contains('show')) {
      var tabId = tabPane.id;
      if (tabId) {
        var tabTrigger = document.querySelector('[data-bs-target="#' + CSS.escape(tabId) + '"], [href="#' + CSS.escape(tabId) + '"]');
        if (tabTrigger && window.bootstrap && window.bootstrap.Tab) {
          bootstrap.Tab.getOrCreateInstance(tabTrigger).show();
        }
      }
    }

    // 3. Kapalı akordiyon içindeyse aç
    var accordionCollapse = element.closest('.accordion-collapse');
    if (accordionCollapse && !accordionCollapse.classList.contains('show')) {
      if (window.bootstrap && window.bootstrap.Collapse) {
        bootstrap.Collapse.getOrCreateInstance(accordionCollapse).show();
      }
    }

    // 4. Kapalı modal içindeyse aç
    var modalEl = element.closest('.modal');
    if (modalEl && !modalEl.classList.contains('show')) {
      if (window.bootstrap && window.bootstrap.Modal) {
        bootstrap.Modal.getOrCreateInstance(modalEl).show();
      }
    }

    // 5. Yumuşak kaydır ve odakla
    var prefersReducedMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    setTimeout(function () {
      element.scrollIntoView({
        behavior: prefersReducedMotion ? 'auto' : 'smooth',
        block: 'center'
      });
      try {
        element.focus({ preventScroll: true });
      } catch (_) {
        element.focus();
      }
    }, 120);
  }

  /* ==============================================================
     # 1) Bildirimler (Toast Sistemi)
     ============================================================== */

  var lastToastPayload = '';
  var lastToastTime = 0;

  window.fibiToast = function (message, type, options) {
    type = type || 'success';
    options = options || {};

    // 1 saniye içinde aynı mesajın tekrarını engelle
    var now = Date.now();
    var dedupeKey = type + '::' + (message || '') + '::' + (options.title || '');
    if (dedupeKey === lastToastPayload && (now - lastToastTime) < 1000) {
      return;
    }
    lastToastPayload = dedupeKey;
    lastToastTime = now;

    var stack = document.getElementById('toastStack');
    if (!stack) {
      stack = document.createElement('div');
      stack.id = 'toastStack';
      stack.className = 'toast-container position-fixed top-0 end-0 p-3 toast-stack-fixed';
      stack.setAttribute('aria-live', 'polite');
      stack.setAttribute('aria-atomic', 'false');
      document.body.appendChild(stack);
    }

    var typeConfig = {
      success: {
        icon: 'bi-check-circle-fill',
        title: 'Başarılı',
        delay: 4000,
        autohide: true
      },
      error: {
        icon: 'bi-x-octagon-fill',
        title: 'İşlem başarısız',
        delay: 0,
        autohide: false
      },
      warning: {
        icon: 'bi-exclamation-triangle-fill',
        title: 'Dikkat',
        delay: 7000,
        autohide: true
      },
      info: {
        icon: 'bi-info-circle-fill',
        title: 'Bilgi',
        delay: 5000,
        autohide: true
      }
    };

    var cfg = typeConfig[type] || typeConfig.info;
    var title = options.title || cfg.title;
    var hasCustomDelay = typeof options.delay === 'number';
    var delay = hasCustomDelay ? options.delay : cfg.delay;
    var autohide = cfg.autohide;
    if (hasCustomDelay) {
      autohide = delay > 0;
    }

    // Toast ana konteyneri
    var toastEl = document.createElement('div');
    toastEl.className = 'toast fibi-toast fibi-toast-' + type;
    toastEl.setAttribute('role', type === 'error' ? 'alert' : 'status');
    toastEl.setAttribute('aria-live', type === 'error' ? 'assertive' : 'polite');
    toastEl.setAttribute('aria-atomic', 'true');

    // Başlık alanı
    var headerEl = document.createElement('div');
    headerEl.className = 'toast-header fibi-toast-header';

    var iconEl = document.createElement('i');
    iconEl.className = 'bi ' + cfg.icon + ' me-2 fibi-toast-icon';
    iconEl.setAttribute('aria-hidden', 'true');
    headerEl.appendChild(iconEl);

    var titleEl = document.createElement('strong');
    titleEl.className = 'me-auto fibi-toast-title';
    titleEl.textContent = title;
    headerEl.appendChild(titleEl);

    var closeBtn = document.createElement('button');
    closeBtn.type = 'button';
    closeBtn.className = 'btn-close';
    closeBtn.setAttribute('data-bs-dismiss', 'toast');
    closeBtn.setAttribute('aria-label', 'Kapat');
    headerEl.appendChild(closeBtn);

    toastEl.appendChild(headerEl);

    // Gövde alanı
    var bodyEl = document.createElement('div');
    bodyEl.className = 'toast-body fibi-toast-body';

    if (message) {
      var msgEl = document.createElement('div');
      msgEl.className = 'fibi-toast-message';
      msgEl.textContent = message;
      bodyEl.appendChild(msgEl);
    }

    if (Array.isArray(options.items) && options.items.length > 0) {
      var listEl = document.createElement('ul');
      listEl.className = 'fibi-toast-list mb-0 mt-2 ps-3';
      options.items.forEach(function (itemText, idx) {
        var li = document.createElement('li');
        if (typeof options.onClickItem === 'function') {
          var itemBtn = document.createElement('button');
          itemBtn.type = 'button';
          itemBtn.className = 'btn btn-link p-0 text-start fibi-toast-item-btn';
          itemBtn.textContent = itemText;
          itemBtn.addEventListener('click', function () {
            options.onClickItem(idx);
          });
          li.appendChild(itemBtn);
        } else {
          li.textContent = itemText;
        }
        listEl.appendChild(li);
      });
      bodyEl.appendChild(listEl);
    }

    toastEl.appendChild(bodyEl);

    // Otomatik kapananlarda ilerleme çizgisi
    if (autohide && delay > 0) {
      var progressEl = document.createElement('div');
      progressEl.className = 'fibi-toast-progress';
      progressEl.style.animationDuration = delay + 'ms';
      toastEl.appendChild(progressEl);
    }

    stack.appendChild(toastEl);

    if (window.bootstrap && window.bootstrap.Toast) {
      var bsToast = new bootstrap.Toast(toastEl, {
        autohide: autohide,
        delay: autohide ? delay : undefined
      });

      toastEl.addEventListener('hidden.bs.toast', function () {
        if (toastEl.parentNode) {
          toastEl.parentNode.removeChild(toastEl);
        }
      });

      bsToast.show();
    }
  };

  /* ==============================================================
     # 3) Alan Açıklamaları (Tooltip Başlatma Fonksiyonu)
     ============================================================== */

  window.fibiInitHelp = function (root) {
    root = root || document;

    var helpElements = root.querySelectorAll('[data-help]');
    helpElements.forEach(function (el) {
      var helpText = el.getAttribute('data-help');
      if (!helpText) return;

      var label = findFieldLabel(el);
      var container = label || el.parentNode;
      if (!container) return;

      if (container.querySelector('.help-tip')) {
        return;
      }

      var tipBtn = document.createElement('button');
      tipBtn.type = 'button';
      tipBtn.className = 'help-tip';
      tipBtn.setAttribute('data-bs-toggle', 'tooltip');
      tipBtn.setAttribute('data-bs-placement', 'top');
      tipBtn.setAttribute('aria-label', 'Açıklama');
      tipBtn.setAttribute('title', helpText);

      var icon = document.createElement('i');
      icon.className = 'bi bi-info-circle';
      icon.setAttribute('aria-hidden', 'true');
      tipBtn.appendChild(icon);

      if (label) {
        label.appendChild(tipBtn);
      } else if (el.nextSibling) {
        el.parentNode.insertBefore(tipBtn, el.nextSibling);
      } else {
        el.parentNode.appendChild(tipBtn);
      }
    });

    if (window.bootstrap && window.bootstrap.Tooltip) {
      var tooltips = root.querySelectorAll('[data-bs-toggle="tooltip"]');
      tooltips.forEach(function (tipEl) {
        bootstrap.Tooltip.getOrCreateInstance(tipEl, {
          trigger: 'hover focus click'
        });
      });

      // Yalnızca ikon içeren butonlarda tooltip başlat
      var iconOnlyBtns = root.querySelectorAll('.btn[title]');
      iconOnlyBtns.forEach(function (btn) {
        var clone = btn.cloneNode(true);
        clone.querySelectorAll('i, svg, span.spinner-border').forEach(function (n) { n.remove(); });
        var text = (clone.textContent || '').trim();
        if (text === '' && !btn.hasAttribute('data-bs-toggle')) {
          btn.setAttribute('data-bs-toggle', 'tooltip');
          bootstrap.Tooltip.getOrCreateInstance(btn, {
            trigger: 'hover focus click'
          });
        }
      });
    }
  };

  /* ==============================================================
     # Global Olay Dinleyicileri (Submit, Input, Bfcache, Tooltip Dışı Tıklama)
     ============================================================== */

  // Form Doğrulama (Capture Aşamasında - Diğer tüm dinleyicilerden önce yakalar)
  document.addEventListener('submit', function (e) {
    var form = e.target;
    if (!form || !form.checkValidity) return;
    if (!form.closest('main') && !form.closest('.modal')) return;

    if (!form.checkValidity()) {
      e.preventDefault();
      e.stopImmediatePropagation();

      form.classList.add('was-validated');

      var invalids = Array.from(form.querySelectorAll(':invalid')).filter(function (el) {
        return el.tagName === 'INPUT' || el.tagName === 'SELECT' || el.tagName === 'TEXTAREA';
      });

      if (invalids.length > 0) {
        var labels = invalids.map(function (el) {
          var lbl = findFieldLabel(el);
          if (lbl) {
            lbl.classList.add('field-invalid-label');
          }
          return getCleanLabelText(el);
        });

        window.fibiToast('Lütfen zorunlu alanları doldurun.', 'warning', {
          title: 'Eksik bilgi',
          items: labels,
          onClickItem: function (idx) {
            if (invalids[idx]) {
              revealAndFocus(invalids[idx]);
            }
          }
        });

        revealAndFocus(invalids[0]);
      }
    }
  }, true);

  // Form Gönderilirken Çift Tıklamayı Engelleme ve Spinner (Normal Aşama - Yalnızca geçerli gönderimde)
  document.addEventListener('submit', function (e) {
    var form = e.target;
    if (!form || form.tagName !== 'FORM') return;
    if (!form.closest('main') && !form.closest('.modal')) return;
    var method = (form.getAttribute('method') || '').toLowerCase();
    if (method !== 'post') return;

    // data-confirm formu henüz onaylanmadıysa confirm-modal bekleyecek
    if (form.hasAttribute('data-confirm') && !form.__confirmed) {
      return;
    }

    if (typeof form.checkValidity === 'function' && !form.checkValidity()) {
      return;
    }

    var submitBtns = form.querySelectorAll('button[type="submit"]');
    submitBtns.forEach(function (btn) {
      if (btn.disabled) return;
      btn.disabled = true;
      btn.dataset.fibiOrigHtml = btn.innerHTML;
      var spinner = document.createElement('span');
      spinner.className = 'spinner-border spinner-border-sm me-2';
      spinner.setAttribute('role', 'status');
      spinner.setAttribute('aria-hidden', 'true');
      btn.insertBefore(spinner, btn.firstChild);
    });
  }, false);

  // Geri tuşu / bfcache durumunda submit butonlarını eski haline getir
  window.addEventListener('pageshow', function () {
    var disabledBtns = document.querySelectorAll('button[data-fibi-orig-html]');
    disabledBtns.forEach(function (btn) {
      btn.disabled = false;
      btn.innerHTML = btn.dataset.fibiOrigHtml;
      delete btn.dataset.fibiOrigHtml;
    });
  });

  // Geçersiz alan düzeltilince vurguyu kaldırma
  function handleFieldChange(e) {
    var field = e.target;
    if (!field || !field.tagName) return;
    var tag = field.tagName.toUpperCase();
    if (tag !== 'INPUT' && tag !== 'SELECT' && tag !== 'TEXTAREA') return;

    if (field.classList.contains('is-invalid')) {
      field.classList.remove('is-invalid');
    }
    var label = findFieldLabel(field);
    if (label && label.classList.contains('field-invalid-label')) {
      if (typeof field.checkValidity !== 'function' || field.checkValidity()) {
        label.classList.remove('field-invalid-label');
      }
    }
  }
  document.addEventListener('input', handleFieldChange, true);
  document.addEventListener('change', handleFieldChange, true);

  // Mobilde dışarı dokununca açık tooltip'leri kapatma
  document.addEventListener('click', function (e) {
    if (!e.target.closest('[data-bs-toggle="tooltip"], .tooltip')) {
      var openTooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
      openTooltips.forEach(function (el) {
        if (window.bootstrap && window.bootstrap.Tooltip) {
          var inst = bootstrap.Tooltip.getInstance(el);
          if (inst) {
            inst.hide();
          }
        }
      });
    }
  });

  /* ==============================================================
     # DOMContentLoaded Olayı
     ============================================================== */

  document.addEventListener('DOMContentLoaded', function () {
    // 0) Formların yerleşik tarayıcı balonlarını devre dışı bırakma (özel doğrulama akışı için)
    var formsWithConstraints = document.querySelectorAll('main form, .modal form');
    formsWithConstraints.forEach(function (form) {
      if (form.querySelector('[required], [pattern], [min], [max], [maxlength]')) {
        form.noValidate = true;
      }
    });

    // a) Modal açma (mevcut)
    var openModalEls = document.querySelectorAll('[data-open-modal]');
    openModalEls.forEach(function (el) {
      var modalId = el.getAttribute('data-open-modal');
      if (!modalId) {
        return;
      }
      var modalEl = document.getElementById(modalId);
      if (modalEl && window.bootstrap && window.bootstrap.Modal) {
        try {
          bootstrap.Modal.getOrCreateInstance(modalEl).show();
        } catch (err) {
          // sessiz geç
        }
      }
    });

    // b) Arama süzgeci (mevcut)
    var filterInputs = document.querySelectorAll('input[data-filter-input]');
    filterInputs.forEach(function (input) {
      var selector = input.getAttribute('data-filter-input');
      if (!selector) {
        return;
      }
      var container = document.querySelector(selector);
      if (!container) {
        return;
      }

      function applyFilter() {
        var query = (input.value || '').trim().toLocaleLowerCase('tr');
        var items = container.querySelectorAll('[data-filter-item]');
        var groups = container.querySelectorAll('[data-filter-group]');
        var emptyEl = container.querySelector('[data-filter-empty]') || document.querySelector('[data-filter-empty]');
        var totalVisible = 0;

        items.forEach(function (item) {
          var rawText = item.getAttribute('data-filter-text');
          var text = (rawText !== null ? rawText : (item.textContent || '')).toLocaleLowerCase('tr');
          var match = !query || text.indexOf(query) !== -1;
          if (match) {
            item.classList.remove('d-none');
            totalVisible++;
          } else {
            item.classList.add('d-none');
          }
        });

        groups.forEach(function (group) {
          var groupItems = group.querySelectorAll('[data-filter-item]');
          if (groupItems.length > 0) {
            var hasVisible = false;
            groupItems.forEach(function (gi) {
              if (!gi.classList.contains('d-none')) {
                hasVisible = true;
              }
            });
            if (hasVisible) {
              group.classList.remove('d-none');
            } else {
              group.classList.add('d-none');
            }
          }
        });

        if (emptyEl) {
          if (totalVisible === 0 && query.length > 0) {
            emptyEl.classList.remove('d-none');
          } else {
            emptyEl.classList.add('d-none');
          }
        }
      }

      input.addEventListener('input', applyFilter);
      if (input.value) {
        applyFilter();
      }
    });

    // c) Chip görünür alana kaydırma (mevcut)
    var activeChip = document.querySelector('.content-nav .chip.active');
    if (activeChip && typeof activeChip.scrollIntoView === 'function') {
      activeChip.scrollIntoView({ inline: 'center', block: 'nearest' });
    }

    // d) Görsel önizleme (mevcut)
    var previewInputs = document.querySelectorAll('input[data-preview-img]');
    previewInputs.forEach(function (input) {
      var imgId = input.getAttribute('data-preview-img');
      var placeholderId = input.getAttribute('data-preview-placeholder');
      var imgEl = imgId ? document.getElementById(imgId) : null;
      var placeholderEl = placeholderId ? document.getElementById(placeholderId) : null;

      function updatePreview() {
        var val = (input.value || '').trim();
        if (val.length > 0) {
          if (imgEl) {
            imgEl.src = val;
            imgEl.classList.remove('d-none');
          }
          if (placeholderEl) {
            placeholderEl.classList.add('d-none');
          }
        } else {
          if (imgEl) {
            imgEl.src = '';
            imgEl.classList.add('d-none');
          }
          if (placeholderEl) {
            placeholderEl.classList.remove('d-none');
          }
        }
      }

      input.addEventListener('input', updatePreview);
    });

    // e) Karakter sayacı (mevcut)
    var countInputs = document.querySelectorAll('[data-count-target]');
    countInputs.forEach(function (el) {
      var targetId = el.getAttribute('data-count-target');
      var targetEl = targetId ? document.getElementById(targetId) : null;
      if (!targetEl) {
        return;
      }

      function updateCount() {
        var len = (el.value || '').length;
        targetEl.textContent = len;
      }

      el.addEventListener('input', updateCount);
      updateCount();
    });

    // f) Sunucu mesajlarını bildirime (toast) çevirme
    function getAlertText(alertEl) {
      var clone = alertEl.cloneNode(true);
      clone.querySelectorAll('.btn-close, .btn, script, style').forEach(function (n) { n.remove(); });
      return (clone.textContent || '').trim();
    }

    var successAlerts = document.querySelectorAll('main .alert.alert-success.alert-dismissible');
    successAlerts.forEach(function (alertEl) {
      var text = getAlertText(alertEl);
      if (text) {
        window.fibiToast(text, 'success');
      }
      alertEl.remove();
    });

    var dangerAlerts = document.querySelectorAll('main .alert.alert-danger.alert-dismissible');
    dangerAlerts.forEach(function (alertEl) {
      var text = getAlertText(alertEl);
      if (text) {
        window.fibiToast(text, 'error');
      }
    });

    var warningAlerts = document.querySelectorAll('main .alert.alert-warning.alert-dismissible');
    warningAlerts.forEach(function (alertEl) {
      var text = getAlertText(alertEl);
      if (text) {
        window.fibiToast(text, 'warning');
      }
    });

    // g) Onay modalı "Evet, sil" butonuna spinner bağlama
    var confirmModalOk = document.getElementById('confirmModalOk');
    if (confirmModalOk) {
      confirmModalOk.addEventListener('click', function () {
        confirmModalOk.disabled = true;
        confirmModalOk.dataset.fibiOrigHtml = confirmModalOk.innerHTML;
        var spinner = document.createElement('span');
        spinner.className = 'spinner-border spinner-border-sm me-2';
        spinner.setAttribute('role', 'status');
        spinner.setAttribute('aria-hidden', 'true');
        confirmModalOk.insertBefore(spinner, confirmModalOk.firstChild);
      });
    }

    // h) Zorunlu alanların net gösterimi (req-star & aria-required)
    var requiredFields = document.querySelectorAll('main input[required], main select[required], main textarea[required], .modal input[required], .modal select[required], .modal textarea[required]');
    requiredFields.forEach(function (field) {
      field.setAttribute('aria-required', 'true');
      var label = findFieldLabel(field);
      if (label) {
        label.classList.add('required-label');
        var hasStar = label.querySelector('.req-star, .text-danger') || label.textContent.indexOf('*') !== -1;
        if (!hasStar) {
          var star = document.createElement('span');
          star.className = 'req-star text-danger';
          star.setAttribute('aria-hidden', 'true');
          star.textContent = ' *';
          label.appendChild(star);
        }
      }
    });

    // i) Sunucudan doğrulama hatasıyla dönülmüşse bildir ve odakla
    var serverInvalids = Array.from(document.querySelectorAll('main .is-invalid, main .invalid-feedback.d-block, .modal .is-invalid, .modal .invalid-feedback.d-block'));
    if (serverInvalids.length > 0) {
      var invalidControls = [];
      serverInvalids.forEach(function (el) {
        if (el.classList.contains('is-invalid')) {
          if (invalidControls.indexOf(el) === -1) invalidControls.push(el);
        } else if (el.classList.contains('invalid-feedback')) {
          var prev = el.previousElementSibling;
          if (prev && (prev.tagName === 'INPUT' || prev.tagName === 'SELECT' || prev.tagName === 'TEXTAREA')) {
            if (invalidControls.indexOf(prev) === -1) invalidControls.push(prev);
          }
        }
      });

      if (invalidControls.length > 0) {
        var errLabels = invalidControls.map(function (ctrl) {
          var lbl = findFieldLabel(ctrl);
          if (lbl) {
            lbl.classList.add('field-invalid-label');
          }
          return getCleanLabelText(ctrl);
        });

        window.fibiToast('Formda düzeltilmesi gereken ' + invalidControls.length + ' alan var.', 'error', {
          title: 'Hatalı alanlar',
          items: errLabels,
          onClickItem: function (idx) {
            if (invalidControls[idx]) {
              revealAndFocus(invalidControls[idx]);
            }
          }
        });

        revealAndFocus(invalidControls[0]);
      }
    }

    // j) Alan açıklamaları ve tooltip'leri başlat
    window.fibiInitHelp(document);
  });
})();
