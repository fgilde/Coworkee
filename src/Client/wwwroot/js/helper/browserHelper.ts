export class BrowserHelper {

    public download(options) {
        var fileUrl = options.url || "data:" + options.mimeType + ";base64," + options.base64String;
        fetch(fileUrl)
            .then(response => response.blob())
            .then(blob => {
                var link = window.document.createElement("a");
                //link.href = window.URL.createObjectURL(blob, { type: options.mimeType });
                link.href = window.URL.createObjectURL(blob);
                link.download = options?.fileName;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            });
    }

    public setTitle(title: string) {
        var appName = window[window['___appJsNameSpace']]['ApplicationConstants']['ApplicationName'];
        if (title.includes('-') || title === appName) {
            window.document.title = title;
        } else {
            document.title = appName ? appName + ' - ' + title : title;
        }
    }

    public isDarkMode(): boolean {
        return (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches);
    }

    public scrollToBottom(elementName: string) {
        var element = document.getElementById(elementName);
        element.scrollTop = element.scrollHeight - element.clientHeight;
    }

    public playAudio(elementName: string) {
        let el = document.getElementById(elementName) as HTMLAudioElement;
        el.play();
    }

    public reloadSilent(): Promise<void> {
        let unloadScripts = () => {
            var loadedScripts = Array.from(document.querySelectorAll('script'));
            loadedScripts.forEach(script => {
                script.parentNode.removeChild(script);
            });
        }
        unloadScripts();
        return fetch(window.location.href, { method: 'GET', redirect: 'follow' })
            .then(response => response.text())
            .then(html => {
                // Convert the HTML string into a document object
                unloadScripts();
                var parser = new DOMParser();
                var doc = parser.parseFromString(html, 'text/html');
                document.replaceChild(
                    document.importNode(doc.documentElement, true),
                    document.documentElement
                );
                window['initialLoad']();
            }
            ).catch(err => {
                // There was an error
                console.warn('Something went wrong.', err);
            });
    }

    public changeFavIcon(url: string) {
        var link: HTMLLinkElement = document.querySelector("link[rel~='icon']");
        if (!link) {
            link = document.createElement('link');
            link.rel = 'icon';
            document.getElementsByTagName('head')[0].appendChild(link);
        }
        link.href = url;
    }

    public changeUrl(url: string): void {
        history.pushState(null, '', url);
    }

    public removeUrlParams(url, parameters): string {
        ((parameters ? (typeof parameters === 'string' ? [parameters] : parameters) : ['[^#]*'])).forEach(
            (parameter) => {
                url = url
                    .replace(new RegExp('(?:&(' + parameter + '=?[^#&]*))'), '')
                    .replace(new RegExp('(?:\\?(' + parameter + '=?[^#&]*&))'), '?')
                    .replace(new RegExp('(?:\\?(' + parameter + '=?[^#&]*))'), '');
            }
        );
        return url;
    }

    public urlParams(url?): unknown {
        try {
            var search = url ? url.match(/(?:[^?]*)\??([^#]*)/)[1] : window.location.search.substring(1);
            return JSON.parse('{"' + search.replace(/&/g, '","').replace(/=/g, '":"') + '"}',
                (key, value) =>
                    key === '' ? value : decodeURIComponent(value)
            );
        } catch (e) {
            return {};
        }
    }

    public navigateToExternalUrl(url: string): void {
        // noreferrer is important that's because otherwise the new window is opened in the same process with the opener window.
        window.open(url, '_blank', 'noreferrer');
    }

    /**
    * Resolves a promise filled with the value from given expression after the expression function returns a value.
    * @param expression The expression to test
    */
    public when<T>(expression: () => T, timeout?: number): Promise<T> {
        return new Promise((resolve) => {
            const taskId = setInterval(() => {
                const result = expression();
                if (!!result) {
                    clearInterval(taskId);
                    resolve(result);
                }
            }, timeout || 50);
        });
    }

    public clickOnElement(selector: string) {
        (document.querySelector(selector) as HTMLElement)?.click();
    }

}