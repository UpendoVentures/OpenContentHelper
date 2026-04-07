<template>
    <div class="bdm-page container-fluid py-3">
        <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
            <div>
                <h1 class="h3 mb-1">Business Directory Manager</h1>
                <p class="text-muted mb-0">Manage business listings for this website.</p>
            </div>

            <div class="d-flex gap-2 align-items-center flex-wrap">
                <div v-if="config.IsSuperUser" class="d-flex align-items-center gap-2">
                    <label class="form-label mb-0" for="portalSelect">Portal</label>
                    <select id="portalSelect"
                            class="form-select"
                            style="min-width: 260px;"
                            v-model.number="selectedPortalId"
                            @change="onPortalChanged">
                        <option v-for="portal in config.Portals"
                                :key="portal.PortalId"
                                :value="portal.PortalId">
                            {{ portal.PortalName }}
                        </option>
                    </select>
                </div>

                <router-link class="btn btn-primary" to="/new">
                    Add Business
                </router-link>
            </div>
        </div>

        <div v-if="errorMessage" class="alert alert-danger" role="alert">
            {{ errorMessage }}
        </div>

        <div class="card shadow-sm mb-4 bdm-filter-card">
            <div class="card-body">
                <div class="mb-3">
                    <h2 class="h5 mb-1 bdm-section-title">Search and Filters</h2>
                    <p class="text-muted small mb-0">
                        Narrow the list by text, status, visibility, and featured state.
                    </p>
                </div>

                <div class="row g-3 mb-3">
                    <div class="col-12">
                        <label class="form-label" for="searchText">Search</label>
                        <input id="searchText" autocomplete="off" 
                               v-model.trim="searchText" class="form-control" type="text" 
                               placeholder="Search company, slug, category, city, phone, or website" />
                    </div>
                </div>

                <div class="row g-3 align-items-end">
                    <div class="col-12 col-md-6 col-xl-3">
                        <label class="form-label" for="filterStatus">Status</label>
                        <select id="filterStatus" v-model="statusFilter" class="form-select">
                            <option value="">All</option>
                            <option value="active">Active only</option>
                            <option value="inactive">Inactive only</option>
                        </select>
                    </div>

                    <div class="col-12 col-md-6 col-xl-3">
                        <label class="form-label" for="filterVisibility">Visibility</label>
                        <select id="filterVisibility" v-model="visibilityFilter" class="form-select">
                            <option value="">All</option>
                            <option value="public">Public only</option>
                            <option value="private">Private only</option>
                        </select>
                    </div>

                    <div class="col-6 col-md-4 col-xl-2">
                        <label class="form-label" for="pageSize">Per Page</label>
                        <select id="pageSize" v-model.number="pageSize" class="form-select">
                            <option v-for="size in pageSizeOptions" :key="size" :value="size">
                                {{ size }}
                            </option>
                        </select>
                    </div>

                    <div class="col-6 col-md-4 col-xl-2">
                        <div class="form-check bdm-featured-check">
                            <input id="featuredOnly"
                                   class="form-check-input"
                                   type="checkbox"
                                   v-model="featuredOnly" />
                            <label class="form-check-label" for="featuredOnly">Featured only</label>
                        </div>
                    </div>

                    <div class="col-12 col-md-4 col-xl-2">
                        <button type="button"
                                class="btn btn-outline-secondary w-100"
                                @click="resetFilters">
                            Reset Filters
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-3 mb-4">
            <div class="col-6 col-xl-3">
                <div class="card shadow-sm bdm-stat-card bdm-stat-card-total h-100">
                    <div class="card-body">
                        <div class="bdm-stat-label">Total Businesses</div>
                        <div class="bdm-stat-value">{{ totalItems }}</div>
                        <div class="bdm-stat-subtext">All results after current filters</div>
                    </div>
                </div>
            </div>

            <div class="col-6 col-xl-3">
                <div class="card shadow-sm bdm-stat-card bdm-stat-card-active h-100">
                    <div class="card-body">
                        <div class="bdm-stat-label">Active</div>
                        <div class="bdm-stat-value">{{ activeCount }}</div>
                        <div class="bdm-stat-subtext">Listings currently enabled</div>
                    </div>
                </div>
            </div>

            <div class="col-6 col-xl-3">
                <div class="card shadow-sm bdm-stat-card bdm-stat-card-public h-100">
                    <div class="card-body">
                        <div class="bdm-stat-label">Public</div>
                        <div class="bdm-stat-value">{{ publicCount }}</div>
                        <div class="bdm-stat-subtext">Visible on the public website</div>
                    </div>
                </div>
            </div>

            <div class="col-6 col-xl-3">
                <div class="card shadow-sm bdm-stat-card bdm-stat-card-featured h-100">
                    <div class="card-body">
                        <div class="bdm-stat-label">Featured</div>
                        <div class="bdm-stat-value">{{ featuredCount }}</div>
                        <div class="bdm-stat-subtext">Highlighted business listings</div>
                    </div>
                </div>
            </div>
        </div>

        <div v-if="isLoading" class="alert alert-info" role="status">
            Loading businesses...
        </div>

        <div v-else-if="totalItems === 0" class="card shadow-sm">
            <div class="card-body py-4 px-4">
                <h2 class="h5">No businesses found</h2>
                <p class="mb-3">No businesses are currently assigned to this website.</p>
                <router-link class="btn btn-primary" to="/new">
                    Add the first business
                </router-link>
            </div>
        </div>

        <div v-else class="table-responsive">

            <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3 bdm-results-summary pt-2 px-3">
                <div class="small text-muted">
                    Showing <strong>{{ pageStart }}</strong> to <strong>{{ pageEnd }}</strong>
                    of <strong>{{ totalItems }}</strong> businesses
                </div>

                <div class="small text-muted">
                    Page <strong>{{ currentPage }}</strong> of <strong>{{ totalPages }}</strong>
                </div>
            </div>

            <table class="table table-striped table-hover align-middle">
                <thead>
                    <tr>
                        <th scope="col">Status</th>
                        <th scope="col">Company</th>
                        <th scope="col">Category</th>
                        <th scope="col">City</th>
                        <th scope="col">Phone</th>
                        <th scope="col">Email</th>
                        <th scope="col">Website</th>
                        <th scope="col">Featured</th>
                        <th scope="col" class="text-end">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in items"
                        :key="item.CompanyId"
                        class="bdm-clickable-row"
                        :title="'Double-click to edit ' + item.CompanyName"
                        @dblclick="onRowDoubleClick($event, item)">
                        <td>
                            <div class="bdm-status-stack">
                                <div class="bdm-status-line">
                                    <span class="badge bdm-badge-membership"
                                          data-bs-toggle="tooltip"
                                          data-bs-placement="top"
                                          :title="'Membership status. This business is currently ' + getMembershipStatusText(item.MembershipStatus).toLowerCase() + '.'">
                                        Current
                                    </span>
                                </div>
                                <div class="bdm-status-line">
                                    <span :class="item.IsActive ? 'badge bg-success-subtle text-success border' : 'badge bg-secondary-subtle text-secondary border'"
                                          data-bs-toggle="tooltip"
                                          data-bs-placement="top"
                                          :title="item.IsActive
                          ? 'Active means this listing is enabled in the directory manager.'
                          : 'Inactive means this listing is disabled in the directory manager.'">
                                        {{ item.IsActive ? 'Active' : 'Inactive' }}
                                    </span>
                                    <span :class="item.IsPublic ? 'badge bg-primary-subtle text-primary border' : 'badge bg-dark-subtle text-dark border'"
                                          data-bs-toggle="tooltip"
                                          data-bs-placement="top"
                                          :title="item.IsPublic
                          ? 'Public means this listing is visible on the public website.'
                          : 'Private means this listing is hidden from the public website.'">
                                        {{ item.IsPublic ? 'Public' : 'Private' }}
                                    </span>
                                </div>
                            </div>
                        </td>
                        <td>
                            <div class="fw-bold">{{ item.CompanyName }}</div>
                            <div class="small text-muted mb-1" v-if="item.PrimaryCategoryName">
                                {{ item.PrimaryCategoryName }}
                            </div>
                            <div class="small bdm-company-slug">
                                <a :href="getPublicCompanyUrl(item.Slug)"
                                   class="bdm-company-slug-link"
                                   target="_blank"
                                   rel="noopener noreferrer"
                                   data-bs-toggle="tooltip"
                                   data-bs-placement="top"
                                   :title="'Open the public listing for ' + item.CompanyName + ' in a new tab.'"
                                   :aria-label="'Open public listing for ' + item.CompanyName + ' in a new tab'">
                                    /{{ item.Slug }}
                                    <i class="fas fa-external-link-alt ms-1 bdm-external-link-icon" aria-hidden="true"></i>
                                    <span class="visually-hidden">Opens in a new tab</span>
                                </a>
                            </div>
                        </td>
                        <td>{{ item.PrimaryCategoryName || '' }}</td>
                        <td>{{ formatCityRegion(item) }}</td>
                        <td>
                            <a v-if="item.Phone" :href="getPhoneLink(item.Phone)" class="text-decoration-none"
                               data-bs-toggle="tooltip" data-bs-placement="top"
                               :title="'Call: ' + item.Phone"
                               :aria-label="'Call ' + item.CompanyName">
                                Call
                            </a>
                        </td>
                        <td>
                            <a v-if="item.Email"
                               :href="'mailto:' + item.Email" class="text-decoration-none"
                               data-bs-toggle="tooltip" data-bs-placement="top"
                               :title="'Email: ' + item.Email"
                               :aria-label="'Email ' + item.CompanyName">
                                Email
                            </a>
                        </td>
                        <td>
                            <a v-if="item.WebsiteUrl" :href="item.WebsiteUrl"
                               data-bs-toggle="tooltip" data-bs-placement="top"
                               :title="'Visit External Website: ' + item.WebsiteUrl"
                               target="_blank" rel="noopener noreferrer">
                                Visit
                            </a>
                        </td>
                        <td>{{ item.IsFeatured ? 'Yes' : 'No' }}</td>
                        <td class="text-end">
                            <div class="bdm-actions-cell">
                                <div class="btn-group btn-group-sm"
                                     role="group"
                                     :aria-label="'Actions for ' + item.CompanyName">
                                    <router-link class="btn btn-outline-primary" :to="`/edit/${item.CompanyId}`">
                                        Edit
                                    </router-link>
                                    <a class="btn btn-outline-secondary"
                                       :href="getPublicCompanyUrl(item.Slug)"
                                       target="_blank"
                                       rel="noopener noreferrer">
                                        View
                                    </a>
                                </div>

                                <div v-if="item.UpdatedOn" class="bdm-updated-meta">
                                    <span class="bdm-updated-meta-inner"
                                          data-bs-toggle="tooltip"
                                          data-bs-placement="top"
                                          :title="'Last updated date and time for this listing.'">
                                        <i class="fas fa-clock me-1" aria-hidden="true"></i>
                                        <span class="visually-hidden">Last updated</span>
                                        {{ formatDateTime(item.UpdatedOn) }}
                                    </span>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>

            <div class="d-flex flex-wrap justify-content-between align-items-center gap-3 mt-3">
                <div class="small text-muted mx-2 px-2">
                    Showing {{ pageStart }} to {{ pageEnd }} of {{ totalItems }} businesses
                </div>

                <nav aria-label="Business directory pagination">
                    <ul class="pagination pagination-sm mb-0">
                        <li class="page-item" :class="{ disabled: currentPage <= 1 }">
                            <button class="page-link" type="button" @click="goToPreviousPage" :disabled="currentPage <= 1">
                                Previous
                            </button>
                        </li>

                        <li v-for="page in visiblePageNumbers"
                            :key="page"
                            class="page-item"
                            :class="{ active: page === currentPage }">
                            <button class="page-link" type="button" @click="goToPage(page)">
                                {{ page }}
                            </button>
                        </li>

                        <li class="page-item" :class="{ disabled: currentPage >= totalPages }">
                            <button class="page-link" type="button" @click="goToNextPage" :disabled="currentPage >= totalPages">
                                Next
                            </button>
                        </li>
                    </ul>
                </nav>
            </div>

        </div>
    </div>
