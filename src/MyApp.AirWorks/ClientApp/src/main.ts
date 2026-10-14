import { adminApps, defaultApps, entityApp, useCoworkee } from '@coworkee/airworks';
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
  // the one-line way: an entity set as a searchable table
  entityApp({ entitySet: 'Brands', liveType: 'Brand', icon: 'ratio-shoppingCart.png', requiredPermissions: ['Catalog.Brands.View'] }),
);

document.body.append(document.createElement('air-desktop'));
