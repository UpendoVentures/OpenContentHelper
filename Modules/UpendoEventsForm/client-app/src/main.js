import { createApp } from 'vue';
import App from './App.vue';
import { getResx, antiForgeryToken } from './assets/api';
import router from './router';

function getResxPromise(dnnConfig, resxKey) {
    return new Promise((resolve) => {
        getResx(dnnConfig, resxKey, resolve);
    });
}

document.addEventListener('DOMContentLoaded', function () {
    const allAppElements = document.getElementsByClassName('appModule');

    for (let i = 0; i < allAppElements.length; i++) {
        const thisAppElm = allAppElements[i];

        const dnnConfig = {
            tabId: Number(thisAppElm.getAttribute('data-tabid')),
            moduleId: Number(thisAppElm.getAttribute('data-moduleid')),
            portalId: Number(thisAppElm.getAttribute('data-portalid')),
            editMode: (thisAppElm.getAttribute('data-editmode') || '').toLowerCase() === 'true',
            apiBaseUrl: thisAppElm.getAttribute('data-apibaseurl'),
            rvt: antiForgeryToken()
        };

        getResxPromise(dnnConfig, 'View')
            .then((resx) => {
                const app = createApp(App);
                app.use(router);
                app.provide('dnnConfig', dnnConfig);
                app.provide('resx', resx);
                app.mount(`#${thisAppElm.id}`);
            })
            .catch((error) => {
                console.log(error);
            });
    }
});