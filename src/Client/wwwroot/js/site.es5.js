'use strict';

window.___getAppJsMainObject = function () {
    return window[window['___appJsNameSpace']];
};

window.___helper = function (name) {
    return window.___getAppJsMainObject()[name];
};

