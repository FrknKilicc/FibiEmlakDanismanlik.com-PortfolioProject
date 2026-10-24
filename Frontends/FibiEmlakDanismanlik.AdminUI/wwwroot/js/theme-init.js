(function () {
  try {
    var pref = localStorage.getItem('fibi-theme') || 'auto';
    var theme = pref;
    if (theme === 'auto') {
      theme = (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) ? 'dark' : 'light';
    }
    document.documentElement.setAttribute('data-bs-theme', theme);
    document.documentElement.setAttribute('data-theme-pref', pref);
  } catch (e) {
    document.documentElement.setAttribute('data-bs-theme', 'light');
    document.documentElement.setAttribute('data-theme-pref', 'auto');
  }
})();
