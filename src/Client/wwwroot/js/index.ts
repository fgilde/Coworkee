import * as helper from './helper/index.js'
// TODO: Rebuild whole file, its Horrable

let jsAppData = {
    mouseArgs: {} as any,
    browserDimensions: {}
}

function loadHelper(namespace:string) {
    var keys = Object.keys(helper);
    window[namespace] = window[namespace] || {};
    for (const key of keys) {
        window[namespace][key] = new helper[key]();
    }
}

function onMouseUpdate (e) {
    jsAppData.mouseArgs = e;
}

function getJsAppData () {
    var res = jsAppData;
    res.browserDimensions = {
        width: window.innerWidth,
        height: window.innerHeight
    };
    res.mouseArgs = (({ pageX, pageY, clientX, clientY, screenX, screenY, movementX, movementY, shiftKey, altKey, metaKey, ctrlKey, button, buttons }:any) => ({ pageX, pageY, clientX, clientY, screenX, screenY, movementX, movementY, shiftKey, altKey, metaKey, ctrlKey, button, buttons }))(jsAppData.mouseArgs);
    //res.mouseArgs = new helper.EventHelper().cloneEvent(jsAppData.mouseArgs, true);
    return res;
};

function onLoaded(appSettings) {
    var nsObject: any = window[appSettings.JsMainNamespace];
    nsObject['AppSettings'] = appSettings;
    nsObject['getJsAppData'] = getJsAppData;
    document.querySelector('#app-logo').insertAdjacentHTML('beforeend', nsObject.CustomIcons.ApplicationMainIcon);
    document.querySelector('#sub-text').innerHTML = `${nsObject.ApplicationConstants.ApplicationName}`;
    document.title = nsObject.ApplicationConstants.ApplicationName + ' - Home';
    loadHelper(appSettings.JsMainNamespace);
    var app = document.getElementById('app');
    app.addEventListener('DOMSubtreeModified', contentChanged, false);

    function contentChanged() {
        app.removeEventListener('DOMSubtreeModified', contentChanged);
        var overlay = document.getElementById('overlay-app-load');
        overlay.classList.add('fade-out');
        setTimeout(() => { overlay.remove(); }, 3000); // Remove element after fadeout
    }
}


function initialLoad() {
    var appSettings: any = {},
        isDebug = window.location.hostname.includes("localhost"); // TODO Find better way

    document.addEventListener('mousemove', onMouseUpdate, false);
    document.addEventListener('mouseenter', onMouseUpdate, false);
    const nav = new helper.BrowserHelper();
    if (nav.urlParams()['safemode']) {
        window.localStorage.clear();
        window.location.href = nav.removeUrlParams(window.location.href, 'safemode');
    }
    var configFetch = () => {
        return fetch('appsettings.json', { method: 'GET', redirect: 'follow' })
            .then(response => response.json())
            .then(json => {
                appSettings = Object.assign({ ...json }, { ...appSettings });
                window['___appJsNameSpace'] = appSettings.JsMainNamespace;
                nav.changeFavIcon(appSettings.BackendOrigin + '/favicon.ico');
                var script = document.createElement('script');
                script.onload = () => {
                    onLoaded(appSettings);
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

initialLoad();