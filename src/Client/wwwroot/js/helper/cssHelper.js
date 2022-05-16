export class CssHelper {
    getCssVariables() {
        return Array.from(document.styleSheets)
            .filter(sheet => sheet.href === null || sheet.href.startsWith(window.location.origin))
            .reduce((acc, sheet) => (acc = [...acc, ...Array.from(sheet.cssRules).reduce((def, rule) => (def = rule.selectorText === ":root"
                ? [...def, ...Array.from(rule.style).filter(name => name.startsWith("--"))]
                : def), [])]), [])
            .map(name => ({ name: name, value: this.getCssVariableValue(name) }));
    }
    findCssVariable(value) {
        value = value.toLowerCase();
        const helper = window[window['___appJsNameSpace']]['ColorHelper'];
        return this.getCssVariables().filter(v => v.value.toLowerCase().includes(value) || helper.ensureHex(v.value).includes(helper.ensureHex(value)));
    }
    getCssVariableValue(varName) {
        return getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
    }
    setCssVariableValue(varName, value) {
        document.documentElement.style.setProperty(varName, value);
    }
}
//# sourceMappingURL=cssHelper.js.map