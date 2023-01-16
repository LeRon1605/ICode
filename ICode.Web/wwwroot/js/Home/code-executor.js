const btnSubmit = document.getElementById('btn-submit');
const inputTxt = document.getElementById('txt-input');
const outputTxt = document.getElementById('txt-output');
const memoryTxt = document.getElementById('memory-txt');
const timeTxt = document.getElementById('time-txt');
const statusTxt = document.getElementById('status-txt');
const languageSelector = document.getElementById('language_Selector');
const data = {
    c: {
        template: `
#include <stdio.h>
int main() {
    printf("Hello, World!");
    return 0;
}
        `,
        content: null
    },
    cpp: {
        template: `
#include <iostream>

int main() {
    std::cout << "Hello World!";
    return 0;
}
        `,
        content: null
    },
    java: {
        template: `
class Main {
    public static void main(String[] args) {
        // Insert code here 
    }
}
        `,
        content: null
    }
};

let flask = new CodeFlask('#txt-code', {
    language: 'c',
    lineNumbers: true
});
flask.addLanguage('c', Prism.languages['c']);

btnSubmit.addEventListener('click', async () => {
    if (flask.getCode().trim() != '') {
        btnSubmit.innerText = 'Pending';
        btnSubmit.classList.add('disabled');
        console.log(JSON.stringify({
            code: flask.getCode(),
            input: inputTxt.value,
            lang: languageSelector.value
        }));
        const response = await fetch('/services/code_executor', {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                code: flask.getCode(),
                input: inputTxt.value,
                lang: languageSelector.value
            })
        });
        try {
            const responseObj = await response.json();
            statusTxt.innerText = (responseObj.status == 1) ? "Success" : ((responseObj.status == 2) ? "Compiler Error" : "Runtime error");
            outputTxt.value = responseObj.output;
            timeTxt.innerText = responseObj.time + 's';
            memoryTxt.innerText = responseObj.memory + 'Kb';
        } catch {
            alert('Something went wrong!!');
        } finally {
            btnSubmit.innerText = 'Submit';
            btnSubmit.classList.remove('disabled');
        }
    }
});

$(document).ready(function () {
    $('select').niceSelect();
});

$('#wrapper').on('change', 'select', () => {
    data[flask.opts.language].content = flask.getCode();
    flask = new CodeFlask('#txt-code',
    {
        language: languageSelector.value,
        lineNumbers: true
    });
    flask.addLanguage(languageSelector.value, Prism.languages[languageSelector.value]);
    if (!data[languageSelector.value].content) {
        flask.updateCode(data[languageSelector.value].template.trim());
    } else {
        flask.updateCode(data[languageSelector.value].content.trim());
    }
});