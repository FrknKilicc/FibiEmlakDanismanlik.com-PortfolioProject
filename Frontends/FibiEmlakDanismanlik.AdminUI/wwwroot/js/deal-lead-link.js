(function () {
    'use strict';

    function initDealLeadLink() {
        var leadSelect = document.getElementById('LinkedLead');
        var nameInput = document.getElementById('CustomerName');
        var phoneInput = document.getElementById('CustomerPhone');

        if (!leadSelect || !nameInput || !phoneInput) {
            return;
        }

        function fillIfEmpty() {
            var selectedOption = leadSelect.options[leadSelect.selectedIndex];
            if (!selectedOption || !selectedOption.value) {
                return;
            }

            var leadName = selectedOption.getAttribute('data-name');
            var leadPhone = selectedOption.getAttribute('data-phone');

            if (leadName && nameInput.value.trim() === '') {
                nameInput.value = leadName;
            }

            if (leadPhone && phoneInput.value.trim() === '') {
                phoneInput.value = leadPhone;
            }
        }

        leadSelect.addEventListener('change', function () {
            fillIfEmpty();
        });

        // Sayfa ilk açıldığında önceden seçili talep varsa ve alanlar boşsa doldur
        fillIfEmpty();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initDealLeadLink);
    } else {
        initDealLeadLink();
    }
})();
