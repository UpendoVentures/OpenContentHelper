function getStorageKey(dnnConfig) {
    const portalId = Number(dnnConfig?.portalId || 0);
    const tabId = Number(dnnConfig?.tabId || 0);
    const moduleId = Number(dnnConfig?.moduleId || 0);

    return `upendo-bdm-list-state:${portalId}:${tabId}:${moduleId}`;
}

function toSafeInt(value, fallback) {
    const parsed = Number(value);

    if (!Number.isFinite(parsed)) {
        return fallback;
    }

    return parsed;
}

function normalizeState(rawState, dnnConfig) {
    const pageSizeOptions = [10, 20, 40];
    const fallbackPortalId = Number(dnnConfig?.portalId || 0);

    const normalized = {
        selectedPortalId: toSafeInt(rawState?.selectedPortalId, fallbackPortalId),
        searchText: typeof rawState?.searchText === 'string' ? rawState.searchText : '',
        statusFilter: typeof rawState?.statusFilter === 'string' ? rawState.statusFilter : '',
        visibilityFilter: typeof rawState?.visibilityFilter === 'string' ? rawState.visibilityFilter : '',
        featuredOnly: !!rawState?.featuredOnly,
        currentPage: Math.max(1, toSafeInt(rawState?.currentPage, 1)),
        pageSize: toSafeInt(rawState?.pageSize, 10)
    };

    if (!pageSizeOptions.includes(normalized.pageSize)) {
        normalized.pageSize = 10;
    }

    if (
        normalized.statusFilter !== '' &&
        normalized.statusFilter !== 'active' &&
        normalized.statusFilter !== 'inactive'
    ) {
        normalized.statusFilter = '';
    }

    if (
        normalized.visibilityFilter !== '' &&
        normalized.visibilityFilter !== 'public' &&
        normalized.visibilityFilter !== 'private'
    ) {
        normalized.visibilityFilter = '';
    }

    return normalized;
}

export function loadBusinessDirectoryListState(dnnConfig) {
    try {
        const key = getStorageKey(dnnConfig);
        const raw = window.sessionStorage.getItem(key);

        if (!raw) {
            return null;
        }

        const parsed = JSON.parse(raw);
        return normalizeState(parsed, dnnConfig);
    }
    catch {
        return null;
    }
}

export function saveBusinessDirectoryListState(dnnConfig, state) {
    try {
        const key = getStorageKey(dnnConfig);
        const normalized = normalizeState(state, dnnConfig);

        window.sessionStorage.setItem(key, JSON.stringify(normalized));
    }
    catch {
        // Intentionally swallow storage errors.
    }
}

export function clearBusinessDirectoryListState(dnnConfig) {
    try {
        const key = getStorageKey(dnnConfig);
        window.sessionStorage.removeItem(key);
    }
    catch {
        // Intentionally swallow storage errors.
    }
}