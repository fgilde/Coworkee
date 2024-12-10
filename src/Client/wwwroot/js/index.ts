import * as helper from './helper/index.js';

interface MouseArgs {
    pageX: number;
    pageY: number;
    clientX: number;
    clientY: number;
    screenX: number;
    screenY: number;
    movementX: number;
    movementY: number;
    shiftKey: boolean;
    altKey: boolean;
    metaKey: boolean;
    ctrlKey: boolean;
    button: number;
    buttons: number;
}

interface BrowserDimensions {
    width: number;
    height: number;
}

interface JsAppData {
    mouseArgs: MouseArgs;
    browserDimensions: BrowserDimensions;
}

let jsAppData: JsAppData = {
    mouseArgs: {} as MouseArgs,
    browserDimensions: {} as BrowserDimensions
};

class HelperLoader {
    static loadHelper(namespace: string) {
        const keys = Object.keys(helper);
        window[namespace] = window[namespace] || {};
        for (const key of keys) {
            window[namespace][key] = new helper[key]();
        }
    }
}

class MouseTracker {
    static onMouseUpdate(e: MouseEvent): void {
        jsAppData.mouseArgs = e;
    }

    static getJsAppData(): JsAppData {        
        const res: JsAppData = jsAppData;
        res.browserDimensions = {
            width: window.innerWidth,
            height: window.innerHeight
        };                
        res.mouseArgs = (({ pageX, pageY, clientX, clientY, screenX, screenY, movementX, movementY, shiftKey, altKey, metaKey, ctrlKey, button, buttons }: any) => ({ pageX, pageY, clientX, clientY, screenX, screenY, movementX, movementY, shiftKey, altKey, metaKey, ctrlKey, button, buttons }))(jsAppData.mouseArgs);
        return res;
    }
}

class ApplicationLoader {
    static onLoaded(appSettings: any): void {
        const nsObject: any = window[appSettings.JsMainNamespace];
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
    static initialLoad(): void {
        let appSettings: any = {};
        const isDebug = window.location.hostname.includes("localhost");  // TODO Find better way

        document.addEventListener('mousemove', MouseTracker.onMouseUpdate, false);
        document.addEventListener('mouseenter', MouseTracker.onMouseUpdate, false);
        const nav = new helper.BrowserHelper();

        if (nav.urlParams()['safemode']) {
            window.localStorage.clear();
            window.location.href = nav.removeUrlParams(window.location.href, 'safemode');
        }

        const configFetch = (): Promise<void> => {
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
        } else {
            configFetch();
        }
    }
}


window['___getAppJsMainObject'] = () => {
    return window[window['___appJsNameSpace']];
}
 
window['___helper'] = (name) => {
    return window['___getAppJsMainObject']()[name];
}

window['___appFullyLoaded'] = Initializer.appFullyLoaded;


Initializer.initialLoad();
