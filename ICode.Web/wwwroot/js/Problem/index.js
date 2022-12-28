let flask = new CodeFlask('#codeBlock',
{
        language: 'c',
        lineNumbers: true
    });
flask.addLanguage('c', Prism.languages['c']);
$(document).ready(function () {
    $('select').niceSelect();
});
const languageSelector = document.getElementById('language_Selector');
const form = document.getElementById('form');

form.addEventListener('formdata', e => {
    const formData = e.formData;
    formData.set('Code', flask.getCode());
});

form.addEventListener('submit', async e => {
    if (flask.getCode() === '') {
        e.preventDefault();
    }
});

console.log(languageSelector)

$('#wrapper').on('change', 'select', () => {
    const code = flask.getCode();
    flask = new CodeFlask('#codeBlock',
        {
            language: languageSelector.value,
            lineNumbers: true
        });
    flask.addLanguage(languageSelector.value, Prism.languages[languageSelector.value]);
    flask.updateCode(code);
});