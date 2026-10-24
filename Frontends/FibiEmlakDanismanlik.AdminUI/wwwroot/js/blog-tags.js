(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        var tagsInput = document.getElementById('TagsText');
        if (!tagsInput) {
            return;
        }

        var tagButtons = document.querySelectorAll('button[data-tag]');
        tagButtons.forEach(function (button) {
            button.addEventListener('click', function () {
                var tagToAdd = (button.getAttribute('data-tag') || '').trim();
                if (!tagToAdd) {
                    return;
                }

                var currentVal = tagsInput.value || '';
                var existingTags = currentVal
                    .split(',')
                    .map(function (t) { return t.trim(); })
                    .filter(function (t) { return t.length > 0; });

                var targetLower = tagToAdd.toLocaleLowerCase('tr-TR');
                var alreadyExists = existingTags.some(function (t) {
                    return t.toLocaleLowerCase('tr-TR') === targetLower;
                });

                if (!alreadyExists) {
                    existingTags.push(tagToAdd);
                    tagsInput.value = existingTags.join(', ');
                    tagsInput.dispatchEvent(new Event('input', { bubbles: true }));
                    tagsInput.dispatchEvent(new Event('change', { bubbles: true }));
                }
            });
        });
    });
})();
