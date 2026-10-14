import { esc, mudExFrame, type Coworkee, type CoworkeeApp } from '@coworkee/airworks';
import type { AppContext, Intent } from '@fgilde/airworks';
// the demo's own DTOs, generated from its OpenAPI description (sdk/typescript); type-only, nothing of it is bundled
import type { components } from '../../../../../sdk/typescript/src/schema';

type Product = components['schemas']['ProductDto'];

export const productAppId = 'myapp.product';

// the API takes images up to 2 MB as data URL (AddEditProductValidator); base64 makes a file a third larger
const maxImageBytes = 1.5 * 1024 * 1024;

export const price = (rate: number | string, culture: string) => Number(rate).toLocaleString(culture, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

const readDataUrl = (file: File) => new Promise<string>((resolve, reject) => {
  const reader = new FileReader();
  reader.onload = () => resolve(String(reader.result));
  reader.onerror = () => reject(reader.error);
  reader.readAsDataURL(file);
});

/** One product in a window: image in the MudEx file display, details, and a new image for users who may edit products. */
export const productApp = (coworkee: Coworkee): CoworkeeApp => ({
  id: productAppId,
  title: coworkee.t('Product'),
  icon: 'ratio-product.png',
  accent: '#6a3fb5',
  width: 760,
  height: 520,
  // opened by the catalog and the search with an intent ({ id }), not from the start menu
  hidden: true,
  requiredPermissions: ['Catalog.Products.View'],
  render: ({ host, intent, onIntent, setTitle, setState }: AppContext) => {
    const { t } = coworkee;
    host.classList.add('cw-host');
    host.innerHTML = `<div class="cw-app myapp-product"><p class="cw-muted">${esc(t('MudDataGrid_Loading'))}</p></div>`;
    const root = host.firstElementChild as HTMLElement;

    const render = (item: Product) => {
      const row = (label: string, value: unknown) => value ? `<dt>${esc(t(label))}</dt><dd>${esc(value)}</dd>` : '';
      root.innerHTML = `<div class="myapp-image"></div>
        <div class="myapp-details">
          <h2>${esc(item.name)}</h2>
          <dl>${row('Brand', item.brand?.name)}${row('Barcode', item.barcode)}${row('Rate', price(item.rate, coworkee.culture))}${row('Description', item.description)}</dl>
          ${coworkee.can('Catalog.Products.Edit')
            ? `<label class="myapp-upload"><span>${esc(t('Change image'))}</span><input type="file" accept="image/*" hidden/></label>` : ''}
        </div>`;
      const image = root.querySelector('.myapp-image')!;
      if (item.imageDataUrl) {
        // a MudEx web component in a frame of its own (the frame page lives in wwwroot/mudex); the file display takes data URLs as well
        const contentType = /^data:([^;,]+)/.exec(item.imageDataUrl)?.[1] ?? 'image/png';
        image.append(mudExFrame('mudex-file-display', { url: item.imageDataUrl, 'content-type': contentType, 'file-name': item.name, 'show-file-name': 'false' },
          { frameUrl: coworkee.mudExFrameUrl, dark: coworkee.mode === 'dark', title: item.name }));
      } else {
        image.innerHTML = `<p class="cw-muted">${esc(t('No image'))}</p>`;
      }
      root.querySelector<HTMLInputElement>('input[type=file]')?.addEventListener('change', (event) => {
        const file = (event.target as HTMLInputElement).files?.[0];
        if (file) void upload(item, file);
      });
    };

    const upload = async (item: Product, file: File) => {
      if (file.size > maxImageBytes) return coworkee.notify(t('The image must be smaller than {0}.', '1.5 MB'), 'error');
      try {
        // a write through the BFF: request adds the CSRF header, the API checks Catalog.Products.Edit again
        const body = { name: item.name, barcode: item.barcode, description: item.description, rate: item.rate, brandId: item.brandId, imageDataUrl: await readDataUrl(file) };
        await coworkee.request(`/api/v1/products/${item.id}`, { method: 'PUT', body: JSON.stringify(body) });
        coworkee.notify(t('Image saved'));
        await show({ id: item.id });
      } catch (error) {
        coworkee.notify(error instanceof Error ? error.message : String(error), 'error');
      }
    };

    const show = async (next?: Intent) => {
      if (!next?.id) return;
      let product: Product;
      try {
        product = await coworkee.request<Product>(`/api/v1/products/${encodeURIComponent(next.id)}`);
      } catch (error) {
        root.innerHTML = `<p class="cw-error">${esc(error instanceof Error ? error.message : error)}</p>`;
        return;
      }
      setTitle(product.name);
      // the state survives a reload of the desktop: the window opens again with this intent
      setState({ id: product.id }, product.name);
      render(product);
    };

    onIntent(show);
    void show(intent);
  },
});
