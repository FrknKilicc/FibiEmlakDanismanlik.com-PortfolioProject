(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var toggleAllBtn = document.getElementById('btnToggleAll');
        var deleteForm = document.getElementById('orphanDeleteForm');
        var checkboxes = Array.from(document.querySelectorAll('input[type="checkbox"][name="paths"]'));
        var selectedCountEl = document.getElementById('selectedCountText');
        var deleteBtn = document.getElementById('btnDeleteSelected');

        function updateSelectionState() {
            var total = checkboxes.length;
            var checkedCount = 0;

            for (var i = 0; i < total; i++) {
                if (checkboxes[i].checked) {
                    checkedCount++;
                }
            }

            if (selectedCountEl) {
                selectedCountEl.textContent = checkedCount + ' / ' + total + ' görsel seçili';
            }

            if (deleteBtn) {
                deleteBtn.disabled = (checkedCount === 0);
            }

            if (toggleAllBtn) {
                if (checkedCount === total && total > 0) {
                    toggleAllBtn.innerHTML = '<i class="bi bi-dash-square me-1"></i> Tüm Seçimleri Kaldır';
                } else {
                    toggleAllBtn.innerHTML = '<i class="bi bi-check-square me-1"></i> Tümünü Seç';
                }
            }
        }

        if (toggleAllBtn) {
            toggleAllBtn.addEventListener('click', function () {
                var total = checkboxes.length;
                var allChecked = total > 0;
                for (var i = 0; i < total; i++) {
                    if (!checkboxes[i].checked) {
                        allChecked = false;
                        break;
                    }
                }

                var targetState = !allChecked;
                for (var j = 0; j < total; j++) {
                    checkboxes[j].checked = targetState;
                }

                updateSelectionState();
            });
        }

        checkboxes.forEach(function (cb) {
            cb.addEventListener('change', updateSelectionState);
        });

        if (deleteForm) {
            deleteForm.addEventListener('submit', function (e) {
                var anyChecked = false;
                for (var i = 0; i < checkboxes.length; i++) {
                    if (checkboxes[i].checked) {
                        anyChecked = true;
                        break;
                    }
                }

                if (!anyChecked) {
                    e.preventDefault();
                    e.stopImmediatePropagation();
                    if (window.fibiToast) {
                        window.fibiToast('Lütfen silinecek en az bir görsel seçin.', 'warning');
                    } else {
                        alert('Lütfen silinecek en az bir görsel seçin.');
                    }
                }
            }, true);
        }

        updateSelectionState();
    });
})();
