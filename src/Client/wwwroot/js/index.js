import * as helper from './helper/index.js';
let jsAppData = {
    mouseArgs: {},
    browserDimensions: {}
};
class HelperLoader {
    static loadHelper(namespace) {
        const keys = Object.keys(helper);
        window[namespace] = window[namespace] || {};
        for (const key of keys) {
            window[namespace][key] = new helper[key]();
        }
    }
}
class MouseTracker {
    static onMouseUpdate(e) {
        jsAppData.mouseArgs = e;
    }
    static getJsAppData() {
        const res = jsAppData;
        res.browserDimensions = {
            width: window.innerWidth,
            height: window.innerHeight
        };
        res.mouseArgs = (({ pageX, pageY, clientX, clientY, screenX, screenY, movementX, movementY, shiftKey, altKey, metaKey, ctrlKey, button, buttons }) => ({ pageX, pageY, clientX, clientY, screenX, screenY, movementX, movementY, shiftKey, altKey, metaKey, ctrlKey, button, buttons }))(jsAppData.mouseArgs);
        return res;
    }
}
class ApplicationLoader {
    static onLoaded(appSettings) {
        const nsObject = window[appSettings.JsMainNamespace];
        nsObject['AppSettings'] = appSettings;
        nsObject['getJsAppData'] = MouseTracker.getJsAppData;
        document.title = `${nsObject.ApplicationConstants.ApplicationName} - Home`;
        HelperLoader.loadHelper(appSettings.JsMainNamespace);
    }
}
class Initializer {
    static appFullyLoaded() {
        document.body.classList.remove('app-loading');
    }
    static initialLoad() {
        let appSettings = {};
        const isDebug = window.location.hostname.includes("localhost"); // TODO Find better way
        document.addEventListener('mousemove', MouseTracker.onMouseUpdate, false);
        document.addEventListener('mouseenter', MouseTracker.onMouseUpdate, false);
        const nav = new helper.BrowserHelper();
        if (nav.urlParams()['safemode']) {
            window.localStorage.clear();
            window.location.href = nav.removeUrlParams(window.location.href, 'safemode');
        }
        const configFetch = () => {
            return fetch('appsettings.json', { method: 'GET', redirect: 'follow' })
                .then(response => response.json())
                .then(json => {
                appSettings = Object.assign({ ...json }, { ...appSettings });
                console.log(appSettings);
                window['___appJsNameSpace'] = appSettings.JsMainNamespace;
                nav.changeFavIcon(appSettings.BackendOrigin + '/favicon.ico');
                var script = document.createElement('script');
                script.onload = () => {
                    ApplicationLoader.onLoaded(appSettings);
                };
                script.src = `${appSettings.BackendOrigin}/${appSettings.JsMainNamespace}/resources.js`;
                document.head.appendChild(script);
            });
        };
        if (isDebug) {
            fetch('appsettings.Development.json', { method: 'GET', redirect: 'follow' })
                .then(response => response.json())
                .then(json => {
                appSettings = json;
                configFetch();
            });
        }
        else {
            configFetch();
        }
    }
}
window['___getAppJsMainObject'] = () => {
    return window[window['___appJsNameSpace']];
};
window['___helper'] = (name) => {
    return window['___getAppJsMainObject']()[name];
};
window['___appFullyLoaded'] = Initializer.appFullyLoaded;
Initializer.initialLoad();
//# sourceMappingURL=index.js.map