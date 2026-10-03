export const priceFormatter = new Intl.NumberFormat('hu-HU', {
    style: 'currency',
    currency: 'HUF',
    maximumFractionDigits: 0,
});

// Rövid pénzösszeg tengelyfeliratnak: 80 000 -> "80 E Ft"
export const compactPriceFormatter = new Intl.NumberFormat('hu-HU', {
    style: 'currency',
    currency: 'HUF',
    notation: 'compact',
    maximumFractionDigits: 1,
});

export const dateTimeFormatter = new Intl.DateTimeFormat('hu-HU', {
    dateStyle: 'medium',
    timeStyle: 'short',
});
