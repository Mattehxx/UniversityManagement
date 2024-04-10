let selectedExamId = 0;
let selectedExamSessionId = 0;
let allExamsData = [];
let allCoursesData = [];
let allProfessorsData = [];
let allStudentsData = [];

/*GET ALL*/
document.addEventListener("DOMContentLoaded", function (event) {
    GetAllExams();
    GetAllCourses();
    GetAllProfessors();
    GetAllStudents();
});


function GetAllExams() {
    const endpoint = '/api/Exam/GetAll';

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                response.json().then(json => {
                    allExamsData = json;
                    FillExamsTable(json);
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

function FillExamsTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#examsTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';
        data.forEach(element => {

            tableBody.innerHTML +=
                `<tr>
                <td class="d-none">${element.examId}</td>
                <td>${element.title}</td>
                <td>${element.credits}</td>
                <td>${element.courseTitle}</td>
                <td>${element.professorSurname}</td>
                <td>
                    <button type="button" class="btn btn-outline-secondary" id="bntExamSessions_${element.examId}"
                            data-bs-toggle="modal" data-bs-target="#examSessionsModal" title="Open exam sessions" onclick="FillExamSessionTable(this.id);">
                            <i class="fa-solid fa-calendar-days"></i>
                    </button>
                    <button type="button" class="btn btn-outline-info" id="bntExamStudents_${element.examId}"
                            data-bs-toggle="modal" data-bs-target="#editExamStudentsModal" title="Students grades" onclick="GetExamStudents(this.id);">
                        <i class="fa-solid fa-address-card"></i>
                    </button>
                    <button type="button" class="btn btn-outline-warning" id="bntEdit_${element.examId}"
                                data-bs-toggle="modal" data-bs-target="#editExamModal" title="Edit exam" onclick="FillEditExamModal(this.id);">
                        <i class="fa-solid fa-pencil mr-1"></i>
                    </button>
                    <button type="button" class="btn btn-outline-danger" id="btnDelete_${element.examId}" 
                        data-bs-toggle="modal" data-bs-target="#confirmModal" title="Delete exam" onclick="selectedExamId = GetIdFromString(this.id);">
                        <i class="fa-solid fa-trash mr-1"></i>
                    </button>
                </td>
            </tr>`;
        })
    }
}



/*GETBYID*/
async function GetExamById(id) {
    const endpoint = `/api/Exam/GetSingle/${id}`;

    const response = await fetch(endpoint, {
        method: 'GET'
    });

    if (!response.ok)
        return Promise.reject();

    return response.json();
}

/*EDIT EXAM*/
function FillEditExamModal(examId) {
    selectedExamId = GetIdFromString(examId);
    let response = GetExamById(selectedExamId);

    response.then(exam => {
        document.querySelector('#inpEditTitle').value = exam.title;
        document.querySelector('#inpEditCredits').value = exam.credits;
        document.querySelector('#slcEditCourse').value = exam.courseId;
        document.querySelector('#slcEditProfessor').value = exam.professorId;
    })
        .catch(error => {
            console.error(error);
        });
}

function EditExam() {
    const endpoint = `/api/Exam/Edit`;

    let title = document.querySelector('#inpEditTitle').value;
    let credits = document.querySelector('#inpEditCredits').value;
    let courseId = document.querySelector('#slcEditCourse').value;
    let professorId = document.querySelector('#slcEditProfessor').value;

    fetch(endpoint, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ExamId: selectedExamId,
            Title: title,
            Credits: credits,
            CourseId: courseId,
            ProfessorId: professorId
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllExams();
            Alerts.Success('Exam successfully edited');
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
        $('#editExamModal').modal('hide');
    })

    return false;
}

function DeleteExam() {
    const endpoint = `/api/Exam/Delete/${selectedExamId}`;

    fetch(endpoint, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                GetAllExams();
                Alerts.Success('Exam successfully deleted');
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
            document.querySelector('#slcEditCourse').innerHTML += html;
            document.querySelector('#slcCourse').innerHTML += html;
            document.querySelector('#slcExportCourse').innerHTML += html;
        });
    }
}


