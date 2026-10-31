/**
 * Fibi Emlak Danışmanlık - İlan Açıklaması Zengin Metin Editörü (TinyMCE Entegrasyonu)
 */
document.addEventListener('DOMContentLoaded', function () {
    var textarea = document.getElementById('propertyDescription');
    if (!textarea || typeof tinymce === 'undefined') {
        return;
    }

    // Düz metin kayıtları HTML paragraflarına dönüştür
    var val = textarea.value || '';
    if (val.trim() && !/<\/?[a-z][\s\S]*>/i.test(val)) {
        var escaped = val
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
        var paras = escaped.split(/\r?\n\s*\r?\n/);
        var htmlParas = paras.map(function (p) {
            var withBrs = p.replace(/\r?\n/g, '<br>');
            return '<p>' + withBrs + '</p>';
        });
        textarea.value = htmlParas.join('');
    }

    var dark = document.documentElement.getAttribute('data-bs-theme') === 'dark';

    tinymce.init({
        target: textarea,
        base_url: '/assets/vendor/tinymce',
        suffix: '.min',
        language: 'tr',
        language_url: '/assets/vendor/tinymce/langs/tr.js',
        menubar: false,
        branding: false,
        promotion: false,
        statusbar: true,
        elementpath: false,
        height: 360,
        plugins: 'lists link autolink wordcount',
        toolbar: 'undo redo | blocks | bold italic underline | bullist numlist | link | removeformat',
        block_formats: 'Paragraf=p; Başlık=h4',
        valid_elements: 'p,br,strong/b,em/i,u,ul,ol,li,h4,a[href|target|rel]',
        link_default_target: '_blank',
        link_assume_external_targets: 'https',
        convert_urls: false,
        entity_encoding: 'raw',
        readonly: textarea.disabled,
        skin: dark ? 'oxide-dark' : 'oxide',
        content_css: dark ? 'dark' : 'default',
        content_style: 'body{font-family:"Open Sans",system-ui,sans-serif;font-size:15px;line-height:1.6}',
        setup: function (editor) {
            editor.on('keydown mousedown paste', function () {
                textarea.dispatchEvent(new CustomEvent('fibi-user-intent', { bubbles: true }));
            });

            editor.on('input change undo redo keyup SetContent', function () {
                editor.save();
                textarea.dispatchEvent(new Event('input', { bubbles: true }));
            });

            editor.on('keydown', function (e) {
                if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 's') {
                    e.preventDefault();
                    tinymce.triggerSave();
                    var f = document.getElementById('listingForm');
                    if (f) {
                        if (typeof f.requestSubmit === 'function') {
                            f.requestSubmit();
                        } else {
                            f.submit();
                        }
                    }
                }
            });
        }
    });

    // Form submit capture aşamasında textarea'ya kaydet
    var listingForm = document.getElementById('listingForm');
    if (listingForm) {
        listingForm.addEventListener('submit', function () {
            tinymce.triggerSave();
        }, true);
    }
});
