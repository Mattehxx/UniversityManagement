class Alerts {
    static Success(message) {
        $.notify(message, {
            className: 'success',
            autoHide: true,
            autoHideDelay: 3000,
            style: 'bootstrap',
            globalPosition: 'top right'
        });
    }

    static Info(message) {
        $.notify(message, {
            className: 'info',
            autoHide: true,
            autoHideDelay: 3000,
            style: 'bootstrap',
            globalPosition: 'top right'
        });
    }

    static Warning(message) {
        $.notify(message, {
            className: 'warn',
            autoHide: true,
            autoHideDelay: 3000,
            style: 'bootstrap',
            globalPosition: 'top right'
        });
    }

    static Error(message) {
        $.notify(message, {
            className: 'error',
            autoHide: true,
            autoHideDelay: 3000,
            style: 'bootstrap',
            globalPosition: 'top right'
        });
    }
}

function MountHtmlPage(url, elementId) {
    fetch(url, {
        method: 'GET'
    })
    .then(response => {
        response.text().then(html => {
            document.querySelector(`#${elementId}`).innerHTML = html;
        })
    })
    .catch(error => {
        console.error(error);
    })
}

/**
 * Function that returns the id separated from the name by '_'
 * @param {string} string 
 * @returns {string}
 */
function GetIdFromString(string) {
    return string.split('_')[1];
}

/**
 * Function that returns the course type string
 * @param {boolean} type 
 * @returns 
 */
function TranslateType(type) {
    return type ? 'Master' : 'Bachelor';
}

/**
 * Function that returns the year of the given string date
 * @param {string} date 
 * @returns {string}
 */
function GetOnlyYearFromDateTime(date) {
    let dateObject = new Date(date);
    return dateObject.getFullYear();
}

/**
 * Function that returns the date of the given string date time
 * @param {string} date 
 * @returns {string}
 */
function GetDateFromDateTime(date) {
    return date.split('T')[0];
}

function FormatDateTime(date) {
    let splitted = date.split('T');
    return `${splitted[0]} ${splitted[1].substring(0, 5)}`;
}

function ToggleActiveButtonSidebar(navLinkId) {
    let navLink = document.querySelector(`#${navLinkId}`);
    navLink.classList.add('active');
}

function OnElementAvailable(elementId, callback) {
    const observer = new MutationObserver(mutations => {
        if (document.querySelector(`#${elementId}`)) {
            observer.disconnect();
            callback();
        }
    });
  
    observer.observe(document.body, { childList: true, subtree: true });
}

function TranslateMark(mark) {
    return mark == null ? 'Mark not present' : mark;
}

function GetSelectedOptionsFromMultiSelect(options) {
    return Array.from(options).filter((option) => {
        return option.selected;
    }).map((option) => {
        return parseInt(option.value);
    });
}