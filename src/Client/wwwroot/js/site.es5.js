"use strict";

window.Download = function (options) {
    var fileUrl = "data:" + options.mimeType + ";base64," + options.base64String;
    fetch(fileUrl).then(function (response) {
        return response.blob();
    }).then(function (blob) {
        var link = window.document.createElement("a");
        link.href = window.URL.createObjectURL(blob, { type: options.mimeType });
        link.download = options.fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    });
};

window.ChangeFavIcon = function (url) {
    var link = document.querySelector("link[rel~='icon']");
    if (!link) {
        link = document.createElement('link');
        link.rel = 'icon';
        document.getElementsByTagName('head')[0].appendChild(link);
    }
    link.href = url;
};

window.initialLoad = function () {
    if (urlParams()['safemode']) {
        window.localStorage.clear();
        window.location.href = removeUrlParams(window.location.href, 'safemode');
    }

    fetch('appsettings.json', { method: 'GET', redirect: 'follow' }).then(function (response) {
        return response.json();
    }).then(function (json) {
        ChangeFavIcon(json.BackendOrigin + '/favicon.ico');
        var script = document.createElement('script');
        script.onload = function () {
            document.querySelector('#app-logo').insertAdjacentHTML('beforeend', Application.CustomIcons.ApplicationMainIcon);
            document.querySelector('#sub-text').innerHTML = Application.ApplicationConstants.ApplicationName + " " + Application.ApplicationConstants.Version;
            document.title = Application.ApplicationConstants.ApplicationName + ' - Home';
            var app = document.getElementById('app');
            app.addEventListener('DOMSubtreeModified', contentChanged, false);

            function contentChanged() {
                app.removeEventListener('DOMSubtreeModified', contentChanged);
                var overlay = document.getElementById('overlay-app-load');
                overlay.classList.add('fade-out');
                setTimeout(function () {
                    overlay.remove();
                }, 3000); // Remove element after fadeout
            }
        };
        script.src = json.BackendOrigin + '/Application/resources.js';

        document.head.appendChild(script);
    });
};

window.reloadSilent = function () {
    var unloadScripts = function unloadScripts(fileNames) {
        var loadedScripts = Array.from(document.querySelectorAll('script'));
        loadedScripts.forEach(function (script) {
            script.parentNode.removeChild(script);
        });
    };
    unloadScripts();
    fetch(window.location.href, { method: 'GET', redirect: 'follow' }).then(function (response) {
        return response.text();
    }).then(function (html) {
        // Convert the HTML string into a document object
        unloadScripts();
        var parser = new DOMParser();
        var doc = parser.parseFromString(html, 'text/html');
        document.replaceChild(document.importNode(doc.documentElement, true), document.documentElement);
        initialLoad();
    })["catch"](function (err) {
        // There was an error
        console.warn('Something went wrong.', err);
    });
};

window.ChangeUrl = function (url) {
    history.pushState(null, '', url);
};

window.removeUrlParams = function (url, parameters) {
    (parameters ? typeof parameters === 'string' ? [parameters] : parameters : ['[^#]*']).forEach(function (parameter) {
        url = url.replace(new RegExp('(?:&(' + parameter + '=?[^#&]*))'), '').replace(new RegExp('(?:\\?(' + parameter + '=?[^#&]*&))'), '?').replace(new RegExp('(?:\\?(' + parameter + '=?[^#&]*))'), '');
    });
    return url;
};

window.urlParams = function (url) {
    try {
        var search = url ? url.match(/(?:[^?]*)\??([^#]*)/)[1] : window.location.search.substring(1);
        return JSON.parse('{"' + search.replace(/&/g, '","').replace(/=/g, '":"') + '"}', function (key, value) {
            return key === '' ? value : decodeURIComponent(value);
        });
    } catch (e) {
        return {};
    }
};

window.ScrollToBottom = function (elementName) {
    var element = document.getElementById(elementName);
    element.scrollTop = element.scrollHeight - element.clientHeight;
};

window.SetTitle = function (title) {
    var appName = Application.ApplicationConstants.ApplicationName;
    if (title.includes('-') || title === appName) {
        window.document.title = title;
    } else {
        document.title = appName ? appName + ' - ' + title : title;
    }
};

window.PlayAudio = function (elementName) {
    document.getElementById(elementName).play();
};

