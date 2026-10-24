(function () {
    'use strict';

    var pendingForm = null;

    document.addEventListener('submit', function (e) {
        var form = e.target;
        if (!form || !form.matches || !form.matches('form[data-confirm]')) {
            return;
        }

        if (form.__confirmed) {
            return;
        }

        var message = form.getAttribute('data-confirm') || 'Bu kaydı silmek istediğinize emin misiniz?';
        var modalEl = document.getElementById('confirmModal');
        var textEl = document.getElementById('confirmModalText');
        var okBtn = document.getElementById('confirmModalOk');

        if (modalEl && textEl && okBtn && window.bootstrap && window.bootstrap.Modal) {
            e.preventDefault();
            pendingForm = form;
            textEl.textContent = message;
            var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
        } else {
            if (!window.confirm(message)) {
                e.preventDefault();
            }
        }
    });

    document.addEventListener('DOMContentLoaded', function () {
        var okBtn = document.getElementById('confirmModalOk');
        var modalEl = document.getElementById('confirmModal');

        if (okBtn) {
            okBtn.addEventListener('click', function () {
                if (pendingForm) {
                    pendingForm.__confirmed = true;
                    if (modalEl && window.bootstrap && window.bootstrap.Modal) {
                        var modal = bootstrap.Modal.getInstance(modalEl);
                        if (modal) {
                            modal.hide();
                        }
                    }
                    pendingForm.submit();
                    pendingForm = null;
                }
            });
        }
    });
})();