</template>

<style scoped>
    .bdm-page .table a {
        word-break: break-word;
    }

    .bdm-page .badge {
        font-weight: 600;
    }

    .bdm-page .bdm-status-stack {
        display: flex;
        flex-direction: column;
        gap: 0.35rem;
    }

    .bdm-page .bdm-status-line {
        display: flex;
        flex-wrap: wrap;
        gap: 0.35rem;
    }

    .bdm-page .bdm-badge-membership {
        background-color: #664d03;
        color: #ffffff;
        border: 1px solid #664d03;
    }

    .bdm-page .bdm-actions-cell {
        display: inline-flex;
        flex-direction: column;
        align-items: flex-end;
        gap: 0.35rem;
    }

    .bdm-page .bdm-updated-meta {
        font-size: 0.75rem;
        line-height: 1.2;
        color: #6c757d;
        white-space: nowrap;
    }

    .bdm-page .bdm-updated-meta-inner {
        display: inline-flex;
        align-items: center;
        gap: 0.1rem;
        cursor: help;
    }

    .bdm-page .bdm-company-slug {
        margin-top: 0.15rem;
    }

    .bdm-page .bdm-company-slug-link {
        color: #6c757d;
        text-decoration: none;
        font-size: 0.8125rem;
        display: inline-flex;
        align-items: center;
        opacity: 0.85;
        transition: opacity 0.15s ease-in-out, color 0.15s ease-in-out;
    }

        .bdm-page .bdm-company-slug-link:hover,
        .bdm-page .bdm-company-slug-link:focus {
            color: #495057;
            opacity: 1;
            text-decoration: underline;
        }

    .bdm-page .bdm-external-link-icon {
        font-size: 0.75em;
        opacity: 0.8;
    }

    .bdm-page .bdm-clickable-row {
        cursor: default;
    }
