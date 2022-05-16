window.___getAppJsMainObject = () => {
    return window[window['___appJsNameSpace']];
}

window.___helper = (name) => {
    return window.___getAppJsMainObject()[name];
}