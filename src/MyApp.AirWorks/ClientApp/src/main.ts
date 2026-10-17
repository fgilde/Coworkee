import { adminApps, defaultApps, embedApp, useCoworkee } from '@coworkee/airworks';
import { AirWorks } from '@fgilde/airworks';
import '@fgilde/airworks/style.css';
import '@coworkee/airworks/style.css';
import './styles.css';
import logo from '../../../MyApp.Web.Client/wwwroot/coworkee-icon.svg?url';
import { catalogApp } from './apps/catalog';
import { productApp } from './apps/product';
import { salesDashboardApp } from './apps/sales-dashboard';

// sign-in, theme, logo and texts of the app; AirWorks options (wallpapers, initialShortcuts, ...) go to airWorks: { ... }
const coworkee = await useCoworkee(AirWorks, { title: 'MyApp', logo });

// the library's apps and the demo's own: leave some out, replace them or add more (https://fgilde.github.io/AirWorks/)
await coworkee.register(
  ...defaultApps,
  ...adminApps,
  // hand-written apps of the demo (src/apps): a product catalog with its detail window and a sales dashboard
  catalogApp,
  productApp,
  salesDashboardApp,
  // a page of the web app in a window: the orders with facets, query builder and the browser copy, no rebuild in TypeScript
  embedApp({ id: 'myapp.orders', title: 'Orders', path: '/sales/orders', icon: 'fn-planning.png', accent: '#2e7d32', requiredPermissions: ['SalesOrders.View'] }),
);

document.body.append(document.createElement('air-desktop'));
