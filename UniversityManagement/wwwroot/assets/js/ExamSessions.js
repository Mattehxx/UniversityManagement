/* GLOBAL VARIABLES */
let selectedExamSessionId = 0;
let allExamSessionsData = [];
let allExamsData = [];

document.addEventListener("DOMContentLoaded", function (event) {
    GetAllExamSessions();
    GetAllExams();
})

function GetAllExamSessions() {
    const endpoint = '/api/ExamSession/GetAll'

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                response.json().then(json => {
                    allExamSessionsData = json;
                    FillExamSessionTable(allExamSessionsData);
                })
            } else {
                if (response.status == 500) {
                    Alerts.Error('Internal server error');
                    console.error('Internal server error');
                } else {
                    response.text().then(message => {
                        Alerts.Warning(message);
                        console.warn(message);
                    })
                }
            }
        })
        .catch(error => {
            Alerts.Error(error);
            console.error(error);
        });
}

function FillExamSessionTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#examsessionsTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';
        data.forEach(element => {
            tableBody.innerHTML +=
                `<tr>
                <td class="d-none">${element.examSessionId}</td>
                <td>${FormatDateTime(element.dtSession)}</td>
                <td>${element.location}</td>
                <td>${element.examTitle}</td>
                <td>${element.examCredits}</td>
                <td>
                    <button type="button" class="btn btn-outline-warning" id="bntEdit_${element.examSessionId}"
                        data-bs-toggle="modal" data-bs-target="#editExamSessionModal" title="Edit exam session" onclick="FillEditExamSessionModal(this.id);">
                        <i class="fa-solid fa-pencil mr-1"></i>
                    </button>
                    <button type="button" class="btn btn-outline-danger" id="btnDelete_${element.examSessionId}" 
                        data-bs-toggle="modal" data-bs-target="#confirmModal" title="Delete exam session" onclick="selectedExamSessionId = GetIdFromString(this.id);">
                        <i class="fa-solid fa-trash mr-1"></i>
                    </button>
                </td>
            </tr>`;
        })
    }
}

function FillEditExamSessionModal(ExamSessionId) {
    selectedExamSessionId = GetIdFromString(ExamSessionId);
    let response = GetExamSessionById(selectedExamSessionId);

    response.then(examsession => {
        document.querySelector('#inpEditDtSession').value = examsession.dtSession;
        document.querySelector('#inpEditLocation').value = examsession.location;
        document.querySelector('#slcEditExam').value = examsession.examId;
    })
        .catch(error => {
            console.error(error);
        });
}

function EditExamSession() {
    const endpoint = `/api/ExamSession/Edit`;

    let dtSession = document.querySelector('#inpEditDtSession').value;
    let location = document.querySelector('#inpEditLocation').value;
    let examId = document.querySelector('#slcEditExam').value;

    fetch(endpoint, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ExamSessionId: selectedExamSessionId,
            DtSession: dtSession,
            Location: location,
            ExamId: examId,
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllExamSessions();
            Alerts.Success('Exam Session successfully edited');
        }
        else {
            if (response.status == 500) {
                Alerts.Error('Internal server error');
                console.error('Internal server error');
            } else {
                response.text().then(message => {
                    Alerts.Warning(message);
                    console.warn(message);
                })
            }
        }
    })
    .catch(error => {
        Alerts.Error(error);
        console.error(error);
    })
    .finally(() => {
        $('#editExamSessionModal').modal('hide');
    })

    return false;
}

async function GetExamSessionById(id) {
    const endpoint = `/api/ExamSession/GetSingle/${id}`;

    const response = await fetch(endpoint, {
        method: 'GET'
    });

    if (!response.ok)
        return Promise.reject();

    return response.json();
}

function DeleteExamSession() {
    const endpoint = `/api/ExamSession/Delete/${selectedExamSessionId}`;

    fetch(endpoint, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                GetAllExamSessions();
                Alerts.Success('Exam Session successfully deleted');
            }
            else {
                if (response.status == 500) {
                    Alerts.Error('Internal server error');
                    console.error('Internal server error');
                } else {
                    response.text().then(message => {
                        Alerts.Warning(message);
                        console.warn(message);
                    })
                }
            }
        })
        .catch(error => {
            Alerts.Error(error);
            console.error(error);
        });
}

function AddExamSession() {
    const endpoint = `/api/ExamSession/Create`;

    let dtSession = document.querySelector('#inpDateSession').value;
    let location = document.querySelector('#inpLocation').value;
    let examId = document.querySelector('#slcExam').value;

    fetch(endpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ExamSessionId: 0,
            DtSession: dtSession,
            Location: location,
            ExamId: examId,
        })
    })
        .then(response => {
            if (response.ok) {
                GetAllExamSessions();
                Alerts.Success('Exam Session successfully added');
                document.querySelector('#addExamSessionForm').reset();
            }
            else {
                if (response.status == 500) {
                    Alerts.Error('Internal server error');
                    console.error('Internal server error');
                } else {
                    response.text().then(message => {
                        Alerts.Warning(message);
                        console.warn(message);
                    })
                }
            }
        })
        .catch(error => {
            Alerts.Error(error);
            console.error(error);
        })

    return false;
}
function GetAllExams() {
    const endpoint = '/api/Exam/GetAll';

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                response.json().then(json => {
                    allExamsData = json;
                    FillSlcExam();
                })
            } else {
                if (response.status == 500) {
                    Alerts.Error('Internal server error');
                    console.error('Internal server error');
                } else {
                    response.text().then(message => {
                        Alerts.Warning(message);
                        console.warn(message);
                    })
                }
            }
        })
        .catch(error => {
            Alerts.Error(error);
            console.error(error);
        });
}

function FillSlcExam() {
    if (Array.isArray(allExamsData)) {
        allExamsData.forEach(exam => {
            let html = `<option value="${exam.examId}">${exam.title}</option>`;
            document.querySelector('#slcEditExam').innerHTML += html;
            document.querySelector('#slcExam').innerHTML += html;
        });
    }
}

function SearchExamSessions(input) {
    let filteredData = allExamSessionsData.filter(element => {
        return element.examTitle.toLowerCase().includes(input.toLowerCase());
    });
    FillExamSessionTable(filteredData);
}