</style>

<script>
    import { inject, reactive, ref, onMounted, onUpdated, onBeforeUnmount, computed, watch, nextTick } from 'vue';
    import { useRouter } from 'vue-router';
    import { getConfig, getBusinessesPage } from '../assets/api';
    import { formatCityRegion, formatDateTime, getPublicCompanyUrl } from '../assets/businessDirectoryUtils';
    import { getMembershipStatusText } from '../assets/businessDirectoryOptions';
    import {
        loadBusinessDirectoryListState,
        saveBusinessDirectoryListState,
        clearBusinessDirectoryListState
    } from '../assets/businessDirectoryListState';

    export default {
        name: 'BusinessDirectoryListView', setup() {
            const dnnConfig = inject('dnnConfig');
            const router = useRouter();
            const config = reactive({
                Portals: [],
                Categories: [],
                IsSuperUser: false
            });
            const items = ref([]);
            const isLoading = ref(false);
            const errorMessage = ref('');
            const selectedPortalId = ref(dnnConfig.portalId);
            const searchText = ref('');
            const statusFilter = ref('');
            const visibilityFilter = ref('');
            const featuredOnly = ref(false);
            const currentPage = ref(1);
            const pageSize = ref(10);
            const pageSizeOptions = [10, 20, 40];
            const totalItems = ref(0);
            const activeCount = ref(0);
            const publicCount = ref(0);
            const featuredCount = ref(0);

            const totalPages = computed(() => {
                if (totalItems.value === 0) {
                    return 1;
                }

                return Math.ceil(totalItems.value / pageSize.value);
            });

            const pageStart = computed(() => {
                if (totalItems.value === 0) {
                    return 0;
                }

                return ((currentPage.value - 1) * pageSize.value) + 1;
            });

            const pageEnd = computed(() => {
                if (totalItems.value === 0) {
                    return 0;
                }

                return Math.min(currentPage.value * pageSize.value, totalItems.value);
            });

            const visiblePageNumbers = computed(() => {
                const pages = [];
                const maxVisible = 5;

                let start = Math.max(1, currentPage.value - 2);
                let end = Math.min(totalPages.value, start + maxVisible - 1);

                if ((end - start + 1) < maxVisible) {
                    start = Math.max(1, end - maxVisible + 1);
                }

                for (let i = start; i <= end; i++) {
                    pages.push(i);
                }

                return pages;
            });

            function getCurrentListState() {
                return {
                    selectedPortalId: selectedPortalId.value,
                    searchText: searchText.value || '',
                    statusFilter: statusFilter.value || '',
                    visibilityFilter: visibilityFilter.value || '',
                    featuredOnly: !!featuredOnly.value,
                    currentPage: currentPage.value,
                    pageSize: pageSize.value
                };
            }

            function persistListState() {
                saveBusinessDirectoryListState(dnnConfig, getCurrentListState());
            }

            function restoreListState() {
                const savedState = loadBusinessDirectoryListState(dnnConfig);

                if (!savedState) {
                    return;
                }

                selectedPortalId.value = savedState.selectedPortalId || dnnConfig.portalId;
                searchText.value = savedState.searchText || '';
                statusFilter.value = savedState.statusFilter || '';
                visibilityFilter.value = savedState.visibilityFilter || '';
                featuredOnly.value = !!savedState.featuredOnly;
                currentPage.value = savedState.currentPage > 0 ? savedState.currentPage : 1;
                pageSize.value = pageSizeOptions.includes(savedState.pageSize) ? savedState.pageSize : 10;
            }

            function isInteractiveElement(target) {
                if (!target || typeof target.closest !== 'function') {
                    return false;
                }

                return !!target.closest('a, button, .btn, input, select, textarea, label, [role="button"], [data-row-ignore-dblclick="true"]');
            }

            function onRowDoubleClick(event, item) {
                if (!item || !item.CompanyId) {
                    return;
                }

                if (isInteractiveElement(event.target)) {
                    return;
                }

                persistListState();
                router.push(`/edit/${item.CompanyId}`);
            }

            function loadConfig() {
                return new Promise((resolve, reject) => {
                    getConfig(dnnConfig, selectedPortalId.value, false, (result) => {
                        config.Portals = result?.Portals || [];
                        config.Categories = [];
                        config.IsSuperUser = !!result?.IsSuperUser;
                        resolve();
                    }, reject);
                });
            }

            function loadBusinesses() {
                isLoading.value = true;
                errorMessage.value = '';

                const request = {
                    PortalId: selectedPortalId.value,
                    SearchText: searchText.value || '',
                    StatusFilter: statusFilter.value || '',
                    VisibilityFilter: visibilityFilter.value || '',
                    FeaturedOnly: !!featuredOnly.value,
                    PageNumber: currentPage.value,
                    PageSize: pageSize.value
                };

                return new Promise((resolve, reject) => {
                    getBusinessesPage(dnnConfig, request, (result) => {
                        items.value = result?.Items || [];
                        totalItems.value = result?.TotalItems || 0;
                        activeCount.value = result?.ActiveCount || 0;
                        publicCount.value = result?.PublicCount || 0;
                        featuredCount.value = result?.FeaturedCount || 0;
                        isLoading.value = false;
                        resolve();
                    }, (error) => {
                        items.value = [];
                        totalItems.value = 0;
                        activeCount.value = 0;
                        publicCount.value = 0;
                        featuredCount.value = 0;
                        isLoading.value = false;
                        errorMessage.value = error || 'Unable to load businesses.';
                        reject(error);
                    });
                });
            }

            function reloadAll() {
                errorMessage.value = '';

                return Promise.all([
                    loadConfig(),
                    loadBusinesses()
                ]).catch((error) => {
                    if (!errorMessage.value) {
                        errorMessage.value = error || 'Unable to load the business directory.';
                    }
                });
            }

            function reloadBusinessesOnly() {
                errorMessage.value = '';

                return loadBusinesses().catch((error) => {
                    if (!errorMessage.value) {
                        errorMessage.value = error || 'Unable to load businesses.';
                    }
                });
            }

            function onPortalChanged() {
                errorMessage.value = '';
                currentPage.value = 1;
                persistListState();

                return Promise.all([
                    loadConfig(),
                    loadBusinesses()
                ]).catch((error) => {
                    if (!errorMessage.value) {
                        errorMessage.value = error || 'Unable to load the business directory.';
                    }
                });
            }

            function resetFilters() {
                searchText.value = '';
                statusFilter.value = '';
                visibilityFilter.value = '';
                featuredOnly.value = false;
                pageSize.value = 10;
                currentPage.value = 1;
                //selectedPortalId.value = dnnConfig.portalId; /* uncomment to remove support for persisting the portal selection for superusers */

                clearBusinessDirectoryListState(dnnConfig);
                reloadBusinessesOnly();
            }

            function goToPage(page) {
                const targetPage = Number(page);

                if (targetPage < 1) {
                    currentPage.value = 1;
                }
                else if (targetPage > totalPages.value) {
                    currentPage.value = totalPages.value;
                }
                else {
                    currentPage.value = targetPage;
                }

                reloadBusinessesOnly();
            }

            function goToPreviousPage() {
                goToPage(currentPage.value - 1);
            }

            function goToNextPage() {
                goToPage(currentPage.value + 1);
            }

            function getPhoneLink(phone) {
                if (!phone) {
                    return '';
                }

                const cleaned = String(phone).replace(/[^0-9+]/g, '');
                return `tel:${cleaned}`;
            }

            let tooltipInstances = [];

            function disposeTooltips() {
                tooltipInstances.forEach((instance) => {
                    if (instance && typeof instance.dispose === 'function') {
                        instance.dispose();
                    }
                });

                tooltipInstances = [];
            }

            function initializeTooltips() {
                disposeTooltips();

                if (!window.bootstrap || !window.bootstrap.Tooltip) {
                    return;
                }

                const tooltipElements = document.querySelectorAll('.bdm-page [data-bs-toggle="tooltip"]');
                tooltipInstances = Array.from(tooltipElements).map((element) => new window.bootstrap.Tooltip(element));
            }

            function refreshTooltips() {
                nextTick(() => {
                    initializeTooltips();
                });
            }

            let searchDebounceTimer = null;

            function reloadFromFirstPage() {
                currentPage.value = 1;
                return reloadBusinessesOnly();
            }

            watch([statusFilter, visibilityFilter, featuredOnly, pageSize], () => {
                reloadFromFirstPage();
            });

            watch(searchText, () => {
                if (searchDebounceTimer) {
                    window.clearTimeout(searchDebounceTimer);
                }

                searchDebounceTimer = window.setTimeout(() => {
                    reloadFromFirstPage();
                }, 300);
            });

            watch(totalPages, (newValue) => {
                if (currentPage.value > newValue) {
                    currentPage.value = newValue;
                }
            });

            watch(
                [selectedPortalId, searchText, statusFilter, visibilityFilter, featuredOnly, currentPage, pageSize],
                () => {
                    persistListState();
                },
                { deep: false }
            );

            onMounted(() => {
                restoreListState();
                reloadAll();
                refreshTooltips();
            });

            onUpdated(() => {
                refreshTooltips();
            });

            onBeforeUnmount(() => {
                if (searchDebounceTimer) {
                    window.clearTimeout(searchDebounceTimer);
                    searchDebounceTimer = null;
                }

                disposeTooltips();
            });

            return {
                config,
                items,
                isLoading,
                errorMessage,
                selectedPortalId,
                searchText,
                statusFilter,
                visibilityFilter,
                featuredOnly,
                currentPage,
                pageSize,
                pageSizeOptions,
                totalItems,
                totalPages,
                pageStart,
                pageEnd,
                visiblePageNumbers,
                activeCount,
                publicCount,
                featuredCount,
                reloadAll,
                reloadBusinessesOnly,
                onPortalChanged,
                resetFilters,
                goToPage,
                goToPreviousPage,
                goToNextPage,
                formatCityRegion,
                formatDateTime,
                getMembershipStatusText,
                getPublicCompanyUrl,
                getPhoneLink,
                onRowDoubleClick
            };
        }
    };
</script>
