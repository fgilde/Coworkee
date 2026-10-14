import { esc, type Coworkee, type CoworkeeApp } from '@coworkee/airworks';
import { odataPage, realtimeTopics, searchFilter } from '@coworkee/client';
import type { AppContext } from '@fgilde/airworks';
import { price, productAppId } from './product';

// OData rows carry the property names of the entity (PascalCase)
type ProductRow = { Id: string; Name: string; Rate: number; ImageDataUrl?: string | null; Brand?: { Name: string } | null };

const loadProducts = (text: string, top: number) => odataPage<ProductRow>('Products', {
  select: ['Id', 'Name', 'Rate', 'ImageDataUrl'],
  expand: 'Brand($select=Name)',
  filter: searchFilter(['Name', 'Brand/Name'], text),
  orderBy: 'Name',
  top,
});

/** The products of the demo as cards with image, brand and price; a click opens the product window. */
export const catalogApp = (coworkee: Coworkee): CoworkeeApp => ({
  id: 'myapp.catalog',
  // coworkee.t looks the English text up in the texts the API serves (library texts and src/MyApp.Contracts/Localization)
  title: coworkee.t('Catalog'),
  icon: 'ratio-product.png',
  accent: '#6a3fb5',
  width: 960,
  height: 640,
  // without the permission AirWorks leaves the app out of the desktop, the start menu and the search
  requiredPermissions: ['Catalog.Products.View'],
  // hits in the desktop search; choosing one opens this app with the hit's intent
  search: async (query) => (await loadProducts(query, 8)).items.map((product) => ({
    title: product.Name, description: product.Brand?.Name ?? '', icon: 'ratio-product.png', intent: { id: product.Id },
  })),
  render: ({ host, intent, onIntent, onClose }: AppContext) => {
    const { t } = coworkee;
    host.classList.add('cw-host');
    host.innerHTML = `<div class="cw-app myapp-catalog">
      <div class="cw-toolbar"><input type="search" placeholder="${esc(t('Search'))}" aria-label="${esc(t('Search'))}"/></div>
      <div class="myapp-cards"></div>
    </div>`;
    const input = host.querySelector('input')!;
    const cards = host.querySelector<HTMLElement>('.myapp-cards')!;

    const card = (product: ProductRow) => `<button class="myapp-card" data-id="${esc(product.Id)}">
      ${product.ImageDataUrl ? `<img src="${esc(product.ImageDataUrl)}" alt=""/>` : `<span class="myapp-placeholder">${esc(product.Name.charAt(0))}</span>`}
      <strong>${esc(product.Name)}</strong><small>${esc(product.Brand?.Name ?? '')}</small><span>${esc(price(product.Rate, coworkee.culture))}</span>
    </button>`;

    const show = async () => {
      try {
        const { items } = await loadProducts(input.value, 60);
        cards.innerHTML = items.map(card).join('') || `<p class="cw-muted">${esc(t('Nothing found.'))}</p>`;
      } catch (error) {
        cards.innerHTML = `<p class="cw-error">${esc(error instanceof Error ? error.message : error)}</p>`;
      }
    };

    // another app opens with an intent: the product window shows the product whose id it gets
    const open = (id?: string) => id && coworkee.airWorks.openApp(productAppId, { id });
    cards.addEventListener('click', (event) => open((event.target as Element).closest<HTMLElement>('[data-id]')?.dataset.id));
    let typing: ReturnType<typeof setTimeout> | undefined;
    input.addEventListener('input', () => { clearTimeout(typing); typing = setTimeout(show, 300); });

    // Product has [Realtime]: changes of products the user may see arrive on type:Product; stop following when the window closes
    onClose(coworkee.realtime.subscribe(realtimeTopics.type('Product'), () => void show()));

    onIntent((next) => open(next.id));
    open(intent?.id);
    void show();
  },
});
