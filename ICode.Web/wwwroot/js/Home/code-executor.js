const btnSubmit = document.getElementById('btn-submit');
const inputTxt = document.getElementById('txt-input');
const outputTxt = document.getElementById('txt-output');
const memoryTxt = document.getElementById('memory-txt');
const timeTxt = document.getElementById('time-txt');
const statusTxt = document.getElementById('status-txt');
const languageSelector = document.getElementById('language_Selector');

let flask = new CodeFlask('#txt-code', {
        language: 'cpp',
        lineNumbers: true
});
flask.addLanguage('cpp', Prism.languages['cpp']);

btnSubmit.addEventListener('click', async () => {
    if (flask.getCode().trim() != '') {
        btnSubmit.innerText = 'Processing';
        btnSubmit.classList.add('disabled');
        const response = await fetch('http://localhost:5000/execute', {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                code: flask.getCode(),
                input: inputTxt.value,
                lang: 'cpp'
            })
        });
        const responseObj = await response.json();
        console.log(responseObj);
        statusTxt.innerText = (responseObj.status == 1) ? "Success" : ((responseObj.status == 2) ? "Compiler Error" : "Runtime error");
        outputTxt.value = responseObj.output;
        timeTxt.innerText = responseObj.time + 's';
        memoryTxt.innerText = responseObj.memory + 'Mb';
        btnSubmit.innerText = 'Submit';
        btnSubmit.classList.remove('disabled');
    }
});

$(document).ready(function () {
    $('select').niceSelect();
});

$('#wrapper').on('change', 'select', () => {
    const code = flask.getCode();
    flask = new CodeFlask('#txt-code',
        {
            language: languageSelector.value,
            lineNumbers: true
        });
    flask.addLanguage(languageSelector.value, Prism.languages[languageSelector.value]);
    flask.updateCode(code);
});