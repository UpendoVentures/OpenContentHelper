export function antiForgeryToken() {
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
            let payload = null;

            if (rawText) {
                try {
                    payload = JSON.parse(rawText);
                } catch {
                    payload = rawText;
                }
            }

            if (!response.ok) {
                throw new Error(normalizeErrorPayload(payload, `HTTP ${response.status}`));
            }

            return payload;
        })
        .then((json) => {
            if (typeof onSuccess === 'function') {
                onSuccess(json);
            }
        })
        .catch((error) => {
            if (typeof onError === 'function') {
                onError(error?.message || 'An unexpected error occurred.');
                return;
            }

            console.error(error);
        });
}

export function getResx(dnnConfig, filename, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/Resx/GetResx`;
    doFetch(dnnConfig, `${base}?filename=${encodeURIComponent(filename)}`, 'GET', null, onSuccess, onError);
}

export function getConfig(dnnConfig, portalId, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/GetConfig`;
    doFetch(dnnConfig, `${base}?portalId=${portalId}`, 'GET', null, onSuccess, onError);
}

export function getFilesByFolder(dnnConfig, portalId, folderId, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/GetFilesByFolder`;
    doFetch(dnnConfig, `${base}?portalId=${portalId}&folderId=${folderId}`, 'GET', null, onSuccess, onError);
}

export function getFileById(dnnConfig, portalId, fileId, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/GetFileById`;
    doFetch(dnnConfig, `${base}?portalId=${portalId}&fileId=${fileId}`, 'GET', null, onSuccess, onError);
}

export function getEventForEdit(dnnConfig, portalId, eventId, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/GetEventForEdit`;
    doFetch(dnnConfig, `${base}?portalId=${portalId}&eventId=${eventId}`, 'GET', null, onSuccess, onError);
}

export function checkSlugAvailability(dnnConfig, portalId, slug, eventId, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/CheckSlugAvailability`;
    const url = `${base}?portalId=${portalId}&slug=${encodeURIComponent(slug)}${eventId ? `&eventId=${eventId}` : ''}`;
    doFetch(dnnConfig, url, 'GET', null, onSuccess, onError);
}

export function saveEvent(dnnConfig, model, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/Save`;
    doFetch(dnnConfig, base, 'POST', model, onSuccess, onError);
}

export function createCategory(dnnConfig, model, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/CreateCategory`;
    doFetch(dnnConfig, base, 'POST', model, onSuccess, onError);
}

export function createTag(dnnConfig, model, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/CreateTag`;
    doFetch(dnnConfig, base, 'POST', model, onSuccess, onError);
}

export function getVenueSuggestions(dnnConfig, portalId, query, limit, onSuccess, onError) {
    const base = `${window.location.origin}/API/Upendo.Modules.UpendoEventsForm/EventAdmin/SearchVenueSuggestions`;
    const url = `${base}?portalId=${portalId}&q=${encodeURIComponent(query)}&limit=${limit || 8}`;
    doFetch(dnnConfig, url, 'GET', null, onSuccess, onError);
}