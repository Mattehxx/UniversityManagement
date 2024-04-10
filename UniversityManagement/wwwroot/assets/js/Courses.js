/* GLOBAL VARIABLES */
let selectedCourseId = 0;
let selectedExamId = 0;
let allCoursesData = [];
let allProfessorsData = [];
allExamsData = [];
let alsoNotActive = false;

document.addEventListener("DOMContentLoaded", function (event) {
    GetAllCourses();
    GetAllProfessors();
    GetAllExams();
});

function GetAllCourses() {
    const endpoint = '/api/Course/GetAll';

    fetch(endpoint, {
        method: 'GET'
    })
        .then(response => {
            if (response.ok) {
                response.json().then(json => {
                    allCoursesData = json;
                    FillCourseTable(json);
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

function FillCourseTable(data) {
    if (Array.isArray(data)) {
        let tableBody = document.querySelector('#coursesTableBody');
        if (tableBody.innerHTML != '')
            tableBody.innerHTML = '';
        data.forEach(element => {
            if (alsoNotActive || element.isActive) {
                tableBody.innerHTML +=
                    `<tr>
                    <td class="d-none">${element.courseId}</td>
                    <td>${element.title}</td>
                    <td>${GetOnlyYearFromDateTime(element.startYear)}</td>
                    <td>${TranslateType(element.type)}</td>
                    <td>${element.studentNumber}</td>
                    <td>${element.examNumber}</td>
                    <td>
                        <button type="button" class="btn btn-outline-info" id="bntOpenExams_${element.courseId}"
                            data-bs-toggle="modal" data-bs-target="#examsModal" title="Open exams" onclick="selectedCourseId = GetIdFromString(this.id); SearchExam(selectedCourseId);">
                            <i class="fa-solid fa-bookmark"></i>
                        </button>
                        <button type="button" class="btn btn-outline-success" id="bntAddStudent_${element.courseId}"
                            data-bs-toggle="modal" data-bs-target="#addStudentModal" title="Add student" onclick="selectedCourseId = GetIdFromString(this.id);">
                            <i class="fa-solid fa-user-plus"></i>
                        </button>
                        <button type="button" class="btn btn-outline-warning" id="bntEdit_${element.courseId}"
                            data-bs-toggle="modal" data-bs-target="#editCourseModal" title="Edit course" onclick="FillEditCourseModal(this.id);">
                            <i class="fa-solid fa-pencil"></i>
                        </button>
                        <button type="button" class="btn btn-outline-danger" id="btnDelete_${element.courseId}" 
                            data-bs-toggle="modal" data-bs-target="#confirmModal" title="Delete course" onclick="selectedCourseId = GetIdFromString(this.id);">
                            <i class="fa-solid fa-trash"></i>
                        </button>
                    </td>
                </tr>`;
            }
        });
    }
}

async function GetCourseById(id) {
    const endpoint = `/api/Course/GetSingle/${id}`;

    const response = await fetch(endpoint, {
        method: 'GET'
    });

    if (!response.ok)
        return Promise.reject();

    return response.json();
}

function FillEditCourseModal(courseId) {
    selectedCourseId = GetIdFromString(courseId);
    let response = GetCourseById(selectedCourseId);

    response.then(course => {
        document.querySelector('#inpEditTitle').value = course.title;
        document.querySelector('#inpEditDate').value = GetOnlyYearFromDateTime(course.startYear);
        document.querySelector('#swtEditType').checked = course.type;
    })
        .catch(error => {
            console.error(error);
        });
}

function EditCourse() {
    const endpoint = `/api/Course/Edit`;

    let title = document.querySelector('#inpEditTitle').value;
    let startYear = document.querySelector('#inpEditDate').value + '-01-01';
    let type = document.querySelector('#swtEditType').checked;

    fetch(endpoint, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            CourseId: selectedCourseId,
            Title: title,
            StartYear: startYear,
            Type: type
        })
    })
        .then(response => {
            if (response.ok) {
                GetAllCourses();
                Alerts.Success('Course successfully edited');
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
            $('#editCourseModal').modal('hide');
        })

    return false;
}

function DeleteCourse() {
    const endpoint = `/api/Course/Delete/${selectedCourseId}`;

    fetch(endpoint, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                GetAllCourses();
                Alerts.Success('Course successfully deleted');
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

function SearchCourse(title) {
    let filteredData = allCoursesData.filter(element => {
        return element.title.toLowerCase().includes(title.toLowerCase());
    });
    FillCourseTable(filteredData);
}

function AddCourse() {
    const endpoint = `/api/Course/Create`;

    let title = document.querySelector('#inpTitle').value;
    let startYear = document.querySelector('#inpStartYear').value + '-01-01';
    let type = document.querySelector('#swtType').checked;

    fetch(endpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            CourseId: 0,
            Title: title,
            StartYear: startYear,
            Type: type
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllCourses();
            Alerts.Success('Course successfully added');
            document.querySelector('#addCourseForm').reset();
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

function AddStudent() {
    const endpoint = `/api/Course/CreateStudent`;

    let name = document.querySelector('#inpStudentName').value;
    let surname = document.querySelector('#inpStudentSurname').value;
    let birthDate = document.querySelector('#inpStudentBirthDate').value;
    let fc = document.querySelector('#inpStudentFC').value;

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
            CourseId: selectedCourseId
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllCourses();
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
    .finally(() => {
        $('#addStudentModal').modal('hide');
    })

    return false;
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
            document.querySelector('#slcExamProfessor').innerHTML += html;
        });
    }
}

function AddExam() {
    const endpoint = `/api/Course/CreateExam`;

    let title = document.querySelector('#inpExamTitle').value;
    let credits = document.querySelector('#inpExamCredits').value;
    let professorId = document.querySelector('#slcExamProfessor').value;

    fetch(endpoint, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            ExamId: 0,
            Title: title,
            Credits: credits,
            CourseId: selectedCourseId,
            ProfessorId: professorId
        })
    })
    .then(response => {
        if (response.ok) {
            GetAllCourses();
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
    .finally(() => {
        $('#addExamModal').modal('hide');
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
                    <button type="button" class="btn btn-outline-danger" id="btnDelete_${element.examId}" 
                        data-bs-toggle="modal" data-bs-target="#confirmExamModal" title="Delete Exam" onclick="selectedExamId = GetIdFromString(this.id);">
                        <i class="fa-solid fa-trash mr-1"></i>
                    </button>
                </td>
            </tr>`;
        })
    };
}

function DeleteExam() {
    const endpoint = `/api/Exam/Delete/${selectedExamId}`;

    fetch(endpoint, {
        method: 'DELETE'
    })
    .then(response => {
        if (response.ok) {
            GetAllCourses();
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

function SearchExam(id) {
    let filteredData = allExamsData.filter(element => {
        return element.courseId == id;
    });
    FillExamsTable(filteredData);
}

function UploadFile() {
    const endpoint = 'api/Course/CreateMassiveExams';

    const file = document.querySelector('#addMassiveExams').files[0];
    let formData = new FormData();
    formData.append('CourseId', selectedCourseId);
    formData.append('File', file);
    
    fetch(endpoint, {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if(response.ok) {
            GetAllCourses();
            GetAllExams();
            Alerts.Success('Course successfully edited');
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

function DowloadTemplateFile() {
    const endpoint = '/assets/template/exam_template.csv';

    fetch(endpoint, {
        method: 'GET',
    })
    .then(response => {
        if (response.ok) {
            response.blob().then(blob => {
                console.log(blob);
                const link = document.createElement("a");
                link.href = URL.createObjectURL(blob);
                link.download = 'exam_template.csv';
                link.click();
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