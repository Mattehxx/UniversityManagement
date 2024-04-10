/* GLOBAL VARIABLES */
let selectedProfessorId = 0;
let allProfessorsData = [];
let allCoursesData = [];

document.addEventListener("DOMContentLoaded", function (event) {
    GetAllProfessors();
    GetAllCourses();
});

function GetAllProfessors() {
    const endpoint = '/api/Professor/GetAll';

    fetch(endpoint, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            response.json().then(json => {
                allProfessorsData = json;
                FillProfessorTable(allProfessorsData);
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

function FillProfessorTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#professorsTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';
        data.forEach(element => {
            tableBody.innerHTML +=
                `<tr>
                    <td class="d-none">${element.professorId}</td>
                    <td>${element.name}</td>
                    <td>${element.surname}</td>
                    <td>${GetDateFromDateTime(element.birthDate)}</td>
                    <td>${element.fc}</td>
                    <td>
                        <button type="button" class="btn btn-outline-secondary" id="bntEdit_${element.professorId}"
                            data-bs-toggle="modal" data-bs-target="#futureExamModal" title="Future exam session" onclick="GetFutureExamSession(this.id);">
                            <i class="fa-solid fa-calendar-days"></i>
                        </button>
                        <button type="button" class="btn btn-outline-warning" id="bntEdit_${element.professorId}"
                            data-bs-toggle="modal" data-bs-target="#editProfessorModal" title="Edit professor" onclick="FillEditProfessorModal(this.id);">
                            <i class="fa-solid fa-pencil mr-1"></i>
                        </button>
                        <button type="button" class="btn btn-outline-danger" id="btnDelete_${element.professorId}" 
                            data-bs-toggle="modal" data-bs-target="#confirmModal" title="Delete professor" onclick="selectedProfessorId = GetIdFromString(this.id);">
                            <i class="fa-solid fa-trash mr-1"></i> 
                        </button>
                    </td>
                </tr>`;
        });
    }
}

async function GetProfessorById(id) {
    const endpoint = `/api/Professor/GetSingle/${id}`;

    const response = await fetch(endpoint, {
        method: 'GET'
    });

    if (!response.ok)
        return Promise.reject();

    return response.json();
}

function FillEditProfessorModal(professorId) {
    selectedProfessorId = GetIdFromString(professorId);
    let response = GetProfessorById(selectedProfessorId);

    response.then(professor => {
        document.querySelector('#inpEditName').value = professor.name;
        document.querySelector('#inpEditSurname').value = professor.surname;
        document.querySelector('#inpEditBirthDate').value = GetDateFromDateTime(professor.birthDate);
        document.querySelector('#inpEditFC').value = professor.fc;
    })
        .catch(error => {
            console.error(error);
        });
}

function EditProfessor() {
    const endpoint = `/api/Professor/Edit`;

    let name = document.querySelector('#inpEditName').value;
    let surname = document.querySelector('#inpEditSurname').value;
    let birthDate = document.querySelector('#inpEditBirthDate').value;
    let fc = document.querySelector('#inpEditFC').value;

    fetch(endpoint, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ProfessorId: selectedProfessorId,
            Name: name,
            Surname: surname,
            BirthDate: birthDate,
            FC: fc,
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllProfessors();
            Alerts.Success('Professor successfully edited');
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
        $('#editProfessorModal').modal('hide');
    })

    return false;
}

function DeleteProfessor() {
    const endpoint = `/api/Professor/Delete/${selectedProfessorId}`;

    fetch(endpoint, {
        method: 'DELETE'
    })
    .then(response => {
        if (response.ok) {
            GetAllProfessors();
            Alerts.Success('Professor successfully deleted');
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

function SearchProfessor(surname) {
    let filteredData = allProfessorsData.filter(element => {
        return element.surname.toLowerCase().includes(surname.toLowerCase()) ||
            element.name.toLowerCase().includes(surname.toLowerCase());
    });
    FillProfessorTable(filteredData);
}

function AddProfessor() {
    const endpoint = `/api/Professor/Create`;

    let name = document.querySelector('#inpName').value;
    let surname = document.querySelector('#inpSurname').value;
    let birthDate = document.querySelector('#inpBirthDate').value;
    let fc = document.querySelector('#inpFC').value;

    fetch(endpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ProfessorId: 0,
            Name: name,
            Surname: surname,
            BirthDate: birthDate,
            FC: fc,
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllProfessors();
            Alerts.Success('Professor successfully added');
            document.querySelector('#addProfessorForm').reset();
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


//GETFUTUREEXAMSESSION

function GetFutureExamSession(professorId) {
    selectedProfessorId = GetIdFromString(professorId);

    const endpoint = `/api/Professor/GetFutureExamSession/${selectedProfessorId}`;

    fetch(endpoint, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            response.json().then(json => {
                allFutureExamsData = json;
                FillFutureExamTable(allFutureExamsData);
            })
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
}

function FillFutureExamTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#futureExamsTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';
        data.forEach(element => {
            tableBody.innerHTML +=
                `<tr>
                    <td class="d-none">${element.professorId}</td>
                    <td>${FormatDateTime(element.dtSession)}</td>
                    <td>${element.examCredits}</td>
                    <td>${element.examTitle}</td>
                    <td>${element.location}</td>
                    </tr>`;
        });
    }
}

function GetAllCourses() {
    const endpoint = '/api/Course/GetAll';

    fetch(endpoint, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            response.json().then(json => {
                allCoursesData = json;
                FillSlcCourse();
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

function FillSlcCourse() {
    if (Array.isArray(allCoursesData)) {
        allCoursesData.forEach(course => {
            let html = `<option value="${course.courseId}">${course.title}</option>`;
            document.querySelector('#slcExportCourse').innerHTML += html;
        });
    }
}

function ExportProfessors() {
    let courseId = document.querySelector('#slcExportCourse').value;

    const endpoint = `/api/Export/GetAveragePromotedStudentByExam/${courseId}`;

    fetch(endpoint, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            Alerts.Success('Professors export on azure blob completed');
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
    })

    return false;
}