function antiForgeryToken() {
    const service = window?.$?.ServicesFramework?.();
    return service?.getAntiForgeryValue() || '';
}

function normalizeErrorPayload(payload, fallbackMessage) {
    if (!payload) {
        return fallbackMessage || 'An unexpected error occurred.';
    }

    if (typeof payload === 'string') {
        return payload;
    }

    if (payload.Message) {
        return payload.Message;
    }

    if (payload.message) {
        return payload.message;
    }

    return fallbackMessage || 'An unexpected error occurred.';
}

function doFetch(dnnConfig, url, method, data, onSuccess, onError) {
    const options = {
        method: method || 'GET',
        headers: {
            'Content-Type': 'application/json',
            moduleid: dnnConfig.moduleId,
            tabid: dnnConfig.tabId,
            RequestVerificationToken: antiForgeryToken()
        },
        body: data ? JSON.stringify(data) : null,
        credentials: 'include'
    };

    fetch(new Request(url), options)
        .then(async (response) => {
            const rawText = await response.text();
            let json = null;

            if (rawText) {
                try {
                    json = JSON.parse(rawText);
                }
                catch {
                    json = rawText;
                }
            }

            if (!response.ok) {
                throw new Error(normalizeErrorPayload(json, `HTTP ${response.status}`));
            }

            return json;
        })
        .then((json) => {
            if (typeof onSuccess === 'function') {
                onSuccess(json);
            }
        })
        .catch((error) => {
            if (typeof onError === 'function') {
                onError(error?.message || 'An unexpected error occurred.');
            }
            else {
                console.error(error);
            }
        });
}

export { antiForgeryToken };

export function getResx(dnnConfig, filename, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.BusinessDirectoryManager/Resx/GetResx`;
    doFetch(dnnConfig, `${base}?filename=${encodeURIComponent(filename)}`, 'GET', null, onSuccess, onError);
}

export function getConfig(dnnConfig, portalId, includeCategories, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.BusinessDirectoryManager/BusinessDirectoryAdmin/GetConfig`;
    const url = `${base}?portalId=${portalId}&includeCategories=${includeCategories ? 'true' : 'false'}`;
    doFetch(dnnConfig, url, 'GET', null, onSuccess, onError);
}

export function getBusinessesPage(dnnConfig, request, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.BusinessDirectoryManager/BusinessDirectoryAdmin/GetBusinessesPage`;
    doFetch(dnnConfig, base, 'POST', request, onSuccess, onError);
}

export function getBusinessForEdit(dnnConfig, portalId, companyId, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.BusinessDirectoryManager/BusinessDirectoryAdmin/GetBusinessForEdit`;
    doFetch(dnnConfig, `${base}?portalId=${portalId}&companyId=${companyId}`, 'GET', null, onSuccess, onError);
}

export function checkSlugAvailability(dnnConfig, portalId, slug, companyId, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.BusinessDirectoryManager/BusinessDirectoryAdmin/CheckSlugAvailability`;
    const url = `${base}?portalId=${portalId}&slug=${encodeURIComponent(slug)}${companyId ? `&companyId=${companyId}` : ''}`;
    doFetch(dnnConfig, url, 'GET', null, onSuccess, onError);
}

export function saveBusiness(dnnConfig, model, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.BusinessDirectoryManager/BusinessDirectoryAdmin/Save`;
    doFetch(dnnConfig, base, 'POST', model, onSuccess, onError);
}