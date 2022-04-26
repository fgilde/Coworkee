export class EventHelper {
    addCustomEventListener(name, dotNetObjectRef) {
        document.addEventListener(name, (event) => {
            var args = this.cloneEvent(event, true);
            dotNetObjectRef.invokeMethodAsync('OnCustomEvent', args);
        });
    }
    addCustomEventListenerWhenNotIn(selector, name, dotNetObjectRef) {
        document.addEventListener(name || 'click', (event) => {
            let elements = selector.map(s => Array.from(document.querySelectorAll(s))).reduce((list, item) => list.concat(item)); // selectMany
            if (elements.every(e => !this.isWithin(event, e))) {
                var args = this.cloneEvent(event, true);
                dotNetObjectRef.invokeMethodAsync('OnCustomEvent', args);
            }
        });
    }
    isWithin(event, element) {
        let rect = element.getBoundingClientRect();
        return (event.clientX > rect.left &&
            event.clientX < rect.right &&
            event.clientY < rect.bottom &&
            event.clientY > rect.top);
    }
    stringifyEvent(e) {
        const obj = {};
        for (let k in e) {
            obj[k] = e[k];
        }
        return JSON.stringify(obj, (k, v) => {
            if (v instanceof Node)
                return 'Node';
            if (v instanceof Window)
                return 'Window';
            return v;
        }, ' ');
    }
    cloneEvent(e, serializable) {
        if (serializable) {
            return JSON.parse(this.stringifyEvent(event));
        }
        if (e === undefined || e === null)
            return undefined;
        function ClonedEvent() { }
        ;
        let clone = new ClonedEvent();
        for (let p in e) {
            let d = Object.getOwnPropertyDescriptor(e, p);
            if (d && (d.get || d.set))
                Object.defineProperty(clone, p, d);
            else
                clone[p] = e[p];
        }
        Object.setPrototypeOf(clone, e);
        return clone;
    }
}
//# sourceMappingURL=eventHelper.js.map