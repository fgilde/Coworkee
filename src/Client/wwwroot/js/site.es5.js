"use strict";

window.Download = function (options) {
    var fileUrl = "data:" + options.mimeType + ";base64," + options.byteArray;
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

window.ScrollToBottom = function (elementName) {
    var element = document.getElementById(elementName);
    element.scrollTop = element.scrollHeight - element.clientHeight;
};

window.PlayAudio = function (elementName) {
    document.getElementById(elementName).play();
};

