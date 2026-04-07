import { createRouter, createWebHashHistory } from 'vue-router';

const BusinessDirectoryListView = () => import('./views/BusinessDirectoryListView.vue');
const BusinessDirectoryEditView = () => import('./views/BusinessDirectoryEditView.vue');

const routes = [
    { path: '/', component: BusinessDirectoryListView },
    { path: '/new', component: BusinessDirectoryEditView },
    { path: '/edit/:companyId', component: BusinessDirectoryEditView, props: true },
    { path: '/:pathMatch(.*)*', redirect: '/' }
];

const router = createRouter({
    history: createWebHashHistory(),
    routes
});

export default router;