function GetAllProfessors() {
    const endpoint = '/api/Professor/GetAll';

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                response.json().then(json => {
                    allProfessorsData = json;
                    FillSlcProfessor();
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
function FillSlcProfessor() {
    if (Array.isArray(allProfessorsData)) {
        allProfessorsData.forEach(professor => {
            let html = `<option value="${professor.professorId}">${professor.surname}</option>`;
            document.querySelector('#slcEditProfessor').innerHTML += html;
            document.querySelector('#slcProfessor').innerHTML += html;
        });
    }
}

function AddExam() {
    const endpoint = `/api/Exam/Create`;

    let title = document.querySelector('#inpTitle').value;
    let credits = document.querySelector('#inpCredits').value;
    let courseId = document.querySelector('#slcCourse').value;
    let professorId = document.querySelector('#slcProfessor').value;


    fetch(endpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ExamId: 0,
            Title: title,
            Credits: credits,
            CourseId: courseId,
            ProfessorId: professorId
        })
    })
        .then(response => {
            if (response.ok) {
                GetAllExams();
                Alerts.Success('Exam successfully added');
                document.querySelector('#addExamForm').reset();
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

function SearchExam(input) {
    let filteredData = allExamsData.filter(element => {
        return element.title.toLowerCase().includes(input.toLowerCase()) ||
            element.courseTitle.toLowerCase().includes(input.toLowerCase());
    });
    FillExamsTable(filteredData);
}

//VISUALIZZA STUDENTI ISCRITTI ALL'ESAME
function GetExamStudents(examId) {
    selectedExamId = GetIdFromString(examId);

    const endpoint = `/api/Exam/GetExamStudents/${selectedExamId}`;

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                response.json().then(json => {
                    console.log(json);
                    FillExamStudentTable(json.sessions);
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

function FillExamStudentTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#studentExamsTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';

        data.forEach(session => {
            session.gradeList.forEach(grade => {
                tableBody.innerHTML +=
                    `<tr>
                    <td>${grade.studentName}</td>
                    <td>${grade.studentSurname}</td>
                    <td>${TranslateMark(grade.mark)}</td>
                </tr>`;
            })
        })
    }
}

function FillExamSessionTable(id) {
    selectedExamId = GetIdFromString(id);
    const response = GetExamById(selectedExamId);

    response.then(data => {
        if (Array.isArray(data.sessions)) {
            let tableBody = document.querySelector('#examSessionsTableBody');
            if (tableBody.innerHTML != '')
                tableBody.innerHTML = '';

            data.sessions.forEach(session => {
                tableBody.innerHTML +=
                    `<tr>
                        <td>${FormatDateTime(session.dtSession)}</td>
                        <td>${session.location}</td>
                        <td>
                            <button type="button" class="btn btn-outline-success" id="btnAddStudents_${session.examSessionId}" 
                                data-bs-toggle="modal" data-bs-target="#addStudentModal" title="Add student" onclick="selectedExamSessionId = GetIdFromString(this.id); GetStudentsByExam();">
                                <i class="fa-solid fa-user-plus"></i>
                            </button>
                            <button type="button" class="btn btn-outline-info" id="btnAddMarks_${session.examSessionId}" 
                                data-bs-toggle="modal" data-bs-target="#editGradesModal" title="Edit grades" onclick="selectedExamSessionId = GetIdFromString(this.id); GetExamSessionById();">
                                <i class="fa-solid fa-marker"></i>
                            </button>
                            <button type="button" class="btn btn-outline-danger" id="btnDelete_${session.examSessionId}" 
                                data-bs-toggle="modal" data-bs-target="#confirmExamSessionModal" title="Delete exam session" onclick="selectedExamSessionId = GetIdFromString(this.id);">
                                <i class="fa-solid fa-trash"></i>
                            </button>
                        </td>
                    </tr>`;
            })
        }
    })
}

function AddExamSession() {
    const endpoint = `/api/Exam/CreateExamSession/`;

    let dateTime = document.querySelector('#inpExamSessionDt').value;
    let location = document.querySelector('#inpExamSessionLocation').value;

    fetch(endpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ExamSessionId: 0,
            DtSession: dateTime,
            Location: location,
            ExamId: selectedExamId
        })
    })
    .then(response => {
        if (response.ok) {
            Alerts.Success('Exam session successfully edited');
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
        $('#addExamSessionsModal').modal('hide');
    })

    return false;
}

function DeleteExamSession() {
    const endpoint = `/api/ExamSession/Delete/${selectedExamSessionId}`;

    fetch(endpoint, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                Alerts.Success('Exam session successfully deleted');
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

function GetAllStudents() {
    const endpoint = '/api/Student/GetAll';

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                response.json().then(json => {
                    allStudentsData = json;
                    FillStudentsSlc(json);
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

function FillStudentsSlc(data) {
    if (Array.isArray(data)) {
        let slcStudents = document.querySelector('#slcAddStudents');
        if (slcStudents.innerHTML != '')
            slcStudents.innerHTML = '';

        data.forEach(student => {
            let html = `<option value="${student.studentId}">${student.surname}</option>`;
            slcStudents.innerHTML += html;
        });
    }
}

function AddStudents() {
    const endpoint = `/api/Exam/CreateMassiveStudentsGrade`

    let studentIds = GetSelectedOptionsFromMultiSelect(document.querySelector('#slcAddStudents').options);

    fetch(endpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            SessionId: selectedExamSessionId,
            StudentIds: studentIds
        })
    })
        .then(response => {
            if (response.ok) {
                Alerts.Success('Students successfully booked to exam session');
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

function GetStudentsByExam() {
    let exam = allExamsData.filter(element => {
        return element.examId == selectedExamId;
    });

    let filteredData = allStudentsData.filter(element => {
        return element.courseId == exam[0].courseId;
    });
    FillStudentsSlc(filteredData);
}

function ExportExams() {
    let courseId = document.querySelector('#slcExportCourse').value;

    const endpoint = `/api/Export/GetAverageGradeByExam/${courseId}`;

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                Alerts.Success('Exams export on azure blob completed');
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

function GetExamSessionById() {
    const endpoint = `/api/ExamSession/GetSingle/${selectedExamSessionId}`;

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                response.json().then(json => {
                    FillEditGradesTable(json);
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
        })
}

function FillEditGradesTable(data) {
    if (Array.isArray(data.grades)) {
        let tableBody = document.querySelector('#editGradesTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';

        data.grades.forEach(element => {
            if (element.mark == null) {
                tableBody.innerHTML +=
                    `<tr>
                    <td>${element.studentName}</td>
                    <td>${element.studentSurname}</td>
                    <td>
                        <input type="number" class="form-control input-marks" id="inpMark_${element.studentId}" placeholder="Insert mark here" min="18" max="31">
                    </td>
                </tr>`;
            }
        })
    }
}

function GetAllMarks() {
    let studentMarks = [];
    let inputs = Array.from(document.querySelectorAll('.input-marks'));

    if (Array.isArray(inputs)) {
        inputs.forEach(input => {
            if (input.value != '') {
                studentMarks.push({
                    StudentId: GetIdFromString(input.id),
                    SessionId: selectedExamSessionId,
                    Mark: input.value
                });
            }
        })
    }

    return {
        SessionId: selectedExamSessionId,
        StudentMarks: studentMarks
    };
}

function EditMassiveGrade() {
    const endpoint = `/api/Exam/EditMassiveGrade`;

    let massiveGradeModel = GetAllMarks();

    fetch(endpoint, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            SessionId: massiveGradeModel.SessionId,
            StudentMarks: massiveGradeModel.StudentMarks
        })
    })
        .then(response => {
            if (response.ok) {
                Alerts.Success('Grades successfully edited');
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