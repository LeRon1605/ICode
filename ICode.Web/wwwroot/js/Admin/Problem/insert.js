ClassicEditor
    .create(document.querySelector('#editor'))
    .catch(error => {
        console.error(error);
    });

const form = document.getElementById('insert-form');
const editor = document.getElementById('editor');
const btnAddTestcase = document.getElementById('btn-add-testcase');
const testcaseBlock = document.getElementById('testcase-block');
const block = {
    data: [],
    backup: function () {
        const inputValue = [...document.getElementsByClassName('testcase-input')].map(element => element.value);
        const outputValue = [...document.getElementsByClassName('testcase-output')].map(element => element.value);
        const memoryLimitValue = [...document.getElementsByClassName('testcase-memorylimit')].map(element => element.value);
        const timeLimitValue = [...document.getElementsByClassName('testcase-timelimit')].map(element => element.value);

        this.data = [];
        for (let i = 0; i < inputValue.length; i++) {
            this.data.push({
                input: inputValue[i],
                output: outputValue[i],
                timeLimit: timeLimitValue[i],
                memoryLimit: memoryLimitValue[i]
            });
        }
    },
    insert: function () {
        this.backup();
        this.data.push({
            input: '',
            output: '',
            timeLimit: '',
            memoryLimit: ''
        });
        this.render();
    },
    remove: function (index) {
        if (this.data.length > 1) {
            this.backup();
            this.data.splice(index, 1);
            this.render();
        }
    },
    render: function () {
        testcaseBlock.innerHTML = this.data.reduce((res, element, index) => res + renderTestcaseBlock(element, index), '');
    }
};

const deleteTestcase = (element) => block.remove(element.dataset.index);
const renderTestcaseBlock = (data, index) => {
    return `
        <div class="border rounded p-3 mb-3 testcaseBlock">
                        <div class="d-flex mb-3 justify-content-between">
                            <div class="col-6">
                                <label for="exampleInputEmail1" class="form-label">Input</label>
                                <textarea class="form-control testcase-input" placeholder="Nhập đầu vào" name="TestCases[${index}].Input" rows="1">${data.input}</textarea>
                                <span class="field-validation-valid form-text text-danger" data-valmsg-for="TestCases[${index}].Input" data-valmsg-replace="true"></span>
                            </div>
                            <div class="col-5">
                                <label for="exampleInputEmail1" class="form-label">Time Limit</label>
                                <input class="form-control testcase-timelimit" placeholder="Nhập giới hạn thời gian" name="TestCases[${index}].TimeLimit" value="${data.timeLimit}" data-val="true" data-val-required="Required field." data-val-number="Number field"/>
                                <span class="field-validation-valid form-text text-danger" data-valmsg-for="TestCases[${index}].TimeLimit" data-valmsg-replace="true"></span>
                            </div>
                        </div>
                        <div class="d-flex justify-content-between">
                            <div class="col-6">
                                <label for="exampleInputEmail1" class="form-label">Output</label>
                                <textarea class="form-control testcase-output" placeholder="Nhập đầu ra" name="TestCases[${index}].Output" data-val="true" data-val-required="Required field." rows="1">${data.output}</textarea>
                                <span class="field-validation-valid form-text text-danger" data-valmsg-for="TestCases[${index}].Output" data-valmsg-replace="true"></span>
                            </div>
                            <div class="col-5">
                                <label for="exampleInputEmail1" class="form-label">Memory Limit</label>
                                <input class="form-control testcase-memorylimit" placeholder="Nhập giới hạn bộ nhớ" name="TestCases[${index}].MemoryLimit" value="${data.memoryLimit}" data-val="true" data-val-required="Required field." data-val-number="Number field"/>
                                <span class="field-validation-valid form-text text-danger" data-valmsg-for="TestCases[0].MemoryLimit" data-valmsg-replace="true"></span>
                            </div>
                        </div>
                        <button class="btn btn-danger btnDelete" onClick="deleteTestcase(this)" data-index="${index}"></button>
        </div>
    `;
};

form.addEventListener('submit', e => {
    if (editor.value.trim().length < 10) {
        const editorValidate = document.getElementById('editor-validate');
        if (editorValidate.classList.contains('d-none')) {
            editorValidate.classList.remove('d-none');
        }
        document.getElementById('editor-validate').classList.add('d-block');
        e.preventDefault();
    }
});

btnAddTestcase.addEventListener('click', () => block.insert());