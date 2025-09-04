export class ColorHelper {
    ensureHex(v) {
        try {
            if (!this.isHex(v) && v.toLowerCase().startsWith('rgb')) {
                v = this.rgbaToHex(v);
            }
            return v;
        }
        catch (e) {
            return v;
        }
    }
    isHex(h) {
        if (!h) {
            return false;
        }
        h = h.replace('#', '');
        var a = parseInt(h, 16);
        return (a.toString(16) === h);
    }
    hexToRgbA(hex) {
        try {
            var c;
            if (/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {
                c = hex.substring(1).split('');
                if (c.length == 3) {
                    c = [c[0], c[0], c[1], c[1], c[2], c[2]];
                }
                c = '0x' + c.join('');
                return 'rgba(' + [(c >> 16) & 255, (c >> 8) & 255, c & 255].join(',') + ',1)';
            }
        }
        catch (error) {
            return '';
        }
        return '';
    }
    rgbaToHex(orig) {
        try {
            var a, isPercent, rgb = orig.replace(/\s/g, '').match(/^rgba?\((\d+),(\d+),(\d+),?([^,\s)]+)?/i), alpha = (rgb && rgb[4] || "").trim(), hex = rgb ?
                (rgb[1] | 1 << 8).toString(16).slice(1) +
                    (rgb[2] | 1 << 8).toString(16).slice(1) +
                    (rgb[3] | 1 << 8).toString(16).slice(1) : orig;
            if (alpha !== "") {
                a = alpha;
            }
            else {
                //a = 01;
                a = 0x1;
            }
            a = ((a * 255) | 1 << 8).toString(16).slice(1);
            hex = hex + a;
            return hex;
        }
        catch (error) {
            return '';
        }
    }
    isTransparent(v) {
        return v && (v.toLowerCase() === 'transparent' || v.includes('NaN'));
    }
    isNone(v) {
        return v && (v.toLowerCase() === 'none' || v.includes('ed'));
    }
    isTransparentOrNone(v) {
        return this.isNone(v) || this.isTransparent(v);
    }
}
//# sourceMappingURL=colorHelper.js.map