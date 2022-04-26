export class EventHelper {

    public addCustomEventListener(name: string, dotNetObjectRef): void {
        document.addEventListener(name, (event) => {
            var args = this.cloneEvent(event, true);
            dotNetObjectRef.invokeMethodAsync('OnCustomEvent', args);
        });
    }

    public addCustomEventListenerWhenNotIn(selector: string[], name: string, dotNetObjectRef): void {
        document.addEventListener(name || 'click', (event) => {
            let elements = selector.map(s => Array.from(document.querySelectorAll(s))).reduce((list, item) => list.concat(item)); // selectMany
            if (elements.every(e => !this.isWithin(event as MouseEvent, e))) {
                var args = this.cloneEvent(event, true);
                dotNetObjectRef.invokeMethodAsync('OnCustomEvent', args);
            }
        });
    }

    public isWithin(event: MouseEvent, element: Element): boolean {
        let rect = element.getBoundingClientRect();
        return (event.clientX > rect.left &&
            event.clientX < rect.right &&
            event.clientY < rect.bottom &&
            event.clientY > rect.top);
    }

    public stringifyEvent(e: Event) {
        const obj = {};
        for (let k in e) {
            obj[k] = e[k];
        }
        return JSON.stringify(obj, (k, v) => {
            if (v instanceof Node) return 'Node';
            if (v instanceof Window) return 'Window';
            return v;
        }, ' ');
    }

    public cloneEvent(e: Event, serializable: boolean) {
        if (serializable) {
            return JSON.parse(this.stringifyEvent(event));
        }
        if (e === undefined || e === null) return undefined;
        function ClonedEvent() { };
        let clone = new ClonedEvent();
        for (let p in e) {
            let d = Object.getOwnPropertyDescriptor(e, p);
            if (d && (d.get || d.set)) Object.defineProperty(clone, p, d); else clone[p] = e[p];
        }
        Object.setPrototypeOf(clone, e);
        return clone;
    }
    
}