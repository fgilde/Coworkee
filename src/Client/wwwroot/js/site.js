window.Download = (options) => {
    var fileUrl = "data:" + options.mimeType + ";base64," + options.byteArray;
    fetch(fileUrl)
        .then(response => response.blob())
        .then(blob => {
            var link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, { type: options.mimeType });
            link.download = options.fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
}

window.ScrollToBottom = (elementName) => {
    var element = document.getElementById(elementName);
    element.scrollTop = element.scrollHeight - element.clientHeight;
}

window.SetTitle = (title) => {
    var appName = Application.ApplicationConstants.ApplicationName;
    if (title.includes('-') || title === appName) {
        window.document.title = title;
    } else {
        document.title = appName ? appName + ' - ' + title : title;
    }
}

window.PlayAudio = (elementName) => {
    document.getElementById(elementName).play();
}