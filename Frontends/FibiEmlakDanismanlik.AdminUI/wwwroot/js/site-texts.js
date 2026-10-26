// Site metinleri: "varsayılana döndür" düğmeleri (CSP nedeniyle satır içi script yok)
document.addEventListener('click', function (e) {
    var btn = e.target.closest('[data-reset-default]');
    if (!btn) return;
    var field = document.getElementById(btn.getAttribute('data-reset-default'));
    if (!field) return;
    field.value = field.getAttribute('data-default') || '';
    field.dispatchEvent(new Event('input', { bubbles: true }));
    field.focus();
});
