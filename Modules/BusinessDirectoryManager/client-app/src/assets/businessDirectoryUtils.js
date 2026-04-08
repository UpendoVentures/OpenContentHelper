export function normalizeSlug(value) {
    if (!value) {
        return '';
    }

    return value
        .toString()
        .trim()
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, '-')
        .replace(/-+/g, '-')
        .replace(/^-|-$/g, '');
}

export function nullIfBlank(value) {
    if (value === null || value === undefined) {
        return null;
    }

    const trimmed = value.toString().trim();
    return trimmed ? trimmed : null;
}

export function formatDateTime(value) {
    if (!value) {
        return '';
    }

    const date = new Date(value);
    if (isNaN(date.getTime())) {
        return '';
    }

    return date.toLocaleString([], {
        year: 'numeric',
        month: 'numeric',
        day: 'numeric',
        hour: 'numeric',
        minute: '2-digit',
        second: '2-digit',
        timeZoneName: 'short'
    });
}

export function formatDateForInput(value) {
    if (!value) {
        return '';
    }

    const date = new Date(value);
    if (isNaN(date.getTime())) {
        return '';
    }

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

export function formatCityRegion(item) {
    if (item?.City && item?.Region) {
        return `${item.City}, ${item.Region}`;
    }

    return item?.City || item?.Region || '';
}

export function getPublicCompanyUrl(slug) {
    return `/list/company/${slug}`;
}

export function isValidUrl(value) {
    if (!value) {
        return true;
    }

    try {
        const url = new URL(value);
        return url.protocol === 'http:' || url.protocol === 'https:';
    }
    catch {
        return false;
    }
}

export function isValidEmail(value) {
    if (!value) {
        return true;
    }

    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
}

export function stripHtml(value) {
    if (!value) {
        return '';
    }

    return value
        .replace(/<br\s*\/?>/gi, ' ')
        .replace(/<\/p>/gi, ' ')
        .replace(/<[^>]*>/g, ' ')
        .replace(/\s+/g, ' ')
        .trim();
}

export function isRichTextEmpty(value) {
    return !stripHtml(value);
}

export function isValidYear(value, min, max) {
    if (value === null || value === undefined || value === '') {
        return true;
    }

    const parsed = Number(value);

    if (!Number.isInteger(parsed)) {
        return false;
    }

    return parsed >= min && parsed <= max;
}

export function isValidLatitude(value) {
    if (value === null || value === undefined || value === '') {
        return true;
    }

    const parsed = Number(value);
    return !Number.isNaN(parsed) && parsed >= -90 && parsed <= 90;
}

export function isValidLongitude(value) {
    if (value === null || value === undefined || value === '') {
        return true;
    }

    const parsed = Number(value);
    return !Number.isNaN(parsed) && parsed >= -180 && parsed <= 180;
}