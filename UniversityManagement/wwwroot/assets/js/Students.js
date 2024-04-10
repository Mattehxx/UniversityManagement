/* GLOBAL VARIABLES */
let selectedStudentId = 0;
let allStudentsData = [];
let allCoursesData = [];
let allExamsData = [];

document.addEventListener("DOMContentLoaded", function (event) {
    GetAllStudents();
    GetAllCourses();
});

function GetAllStudents() {
    const endpoint = '/api/Student/GetAll';

    fetch(endpoint, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            response.json().then(json => {
                allStudentsData = json;
                FillStudentTable(json);
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

function FillStudentTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#studentsTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';
        data.forEach(element => {
            tableBody.innerHTML +=
                `<tr>
                    <td class="d-none">${element.studentId}</td>
                    <td>${element.name}</td>
                    <td>${element.surname}</td>
                    <td>${GetDateFromDateTime(element.birthDate)}</td>
                    <td>${element.fc}</td>
                    <td>
                        <button type="button" class="btn btn-outline-secondary" id="btnGet_${element.studentId}" 
                            data-bs-toggle="modal" data-bs-target="#missingexamsModal" title="Missing exams" onclick="GetMissingExams(this.id);">
                            <i class="fa-solid fa-file-circle-xmark"></i>
                        </button>
                        <button type="button" class="btn btn-outline-info" id="btnGet_${element.studentId}" 
                            data-bs-toggle="modal" data-bs-target="#bookletModal" title="Booklet" onclick="GetBooklet(this.id);">
                            <i class="fa-solid fa-book-open"></i>
                        </button>
                        <button type="button" class="btn btn-outline-warning" id="bntEdit_${element.studentId}"
                            data-bs-toggle="modal" data-bs-target="#editStudentModal" title="Edit student" onclick="FillEditStudentModal(this.id);">
                            <i class="fa-solid fa-pencil mr-1"></i>
                        </button>
                        <button type="button" class="btn btn-outline-danger" id="btnDelete_${element.studentId}" 
                            data-bs-toggle="modal" data-bs-target="#confirmModal" title="Delete student" onclick="selectedStudentId = GetIdFromString(this.id);">
                            <i class="fa-solid fa-trash mr-1"></i>   
                        </button>
                    </td>
                </tr>`;
        });
    }
}

async function GetStudentById(id) {
    const endpoint = `/api/Student/GetSingle/${id}`;

    const response = await fetch(endpoint, {
        method: 'GET'
    });

    if (!response.ok)
        return Promise.reject();

    return response.json();
}

function FillEditStudentModal(studentId) {
    selectedStudentId = GetIdFromString(studentId);
    let response = GetStudentById(selectedStudentId);

    response.then(student => {
        document.querySelector('#inpEditName').value = student.name;
        document.querySelector('#inpEditSurname').value = student.surname;
        document.querySelector('#inpEditBirthDate').value = GetDateFromDateTime(student.birthDate);
        document.querySelector('#inpEditFC').value = student.fc;
        document.querySelector('#slcEditCourse').value = student.courseId;
    })
        .catch(error => {
            console.error(error);
        });
}

function FillSlcCourse() {
    if (Array.isArray(allCoursesData)) {
        allCoursesData.forEach(course => {
            let html = `<option value="${course.courseId}">${course.title}</option>`;
            document.querySelector('#slcEditCourse').innerHTML += html;
            document.querySelector('#slcCourse').innerHTML += html;
            document.querySelector('#slcExportCourse').innerHTML += html;
        });
    }
}

function EditStudent() {
    const endpoint = `/api/Student/Edit`;

    let name = document.querySelector('#inpEditName').value;
    let surname = document.querySelector('#inpEditSurname').value;
    let birthDate = document.querySelector('#inpEditBirthDate').value;
    let fc = document.querySelector('#inpEditFC').value;
    let courseId = document.querySelector('#slcEditCourse').value;

    fetch(endpoint, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            StudentId: selectedStudentId,
            Name: name,
            Surname: surname,
            BirthDate: birthDate,
            FC: fc,
            CourseId: courseId
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllStudents();
            Alerts.Success('Student successfully edited');
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
        $('#editStudentModal').modal('hide');
    })

    return false;
}

function DeleteStudent() {
    const endpoint = `/api/Student/Delete/${selectedStudentId}`;

    fetch(endpoint, {
        method: 'DELETE'
    })
    .then(response => {
        if (response.ok) {
            GetAllStudents();
            Alerts.Success('Student successfully deleted');
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

function SearchStudent(surname) {
    let filteredData = allStudentsData.filter(element => {
        return element.surname.toLowerCase().includes(surname.toLowerCase()) ||
            element.name.toLowerCase().includes(surname.toLowerCase());
    });
    FillStudentTable(filteredData);
}

function AddStudent() {
    const endpoint = `/api/Student/Create`;

    let name = document.querySelector('#inpName').value;
    let surname = document.querySelector('#inpSurname').value;
    let birthDate = document.querySelector('#inpBirthDate').value;
    let fc = document.querySelector('#inpFC').value;
    let courseId = document.querySelector('#slcCourse').value;

    fetch(endpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            StudentId: 0,
            Name: name,
            Surname: surname,
            BirthDate: birthDate,
            FC: fc,
            CourseId: courseId
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllStudents();
            Alerts.Success('Student successfully added');
            document.querySelector('#addStudentForm').reset();
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

function GetBooklet(studentId) {
    selectedStudentId = GetIdFromString(studentId)
    const endpoint = `/api/Student/GetBooklet/${selectedStudentId}`;

    fetch(endpoint, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            response.json().then(json => {
                FillBookletTable(json.grades);
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

function FillBookletTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#bookletTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';
        data.forEach(element => {
            tableBody.innerHTML +=
                `<tr>
                    <td>${element.examTitle}</td>
                    <td>${element.examCredits}</td>
                    <td>${TranslateMark(element.mark)}</td>
                </tr>`;
        });
    }
}

function GetMissingExams(studentId) {
    selectedStudentId = GetIdFromString(studentId)
    const endpoint = `/api/Student/GetMissingExams/${selectedStudentId}`;

    fetch(endpoint, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            response.json().then(json => {
                console.log(json);
                FillMissingExamsTable(json);
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

function FillMissingExamsTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#missingexamsTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';
        data.forEach(element => {
            tableBody.innerHTML +=
                `<tr>
                    <td>${element.examTitle}</td>
                    <td>${element.examCredits}</td>
                    <td>${TranslateMark(element.mark)}</td>
                </tr>`;
        });
    }
}

function ExportStudents() {
    let courseId = document.querySelector('#slcExportCourse').value;

    const endpoint = `/api/Export/GetAverageGradeByStudent/${courseId}`;

    fetch(endpoint, {
        method: 'GET'
    })
    .then(response => {
        if (response.ok) {
            Alerts.Success('Students export on azure blob completed');
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