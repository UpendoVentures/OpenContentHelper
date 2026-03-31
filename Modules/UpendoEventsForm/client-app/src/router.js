import { createRouter, createWebHistory } from 'vue-router';
import EventFormView from './views/EventFormView.vue';

const routes = [
    { path: '/', component: EventFormView },
    { path: '/:pathMatch(.*)*', redirect: '/' }
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;