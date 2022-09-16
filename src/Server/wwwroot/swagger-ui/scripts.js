function darkMode(bool) {
    document.body.style.filter = bool ? 'invert(100%) hue-rotate(180deg)' : null;

    var styles = `
    body {
        filter: invert(100%) hue-rotate(180deg);
    }

    html {
        background-color: #111;
    }

    img, video, iframe {
        filter: invert(100%) hue-rotate(180deg);
    }

    .icon {
        filter: invert(15%) hue-rotate(180deg);
    }

    pre {
        filter: invert(6%);
    }

    li::marker {
        color: #666;
    }

    .swagger-ui .topbar {
        background-color: transparent;
    }

    .swagger-ui .topbar .download-url-wrapper .select-label {
        color: #111111;
    }

    .swagger-ui .dialog-ux .backdrop-ux {
        background: rgba(200,200,200,0.6);
    }
`;

    if (bool) {
        var styleSheet = document.createElement("style");
        styleSheet.innerText = styles;
        styleSheet.id = 'dark-styles';
        document.head.appendChild(styleSheet);
    } else {
        var styleSheet = document.getElementById('dark-styles');
        if (styleSheet) {
            document.head.removeChild(styleSheet);
        }
    }
}

function isDarkModePreferred() {
    return (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches);
}

function _swaggerLoginWithToken(token) {
    var state = {
        JWT: {
            name: 'JWT',
            value: 'Bearer ' + token
        }
    }
    localStorage.setItem('authorized', JSON.stringify(state));
}

function _swaggerLoginWithCurrentUser() {
    if (localStorage.authToken) {
        _swaggerLoginWithToken(JSON.parse(localStorage.authToken));
        return true;
    }
    return false;
}

init = function () {
    _swaggerLoginWithCurrentUser();
    var cb = document.getElementById('checkDarkTheme');
    if (cb && isDarkModePreferred()) {
        cb.checked = true;
        darkMode(true);
    }
}();