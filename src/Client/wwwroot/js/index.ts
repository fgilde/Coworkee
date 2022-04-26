import * as helper from './helper/index.js'

let jsAppData = {
    mouseArgs: {} as Event,
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
    //res.mouseArgs = (({ pageX, pageY, clientX, clientY, screenX, screenY, movementX, movementY, shiftKey, altKey, metaKey, ctrlKey, button, buttons }:any) => ({ pageX, pageY, clientX, clientY, screenX, screenY, movementX, movementY, shiftKey, altKey, metaKey, ctrlKey, button, buttons }))(jsAppData.mouseArgs);
    res.mouseArgs = new helper.EventHelper().cloneEvent(jsAppData.mouseArgs, true);
    return res;
};

function initialLoad() {
    document.addEventListener('mousemove', onMouseUpdate, false);
    document.addEventListener('mouseenter', onMouseUpdate, false);
    const nav = new helper.BrowserHelper();
    if (nav.urlParams()['safemode']) {
        window.localStorage.clear();
        window.location.href = nav.removeUrlParams(window.location.href, 'safemode');
    }
    var loaded = (appSettings) => {
        var nsObject: any = window[appSettings.JsMainNamespace];
        nsObject['AppSettings'] = appSettings;
        nsObject['getJsAppData'] = getJsAppData;
        document.querySelector('#app-logo').insertAdjacentHTML('beforeend', nsObject.CustomIcons.ApplicationMainIcon);
        document.querySelector('#sub-text').innerHTML = `${nsObject.ApplicationConstants.ApplicationName} ${nsObject.ApplicationConstants.Version}`;
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
    };
    fetch('appsettings.json', { method: 'GET', redirect: 'follow' })
        .then(response => response.json())
        .then(json => {
            window['___appJsNameSpace'] = json.JsMainNamespace;
            // TODO: BackendOrigin maybe wrong. not using appsettings.development.json here currently
            nav.changeFavIcon(json.BackendOrigin + '/favicon.ico');
            var script = document.createElement('script');
            script.onload = () => {
                loaded(json);
            };
            script.src = `${json.BackendOrigin}/${json.JsMainNamespace}/resources.js`;

            document.head.appendChild(script);
        });
}

initialLoad();