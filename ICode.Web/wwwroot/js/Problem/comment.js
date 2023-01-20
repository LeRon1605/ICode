const commentBlock = document.getElementById('comment-block');
const handleSubmit = async (btnSubmit, parentId = null) => {
    try {
        const body = {
            content: btnSubmit.parentElement.querySelector('.content-input').value.trim(),
            parentId
        };
        if (body.content == '' || body.content.length < 10) {
            alert('Phải nhập ít nhất 10 kí tự mới được comment nha.')
            return;
        }
        const response = await fetch(`http://localhost:5001/problems/${problemId}/comments`, {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getCookie('access_token')}`
            },
            body: JSON.stringify(body)
        });
        if (response.status === 401) {
            alert('Bạn cần phải đăng nhập.');
            return;
        }
        const comment = await response.json();
        if (parentId == null) {
            commentBlock.innerHTML = renderCommentBlock(comment) + commentBlock.innerHTML;
        } else {
            btnSubmit.parentElement.parentElement.querySelector('.reply-block').innerHTML += renderReplyBlock(comment);
        }
        btnSubmit.parentElement.querySelector('.content-input').value = '';
    } catch (exception) {
        alert('Something went wrong');
    }
};

const renderCommentBlock = (obj) => `
    <div class="col text-decoration-none text-dark border-start rounded mb-5">
        <div class="d-flex justify-content-between align-items-start rounded p-3 bg-white shadow comment">
            <div class="d-flex col-12">
                <img class="rounded-circle flex-shrink-0 me-3 fit-cover border" width="50" height="50" src="${user.image}">
                <div style="flex-grow: 1">
                    <div class="d-flex justify-content-between">
                        <p class="fw-bold mb-0">${user.username}</p>
                        <span>${new Date(obj.at).toLocaleString()}</span>
                    </div>
                    <p class="text-muted mb-0">${obj.content}</p>
                </div>
            </div>
            <div class="comment__action">
                <button class="bg-white rounded-circle shadow p-2 mb-2 d-flex align-items-center justify-content-center comment__action_icon" aria-describedby="Chỉnh sửa">
                    <i class="fa-solid fa-pen-to-square"></i>
                </button>
                <button class="bg-white rounded-circle shadow p-2 d-flex align-items-center justify-content-center comment__action_icon" data-id="${obj.id}" data-bs-toggle="modal" data-bs-target="#deleteModal">
                    <i class="fa-solid fa-trash text-danger"></i>
                </button>
            </div>
        </div>
        <div class="mt-2 reply-block"></div>
        <div class="d-flex ms-1" style="transform: translateY(50%);">
            <input class="form-control content-input" placeholder="Nhập bình luận"/>
            <button class="btn btn-primary col-2 ms-2" onClick="handleSubmit(this, '${obj.id}')">Phản hồi</button>
        </div>
    </div>
`;

const renderReplyBlock = (obj) => `
    <div class="col text-decoration-none text-dark mb-2">
        <div class="d-flex align-items-center mb-2">
            <div style="height: 1px; background-color: #cfcfcf;" class="col-1"></div>
            <div class="d-flex justify-content-between align-items-start rounded p-3 bg-white shadow col-11 comment">
                <div class="d-flex col-12">
                    <img class="rounded-circle flex-shrink-0 me-3 fit-cover border" width="50" height="50" src="${user.image}">
                    <div style="flex-grow: 1">
                        <div class="d-flex justify-content-between">
                            <p class="fw-bold mb-0">${user.username}</p>
                            <span>${new Date(obj.at).toLocaleString()}</span>
                        </div>
                        <p class="text-muted mb-0">${obj.content}</p>
                    </div>
                </div>
                <div class="comment__action">
                    <button class="bg-white rounded-circle shadow p-2 mb-2 d-flex align-items-center justify-content-center comment__action_icon" title="Chỉnh sửa">
                        <i class="fa-solid fa-pen-to-square"></i>
                    </button>
                    <button class="bg-white rounded-circle shadow p-2 d-flex align-items-center justify-content-center comment__action_icon" title="Xóa" data-id="${obj.id}" data-bs-toggle="modal" data-bs-target="#deleteModal">
                        <i class="fa-solid fa-trash text-danger"></i>
                    </button>
                </div>
            </div>
        </div>
    </div>
`;

function getCookie(cname) {
    let name = cname + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

let commentDelete;
const deleteInput = document.getElementById('delete-id');
const deleteBtn = document.getElementById('delete-btn');
const deleteForm = document.getElementById('delete-form');

$('#deleteModal').on('show.bs.modal', function (event) {
    commentDelete = event.relatedTarget.parentElement.parentElement.parentElement;
    deleteInput.value = event.relatedTarget.dataset.id;
});

deleteForm.addEventListener('click', async e => {
    e.preventDefault();
    const response = await fetch(`http://localhost:5001/comments/${deleteInput.value}`, {
        method: 'DELETE',
        mode: 'cors',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${getCookie('access_token')}`
        }
    });
    if (response.status == 204) {
        commentDelete.remove();
    } 
    $('#deleteModal').modal('hide');
});