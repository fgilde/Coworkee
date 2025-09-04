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
        const nsObject: any = window[appSettings.jsMainNamespace];
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
    static initialLoad(appSettings): void {                        
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
}
 
window['___helper'] = (name) => {
    return window['___getAppJsMainObject']()[name];
}

window['___appFullyLoaded'] = Initializer.appFullyLoaded;

window['___initialLoad'] = function (appConfig, data) {
    window['___appJsNameSpace'] = appConfig.jsMainNamespace;
    (window as any)[appConfig.jsMainNamespace] = (window as any)[appConfig.jsMainNamespace] || {};
    eval(data);    
    Initializer.initialLoad(appConfig);
}
//Initializer.initialLoad();
