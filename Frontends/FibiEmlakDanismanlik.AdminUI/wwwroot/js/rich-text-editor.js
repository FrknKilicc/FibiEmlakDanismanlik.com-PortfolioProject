/**
 * Fibi Emlak Danışmanlık - Genel Zengin Metin Editörü (TinyMCE)
 * textarea[data-rich-text] alanlarını editöre çevirir (ör. danışman açıklaması).
 * İlan açıklaması kendi dosyasını kullanır (listing-description-editor.js).
 */
document.addEventListener('DOMContentLoaded', function () {
    if (typeof tinymce === 'undefined') {
        return;
    }

    var dark = document.documentElement.getAttribute('data-bs-theme') === 'dark';

    document.querySelectorAll('textarea[data-rich-text]').forEach(function (textarea) {
        // Düz metin kayıtları HTML paragraflarına dönüştür
        var val = textarea.value || '';
        if (val.trim() && !/<\/?[a-z][\s\S]*>/i.test(val)) {
            var escaped = val
                .replace(/&/g, '&amp;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;');
            textarea.value = escaped.split(/\r?\n\s*\r?\n/).map(function (p) {
                return '<p>' + p.replace(/\r?\n/g, '<br>') + '</p>';
            }).join('');
        }

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
            height: 260,
            plugins: 'lists link autolink wordcount',
            toolbar: 'undo redo | bold italic underline | bullist numlist | link | removeformat',
            valid_elements: 'p,br,strong/b,em/i,u,ul,ol,li,a[href|target|rel]',
            link_default_target: '_blank',
            link_assume_external_targets: 'https',
            convert_urls: false,
            entity_encoding: 'raw',
            readonly: textarea.disabled,
            skin: dark ? 'oxide-dark' : 'oxide',
            content_css: dark ? 'dark' : 'default',
            content_style: 'body{font-family:"Open Sans",system-ui,sans-serif;font-size:15px;line-height:1.6}',
            setup: function (editor) {
                editor.on('input change undo redo keyup SetContent', function () {
                    editor.save();
                });
            }
        });

        // Gönderimden önce (capture) editör içeriğini textarea'ya yaz
        var form = textarea.closest('form');
        if (form) {
            form.addEventListener('submit', function () {
                tinymce.triggerSave();
            }, true);
        }
    });
});
