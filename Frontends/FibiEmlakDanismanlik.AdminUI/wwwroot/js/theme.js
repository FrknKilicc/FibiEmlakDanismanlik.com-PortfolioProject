/*!
 * Fibi Admin Tema Yöneticisi (Açık / Koyu / Sistem)
 */
(function () {
  'use strict';

  var STORAGE_KEY = 'fibi-theme';

  function getStoredTheme() {
    try {
      return localStorage.getItem(STORAGE_KEY) || 'auto';
    } catch (e) {
      return 'auto';
    }
  }

  function setStoredTheme(theme) {
    try {
      localStorage.setItem(STORAGE_KEY, theme);
    } catch (e) {
      // localStorage erişimi kısıtlı olabilir
    }
  }

  function getSystemTheme() {
    return (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches)
      ? 'dark'
      : 'light';
  }

  function resolveTheme(pref) {
    if (pref === 'auto') {
      return getSystemTheme();
    }
    return pref === 'dark' ? 'dark' : 'light';
  }

  function updateToggleIcon(pref) {
    var iconClass = 'bi-circle-half';
    if (pref === 'light') {
      iconClass = 'bi-sun';
    } else if (pref === 'dark') {
      iconClass = 'bi-moon-stars';
    }

    var toggleButtons = document.querySelectorAll('#themeToggle, [data-theme-toggle]');
    toggleButtons.forEach(function (btn) {
      var icon = btn.querySelector('i');
      if (icon) {
        icon.classList.remove('bi-sun', 'bi-moon-stars', 'bi-circle-half');
        icon.classList.add(iconClass);
      }
    });
  }

  function updateCheckmarks(pref) {
    var items = document.querySelectorAll('[data-theme-value]');
    items.forEach(function (btn) {
      var val = btn.getAttribute('data-theme-value');
      var isSelected = (val === pref);
      btn.classList.toggle('active', isSelected);
      btn.setAttribute('aria-pressed', isSelected ? 'true' : 'false');

      var check = btn.querySelector('.theme-check');
      if (check) {
        if (isSelected) {
          check.classList.remove('d-none');
        } else {
          check.classList.add('d-none');
        }
      }
    });
  }

  function applyTheme(pref, persist) {
    if (persist) {
      setStoredTheme(pref);
    }

    var resolved = resolveTheme(pref);
    document.documentElement.setAttribute('data-bs-theme', resolved);
    document.documentElement.setAttribute('data-theme-pref', pref);

    updateToggleIcon(pref);
    updateCheckmarks(pref);

    try {
      document.dispatchEvent(new CustomEvent('fibi:themechange', {
        detail: { theme: resolved }
      }));
    } catch (e) {
      var evt = document.createEvent('CustomEvent');
      evt.initCustomEvent('fibi:themechange', false, false, { theme: resolved });
      document.dispatchEvent(evt);
    }
  }

  // Tıklama olayları (Event Delegation)
  document.addEventListener('click', function (e) {
    var btn = e.target.closest('[data-theme-value]');
    if (!btn) {
      return;
    }
    e.preventDefault();
    var pref = btn.getAttribute('data-theme-value');
    if (pref) {
      applyTheme(pref, true);
    }
  });

  // Sistem tercihi değiştiğinde (prefers-color-scheme)
  if (window.matchMedia) {
    var mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    var onSystemThemeChange = function () {
      var pref = getStoredTheme();
      if (pref === 'auto') {
        applyTheme('auto', false);
      }
    };

    if (mediaQuery.addEventListener) {
      mediaQuery.addEventListener('change', onSystemThemeChange);
    } else if (mediaQuery.addListener) {
      mediaQuery.addListener(onSystemThemeChange);
    }
  }

  // İlk yüklemede UI'ı güncelle
  function init() {
    var pref = getStoredTheme();
    var resolved = resolveTheme(pref);
    document.documentElement.setAttribute('data-bs-theme', resolved);
    document.documentElement.setAttribute('data-theme-pref', pref);
    updateToggleIcon(pref);
    updateCheckmarks(pref);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();
