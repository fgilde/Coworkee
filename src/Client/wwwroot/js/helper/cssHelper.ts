export class CssHelper {

    public getCssVariables(): Array<{ name: string; value: string }> {
        return Array.from(document.styleSheets)
            .filter(sheet => sheet.href === null || sheet.href.startsWith(window.location.origin))
            .reduce((acc, sheet) => (acc = [...acc, ...Array.from(sheet.cssRules).reduce((def, rule) => (def = (rule as CSSStyleRule).selectorText === ":root"
                ? [...def, ...Array.from((rule as CSSStyleRule).style).filter(name => name.startsWith("--"))]
                : def), [])]), [])
            .map(name => ({name: name, value: this.getCssVariableValue(name)}));
    }

    public getCssVariableValue(varName: string) {
        return getComputedStyle(document.documentElement).getPropertyValue(varName).trim();
    }

    public setCssVariableValue(varName: string, value: string) {
        document.documentElement.style.setProperty(varName, value);
    }

}