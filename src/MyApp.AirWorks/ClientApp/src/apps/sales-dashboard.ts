import { esc, type Coworkee, type CoworkeeApp } from '@coworkee/airworks';
import { realtimeTopics } from '@coworkee/client';
import type { AppContext } from '@fgilde/airworks';

type FacetOption = { label: string; count: number; meta?: { color?: string } | null };
type FacetGroup = { field: string; options: FacetOption[] };
type FacetPage = { '@odata.count'?: number; '@cn.facets'?: FacetGroup[] };

// no rows ($top=0), only the count and the facets SalesOrder declares with [ProvideFacet]: the server counts the 100,000 orders per status and region
const loadFacets = (coworkee: Coworkee) => coworkee.request<FacetPage>('/odata/SalesOrders?$top=0&$count=true',
  { headers: { Prefer: 'odata.include-annotations="cn.facets"' } });

/** Orders per status and region, counted on the server and refreshed when orders change. */
export const salesDashboardApp = (coworkee: Coworkee): CoworkeeApp => ({
  id: 'myapp.sales-dashboard',
  title: coworkee.t('Sales dashboard'),
  icon: 'ratio-graph.png',
  accent: '#1565c0',
  width: 820,
  height: 600,
  requiredPermissions: ['SalesOrders.View'],
  render: ({ host, onClose }: AppContext) => {
    const { t, culture } = coworkee;
    host.classList.add('cw-host');
    host.innerHTML = `<div class="cw-app myapp-sales"><p class="cw-muted">${esc(t('MudDataGrid_Loading'))}</p></div>`;
    const root = host.firstElementChild as HTMLElement;

    const bars = (title: string, options: FacetOption[] = []) => {
      const max = Math.max(1, ...options.map((option) => option.count));
      return `<section><h3>${esc(t(title))}</h3>${[...options].sort((a, b) => b.count - a.count).map((option) => `<div class="myapp-bar">
        <span>${esc(t(option.label))}</span>
        <i style="width:${(option.count / max) * 100}%;${option.meta?.color ? `background:${esc(option.meta.color)}` : ''}"></i>
        <b>${option.count.toLocaleString(culture)}</b></div>`).join('')}</section>`;
    };

    const show = async () => {
      try {
        const page = await loadFacets(coworkee);
        const group = (field: string) => page['@cn.facets']?.find((facet) => facet.field.toLowerCase() === field)?.options;
        root.innerHTML = `<div class="myapp-kpi"><b>${(page['@odata.count'] ?? 0).toLocaleString(culture)}</b><span>${esc(t('Orders'))}</span></div>
          ${bars('Orders per status', group('status'))}${bars('Orders per region', group('region'))}`;
      } catch (error) {
        root.innerHTML = `<p class="cw-error">${esc(error instanceof Error ? error.message : error)}</p>`;
      }
    };

    // SalesOrder has [Realtime]; bursts of changes (imports, the Blazor table's bulk edits) reload once
    let pending: ReturnType<typeof setTimeout> | undefined;
    onClose(coworkee.realtime.subscribe(realtimeTopics.type('SalesOrder'), () => { clearTimeout(pending); pending = setTimeout(show, 1000); }));
    onClose(() => clearTimeout(pending));
    void show();
  },
});
