// A felület szövegei nyelvenként. A magyar a forrás: a kulcsokat ez határozza meg, az angolnak
// mindegyiket meg kell adnia (különben fordítási hiba). Paraméter: {név}, pl. t('filter.activeCount', { count: 2 }).
export const hu = {
  // Fejléc
  'header.logoAlt': 'Alkalmazás logó',
  'header.noRole': 'Nincs szerepkör',
  'header.logout': 'Kijelentkezés',
  'header.themeToLight': 'Váltás világos témára',
  'header.themeToDark': 'Váltás sötét témára',
  'header.language': 'Nyelv',

  // Oldalsáv (util/pages.ts)
  'nav.section.dashboard': 'Műszerfal',
  'nav.section.warehouse': 'Raktár',
  'nav.section.sales': 'Eladások',
  'nav.section.purchasing': 'Beszerzés',
  'nav.section.masterData': 'Törzsadatok',
  'nav.section.admin': 'Adminisztráció',
  'nav.page.dashboard': 'Fő Műszerfal & Riportok',
  'nav.page.products': 'Termékek',
  'nav.page.warehouses': 'Raktárak',
  'nav.page.customerOrders': 'Vevői rendelések',
  'nav.page.supplierOrders': 'Beszerzési rendelések',
  'nav.page.customers': 'Vevők',
  'nav.page.users': 'Felhasználók & jogosultságok',

  // Oldalakon belüli műveletek (util/pages.ts pageActions)
  'action.products.create': 'Új termék',
  'action.products.edit': 'Termék módosítása',
  'action.products.delete': 'Termék törlése',
  'action.customerOrders.create': 'Új rendelés',
  'action.customerOrders.ship': 'Kiszállítás',
  'action.customerOrders.deliver': 'Kézbesítettnek jelölés',
  'action.customerOrders.cancel': 'Lemondás',
  'action.supplierOrders.create': 'Új beszerzési rendelés',
  'action.supplierOrders.receive': 'Beérkeztetés',
  'action.customers.create': 'Új vevő',

  // Bejelentkezés és jogosultság
  'login.userName': 'Felhasználónév',
  'login.password': 'Jelszó',
  'login.submit': 'Bejelentkezés',
  'login.submitting': 'Bejelentkezés...',
  'login.failed': 'Nem sikerült a bejelentkezés.',
  'auth.noAccess': 'Nincs jogosultságod ennek az oldalnak a megtekintéséhez.',
  'auth.noPages': 'A szerepköreidhez még nem tartozik egyetlen oldal sem. Fordulj az adminisztrátorhoz.',

  // Közös komponensek
  'common.cancel': 'Mégse',
  'common.saving': 'Mentés...',
  'common.inProgress': 'Folyamatban...',
  'common.genericError': 'Hiba történt a művelet során.',
  'common.edit': 'Módosítás',
  'common.delete': 'Törlés',
  'common.editItem': '{item} módosítása',
  'common.deleteItem': '{item} törlése',
  'filter.filters': 'Szűrők',
  'filter.activeCount': 'Szűrők ({count} aktív)',
  'filter.clear': 'Szűrők törlése',
  'filter.rangeFrom': 'tól',
  'filter.rangeTo': 'ig',
  'filter.rangeMin': '{label} – legalább',
  'filter.rangeMax': '{label} – legfeljebb',
};

export type TranslationKey = keyof typeof hu;

export const en: Record<TranslationKey, string> = {
  'header.logoAlt': 'App logo',
  'header.noRole': 'No role',
  'header.logout': 'Sign out',
  'header.themeToLight': 'Switch to light theme',
  'header.themeToDark': 'Switch to dark theme',
  'header.language': 'Language',

  'nav.section.dashboard': 'Dashboard',
  'nav.section.warehouse': 'Warehouse',
  'nav.section.sales': 'Sales',
  'nav.section.purchasing': 'Purchasing',
  'nav.section.masterData': 'Master data',
  'nav.section.admin': 'Administration',
  'nav.page.dashboard': 'Main dashboard & reports',
  'nav.page.products': 'Products',
  'nav.page.warehouses': 'Warehouses',
  'nav.page.customerOrders': 'Customer orders',
  'nav.page.supplierOrders': 'Purchase orders',
  'nav.page.customers': 'Customers',
  'nav.page.users': 'Users & permissions',

  'action.products.create': 'Create product',
  'action.products.edit': 'Edit product',
  'action.products.delete': 'Delete product',
  'action.customerOrders.create': 'Create order',
  'action.customerOrders.ship': 'Ship',
  'action.customerOrders.deliver': 'Mark as delivered',
  'action.customerOrders.cancel': 'Cancel',
  'action.supplierOrders.create': 'Create purchase order',
  'action.supplierOrders.receive': 'Receive',
  'action.customers.create': 'Create customer',

  'login.userName': 'Username',
  'login.password': 'Password',
  'login.submit': 'Sign in',
  'login.submitting': 'Signing in...',
  'login.failed': 'Sign-in failed.',
  'auth.noAccess': 'You do not have permission to view this page.',
  'auth.noPages': 'No pages are assigned to your roles yet. Please contact an administrator.',

  'common.cancel': 'Cancel',
  'common.saving': 'Saving...',
  'common.inProgress': 'In progress...',
  'common.genericError': 'An error occurred during the operation.',
  'common.edit': 'Edit',
  'common.delete': 'Delete',
  'common.editItem': 'Edit {item}',
  'common.deleteItem': 'Delete {item}',
  'filter.filters': 'Filters',
  'filter.activeCount': 'Filters ({count} active)',
  'filter.clear': 'Clear filters',
  'filter.rangeFrom': 'from',
  'filter.rangeTo': 'to',
  'filter.rangeMin': '{label} – at least',
  'filter.rangeMax': '{label} – at most',
};

export type Language = 'hu' | 'en';

export const dictionaries: Record<Language, Record<TranslationKey, string>> = { hu, en };

// A nyelvválasztó menüje (a nevek a saját nyelvükön)
export const languageNames: Record<Language, string> = {
  hu: 'Magyar',
  en: 'English',
};
