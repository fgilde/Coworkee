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
        const nsObject = window[appSettings.jsMainNamespace];
        nsObject['AppSettings'] = appSettings;
        nsObject['getJsAppData'] = MouseTracker.getJsAppData;
        document.title = `${nsObject.ApplicationConstants.ApplicationName} - Home`;
        HelperLoader.loadHelper(appSettings.jsMainNamespace);
    }
}
class Initializer {
    static appFullyLoaded() {
        document.body.classList.remove('app-loading');
        document.querySelector('#app').classList.remove('unloaded-app');
    }
    static initialLoad(appSettings) {
        document.addEventListener('mousemove', MouseTracker.onMouseUpdate, false);
        document.addEventListener('mouseenter', MouseTracker.onMouseUpdate, false);
        const nav = new helper.BrowserHelper();
        if (nav.urlParams()['safemode']) {
            window.localStorage.clear();
            window.location.href = nav.removeUrlParams(window.location.href, 'safemode');
        }
        ApplicationLoader.onLoaded(appSettings);
        Initializer.appFullyLoaded();
    }
}
window['___getAppJsMainObject'] = () => {
    return window[window['___appJsNameSpace']];
};
window['___helper'] = (name) => {
    return window['___getAppJsMainObject']()[name];
};
window['___appFullyLoaded'] = Initializer.appFullyLoaded;
window['___initialLoad'] = function (appConfig, data) {
    window['___appJsNameSpace'] = appConfig.jsMainNamespace;
    window[appConfig.jsMainNamespace] = window[appConfig.jsMainNamespace] || {};
    eval(data);
    Initializer.initialLoad(appConfig);
};
//Initializer.initialLoad();
//# sourceMappingURL=index.js.map