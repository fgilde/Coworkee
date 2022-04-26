export class BrowserHelper {
    download(options) {
        var fileUrl = "data:" + options.mimeType + ";base64," + options.base64String;
        fetch(fileUrl)
            .then(response => response.blob())
            .then(blob => {
            var link = window.document.createElement("a");
            //link.href = window.URL.createObjectURL(blob, { type: options.mimeType });
            link.href = window.URL.createObjectURL(blob);
            link.download = options.fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
    }
    setTitle(title) {
        var appName = window[window['___appJsNameSpace']]['ApplicationConstants']['ApplicationName'];
        if (title.includes('-') || title === appName) {
            window.document.title = title;
        }
        else {
            document.title = appName ? appName + ' - ' + title : title;
        }
    }
    isDarkMode() {
        return (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches);
    }
    scrollToBottom(elementName) {
        var element = document.getElementById(elementName);
        element.scrollTop = element.scrollHeight - element.clientHeight;
    }
    playAudio(elementName) {
        let el = document.getElementById(elementName);
        el.play();
    }
    reloadSilent() {
        let unloadScripts = () => {
            var loadedScripts = Array.from(document.querySelectorAll('script'));
            loadedScripts.forEach(script => {
                script.parentNode.removeChild(script);
            });
        };
        unloadScripts();
        return fetch(window.location.href, { method: 'GET', redirect: 'follow' })
            .then(response => response.text())
            .then(html => {
            // Convert the HTML string into a document object
            unloadScripts();
            var parser = new DOMParser();
            var doc = parser.parseFromString(html, 'text/html');
            document.replaceChild(document.importNode(doc.documentElement, true), document.documentElement);
            window['initialLoad']();
        }).catch(err => {
            // There was an error
            console.warn('Something went wrong.', err);
        });
    }
    changeFavIcon(url) {
        var link = document.querySelector("link[rel~='icon']");
        if (!link) {
            link = document.createElement('link');
            link.rel = 'icon';
            document.getElementsByTagName('head')[0].appendChild(link);
        }
        link.href = url;
    }
    changeUrl(url) {
        history.pushState(null, '', url);
    }
    removeUrlParams(url, parameters) {
        ((parameters ? (typeof parameters === 'string' ? [parameters] : parameters) : ['[^#]*'])).forEach((parameter) => {
            url = url
                .replace(new RegExp('(?:&(' + parameter + '=?[^#&]*))'), '')
                .replace(new RegExp('(?:\\?(' + parameter + '=?[^#&]*&))'), '?')
                .replace(new RegExp('(?:\\?(' + parameter + '=?[^#&]*))'), '');
        });
        return url;
    }
    urlParams(url) {
        try {
            var search = url ? url.match(/(?:[^?]*)\??([^#]*)/)[1] : window.location.search.substring(1);
            return JSON.parse('{"' + search.replace(/&/g, '","').replace(/=/g, '":"') + '"}', (key, value) => key === '' ? value : decodeURIComponent(value));
        }
        catch (e) {
            return {};
        }
    }
    navigateToExternalUrl(url) {
        // noreferrer is important that's because otherwise the new window is opened in the same process with the opener window.
        window.open(url, '_blank', 'noreferrer');
    }
}
//# sourceMappingURL=browserHelper.js.map