<template>
    <div class="bdm-page container-fluid py-3">
        <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
            <div>
                <h1 class="h3 mb-1">{{ isEditMode ? 'Edit Business' : 'Add Business' }}</h1>
                <p class="text-muted mb-0">Create or update a directory listing.</p>
            </div>

            <div class="d-flex gap-2">
                <router-link class="btn btn-outline-secondary" to="/">
                    Cancel
                </router-link>

                <button class="btn btn-outline-primary" type="button" 
                        :disabled="!canViewPublicListing"
                        :title="canViewPublicListing
                            ? 'Open the public business listing in a new tab.'
                            : 'This business must exist and have a valid slug before it can be viewed.'"
                        :aria-label="canViewPublicListing
                            ? 'Open the public business listing in a new tab'
                            : 'View is unavailable until this business exists and has a valid slug'"
                        @click="openPublicView">
                    <i class="fas fa-external-link-alt me-2" aria-hidden="true"></i>
                    <span>View</span>
                    <span class="visually-hidden" v-if="canViewPublicListing">Opens in a new tab</span>
                </button>

                <button class="btn btn-primary"
                        type="button"
                        :disabled="isSaving"
                        @click="save">
                    {{ isSaving ? 'Saving...' : 'Save Business' }}
                </button>
            </div>
        </div>

        <div v-if="errorMessage" class="alert alert-danger" role="alert">
            {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="alert alert-success" role="status">
            {{ successMessage }}
        </div>

        <div v-if="isLoading" class="alert alert-info" role="status">
            Loading business...
        </div>

        <div v-if="validationSummary.length" class="alert alert-danger" role="alert">
            <div class="fw-semibold mb-2">
                Please fix the following before saving:
            </div>
            <ul class="mb-0 ps-3">
                <li v-for="(item, index) in validationSummary" :key="index">
                    {{ item }}
                </li>
            </ul>
        </div>

        <form v-else @submit.prevent="save">
            <div class="row g-4">
                <div class="col-12 col-xl-8">
                    <div class="card shadow-sm mb-4">
                        <div class="card-body">
                            <h2 class="h5 mb-3">Basic Details</h2>

                            <div class="mb-3" v-if="config.IsSuperUser">
                                <label class="form-label" for="PortalId">Portal</label>
                                <select id="PortalId"
                                        class="form-select"
                                        v-model.number="model.PortalId"
                                        @change="reloadConfigForPortal">
                                    <option v-for="portal in config.Portals"
                                            :key="portal.PortalId"
                                            :value="portal.PortalId">
                                        {{ portal.PortalName }}
                                    </option>
                                </select>
                            </div>

                            <div class="row g-3">
                                <div class="col-md-8">
                                    <label class="form-label" for="CompanyName">Company Name</label>
                                    <input id="CompanyName"
                                           class="form-control"
                                           :class="{ 'is-invalid': hasFieldError('CompanyName') }"
                                           v-model="model.CompanyName"
                                           @blur="onCompanyNameBlur(); touchField('CompanyName')" />
                                    <div v-if="hasFieldError('CompanyName')" class="invalid-feedback d-block">
                                        {{ getFieldError('CompanyName') }}
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <label class="form-label" for="MemberSinceYear">Member Since Year</label>
                                    <input id="MemberSinceYear"
                                           class="form-control"
                                           type="number"
                                           min="1900"
                                           max="2100"
                                           v-model.number="model.MemberSinceYear" />
                                </div>

                                <div class="col-md-8">
                                    <label class="form-label" for="Slug">Slug</label>
                                    <div class="input-group">
                                        <input id="Slug"
                                               class="form-control"
                                               v-model="model.Slug"
                                               @blur="checkSlug" />
                                        <button class="btn btn-outline-secondary"
                                                type="button"
                                                @click="checkSlug">
                                            Check
                                        </button>
                                    </div>
                                    <div class="form-text">
                                        Public URL: {{ publicUrlPreview }}
                                    </div>
                                    <div v-if="slugMessage" class="small mt-1" :class="slugIsAvailable ? 'text-success' : 'text-danger'">
                                        {{ slugMessage }}
                                    </div>
                                    <div v-if="hasFieldError('Slug')" class="invalid-feedback d-block">
                                        {{ getFieldError('Slug') }}
                                    </div>
                                </div>

                                <div class="col-md-4">
                                    <label class="form-label" for="MembershipStatus">Membership Status</label>
                                    <select id="MembershipStatus"
                                            class="form-select"
                                            v-model.number="model.MembershipStatus">
                                        <option v-for="option in membershipStatusOptions"
                                                :key="option.value"
                                                :value="option.value">
                                            {{ option.text }}
                                        </option>
                                    </select>
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label" for="LegacySystem">Legacy System</label>
                                    <input id="LegacySystem" class="form-control" v-model="model.LegacySystem" />
                                </div>

                                <div class="col-md-6">
                                    <label class="form-label" for="LegacyCompanyId">Legacy Company ID</label>
                                    <input id="LegacyCompanyId" class="form-control" v-model="model.LegacyCompanyId" />
                                </div>
                            </div>

                            <div class="mt-3">
                                <label class="form-label" for="ShortDescription">Short Description</label>
                                <RichTextEditor v-model="model.ShortDescription"
                                                mode="basic"
                                                placeholder="Add a short summary for this business."
                                                :invalid="hasFieldError('ShortDescription')"
                                                :validation-message="getFieldError('ShortDescription')"
                                                help-text="Use a short, clean summary. Keep it scannable."
                                                toolbar-label="Short description editor toolbar"
                                                @blur="touchField('ShortDescription')" />
                            </div>

                            <div class="mt-3">
                                <label class="form-label" for="LongDescription">Long Description</label>
                                <RichTextEditor v-model="model.LongDescription"
                                                mode="standard"
                                                placeholder="Add the full business description."
                                                :invalid="hasFieldError('LongDescription')"
                                                :validation-message="getFieldError('LongDescription')"
                                                help-text="Use paragraphs, lists, and links where helpful."
                                                toolbar-label="Long description editor toolbar"
                                                @blur="touchField('LongDescription')" />
                            </div>

                            <div class="mt-3">
                                <label class="form-label" for="CategoryIds">Categories</label>
                                <select id="CategoryIds"
                                        class="form-select"
                                        multiple
                                        v-model="model.CategoryIds">
                                    <option v-for="category in config.Categories"
                                            :key="category.CategoryId"
                                            :value="category.CategoryId">
                                        {{ category.CategoryName }}
                                    </option>
                                </select>
                                <div class="form-text">Hold Ctrl or Command to select multiple categories.</div>
                            </div>
                        </div>
                    </div>

                    <div class="card shadow-sm mb-4">
                        <div class="card-body">
                            <h2 class="h5 mb-3">Contact and Address</h2>

                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="form-label">Website URL</label>
                                    <input class="form-control"
                                           :class="{ 'is-invalid': hasFieldError('WebsiteUrl') }"
                                           v-model="model.WebsiteUrl"
                                           @blur="touchField('WebsiteUrl')" />
                                    <div v-if="hasFieldError('WebsiteUrl')" class="invalid-feedback d-block">
                                        {{ getFieldError('WebsiteUrl') }}
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Phone</label>
                                    <input class="form-control" v-model="model.Phone" />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Email</label>
                                    <input class="form-control" v-model="model.Email" />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Primary Business Email</label>
                                    <input class="form-control" v-model="model.PrimaryBusinessEmail" />
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Address 1</label>
                                    <input class="form-control" v-model="model.Address1" />
                                </div>
                                <div class="col-12">
                                    <label class="form-label">Address 2</label>
                                    <input class="form-control" v-model="model.Address2" />
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">City</label>
                                    <input class="form-control" v-model="model.City" />
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">Region</label>
                                    <input class="form-control" v-model="model.Region" />
                                </div>
                                <div class="col-md-4">
                                    <label class="form-label">Postal Code</label>
                                    <input class="form-control" v-model="model.PostalCode" />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Latitude</label>
                                    <input class="form-control" type="number" step="any" v-model.number="model.Latitude" />
                                </div>
                                <div class="col-md-6">
                                    <label class="form-label">Longitude</label>
                                    <input class="form-control" type="number" step="any" v-model.number="model.Longitude" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card shadow-sm mb-4">
                        <div class="card-body">
                            <h2 class="h5 mb-3">Media and SEO</h2>

                            <div class="row g-3">
                                <div class="col-md-4">
                                    <label class="form-label">Logo File ID</label>
                                    <input class="form-control" type="number" v-model.number="model.LogoFileId" />
                                </div>
                                <div class="col-md-8">
                                    <label class="form-label">Logo URL</label>
                                    <input class="form-control" v-model="model.LogoUrl" />
                                </div>
                            </div>

                            <hr />

                            <div class="mb-3">
                                <label class="form-label">SEO Title</label>
                                <input class="form-control" maxlength="255" v-model="model.SeoTitle" />
                                <div class="form-text">{{ (model.SeoTitle || '').length }}/255</div>
                            </div>

                            <div class="mb-3">
                                <label class="form-label">SEO Description</label>
                                <textarea class="form-control"
                                          rows="3"
                                          maxlength="500"
                                          v-model="model.SeoDescription"></textarea>
                                <div class="form-text">{{ (model.SeoDescription || '').length }}/500</div>
                            </div>

                            <div class="mb-3">
                                <label class="form-label">Canonical URL</label>
                                <input class="form-control" v-model="model.CanonicalUrl" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label">Meta Robots</label>
                                <input class="form-control" maxlength="100" v-model="model.MetaRobots" />
                                <div class="form-text">{{ (model.MetaRobots || '').length }}/100</div>
                            </div>

                            <div class="mb-0">
                                <label class="form-label">Open Graph Image URL</label>
                                <input class="form-control" v-model="model.OgImageUrl" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-12 col-xl-4">
                    <div class="card shadow-sm mb-4">
                        <div class="card-body">
                            <h2 class="h5 mb-3">Publishing</h2>

                            <div class="form-check mb-2">
                                <input id="IsActive" class="form-check-input" type="checkbox" v-model="model.IsActive" />
                                <label class="form-check-label" for="IsActive">Active</label>
                            </div>

                            <div class="form-check mb-2">
                                <input id="IsPublic" class="form-check-input" type="checkbox" v-model="model.IsPublic" />
                                <label class="form-check-label" for="IsPublic">Public</label>
                            </div>

                            <div class="form-check mb-3">
                                <input id="IsFeatured" class="form-check-input" type="checkbox" v-model="model.IsFeatured" />
                                <label class="form-check-label" for="IsFeatured">Featured</label>
                            </div>

                            <div class="mb-3" v-if="model.IsFeatured">
                                <label class="form-label">Featured Sort Order</label>
                                <input class="form-control" type="number" v-model.number="model.FeaturedSortOrder" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label">Paid Through Date</label>
                                <input class="form-control" type="date" v-model="model.PaidThroughDate" />
                            </div>

                            <div class="mb-0">
                                <label class="form-label">Renewal Date</label>
                                <input class="form-control" type="date" v-model="model.RenewalDate" />
                            </div>
                        </div>
                    </div>

                    <div class="card shadow-sm">
                        <div class="card-body">
                            <h2 class="h5 mb-3">Social</h2>

                            <div class="mb-3">
                                <label class="form-label">LinkedIn URL</label>
                                <input class="form-control" v-model="model.LinkedInUrl" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label">Facebook URL</label>
                                <input class="form-control" v-model="model.FacebookUrl" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label">Instagram URL</label>
                                <input class="form-control" v-model="model.InstagramUrl" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label">Twitter/X URL</label>
                                <input class="form-control" v-model="model.TwitterUrl" />
                            </div>

                            <div class="mb-0">
                                <label class="form-label">TikTok URL</label>
                                <input class="form-control" v-model="model.TikTokUrl" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>
</template>
<style scoped>
    .bdm-page {
        --bdm-border-color: rgba(0, 0, 0, 0.08);
        --bdm-muted-bg: #f8f9fa;
    }

        .bdm-page .card {
            border: 1px solid var(--bdm-border-color);
            border-radius: 0.9rem;
            overflow: hidden;
        }

        .bdm-page .card-body {
            padding: 1.5rem;
        }

            .bdm-page .card-body h2 {
                font-weight: 700;
                letter-spacing: 0.01em;
            }

        .bdm-page .form-label {
            font-weight: 600;
            margin-bottom: 0.45rem;
        }

        .bdm-page .form-text {
            font-size: 0.875rem;
        }

        .bdm-page .form-control,
        .bdm-page .form-select {
            border-radius: 0.65rem;
        }

            .bdm-page .form-control:focus,
            .bdm-page .form-select:focus {
                box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.12);
            }

        .bdm-page .btn {
            border-radius: 0.65rem;
        }

        .bdm-page .input-group > .btn {
            white-space: nowrap;
        }

        .bdm-page .alert {
            border-radius: 0.8rem;
        }

        .bdm-page .form-check {
            padding: 0.9rem 1rem 0.9rem 2.25rem;
            background: var(--bdm-muted-bg);
            border: 1px solid var(--bdm-border-color);
            border-radius: 0.75rem;
        }

            .bdm-page .form-check + .form-check {
                margin-top: 0.75rem;
            }

        .bdm-page .form-check-input {
            margin-top: 0.2rem;
        }

        .bdm-page select[multiple] {
            min-height: 180px;
        }

        .bdm-page hr {
            opacity: 0.12;
            margin: 1.5rem 0;
        }

        .bdm-page .invalid-feedback {
            font-size: 0.875rem;
        }

        .bdm-page :deep(.bdm-editor-toolbar .btn.active) {
            color: #fff;
            background-color: #0d6efd;
            border-color: #0d6efd;
        }

        .bdm-page :deep(.bdm-editor-shell) {
            border: 1px solid #ced4da;
            border-radius: 0.65rem;
            background: #fff;
            transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
        }

        .bdm-page :deep(.bdm-editor-shell:focus-within) {
            border-color: #86b7fe;
            box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.12);
        }

        .bdm-page :deep(.bdm-editor-shell.is-invalid) {
            border-color: #dc3545;
        }

        .bdm-page :deep(.bdm-editor-content) {
            min-height: 120px;
            padding: 0.875rem 1rem;
            border: 0;
            box-shadow: none !important;
        }

        .bdm-page :deep(.bdm-editor-content p:last-child) {
            margin-bottom: 0;
        }

        .bdm-page :deep(.bdm-editor-content ul),
        .bdm-page :deep(.bdm-editor-content ol) {
            padding-left: 1.25rem;
        }

        .bdm-page :deep(.bdm-editor-content blockquote) {
            border-left: 4px solid #dee2e6;
            padding-left: 1rem;
            color: #6c757d;
            margin: 1rem 0;
        }

        .bdm-page .btn:disabled {
            cursor: not-allowed;
        }
</style>

<script>
    import { inject, reactive, ref, onMounted, computed, nextTick, defineAsyncComponent } from 'vue';
    import { useRoute, useRouter } from 'vue-router';
    import { getConfig, getBusinessForEdit, checkSlugAvailability, saveBusiness } from '../assets/api';
    import { membershipStatusOptions } from '../assets/businessDirectoryOptions';

    const RichTextEditor = defineAsyncComponent(() => import('../components/RichTextEditor.vue'));

    import {
        normalizeSlug,
        formatDateForInput,
        getPublicCompanyUrl,
        isValidUrl,
        isValidEmail,
        isValidYear,
        isValidLatitude,
        isValidLongitude
    } from '../assets/businessDirectoryUtils';

    function createDefaultModel(portalId) {
        return {
            CompanyId: 0,
            PortalId: portalId,
            LegacySystem: '',
            LegacyCompanyId: '',
            CompanyName: '',
            Slug: '',
            ShortDescription: '',
            LongDescription: '',
            WebsiteUrl: '',
            Phone: '',
            Email: '',
            PrimaryBusinessEmail: '',
            Address1: '',
            Address2: '',
            City: '',
            Region: '',
            PostalCode: '',
            Latitude: null,
            Longitude: null,
            MembershipStatus: 1,
            MemberSinceYear: null,
            PaidThroughDate: '',
            RenewalDate: '',
            IsFeatured: false,
            FeaturedSortOrder: 0,
            LogoFileId: null,
            LogoUrl: '',
            LinkedInUrl: '',
            FacebookUrl: '',
            InstagramUrl: '',
            TwitterUrl: '',
            TikTokUrl: '',
            IsPublic: true,
            IsActive: true,
            SeoTitle: '',
            SeoDescription: '',
            CanonicalUrl: '',
            MetaRobots: '',
            OgImageUrl: '',
            CategoryIds: []
        };
    }

    export default {
        name: 'BusinessDirectoryEditView',
        components: {
            RichTextEditor
        },
        setup() {
            const dnnConfig = inject('dnnConfig');
            const route = useRoute();
            const router = useRouter();

            const model = reactive(createDefaultModel(dnnConfig.portalId));
            const config = reactive({
                Portals: [],
                Categories: [],
                IsSuperUser: false
            });

            const isLoading = ref(false);
            const isSaving = ref(false);
            const errorMessage = ref('');
            const successMessage = ref('');
            const slugMessage = ref('');
            const slugIsAvailable = ref(false);
            const validationErrors = reactive({});
            const touchedFields = reactive({});
            const attemptedSave = ref(false);

            const isEditMode = computed(() => !!route.params.companyId);
            const normalizedPublicSlug = computed(() => normalizeSlug(model.Slug || model.CompanyName));
            const publicUrlPreview = computed(() => getPublicCompanyUrl(normalizedPublicSlug.value));
            const canViewPublicListing = computed(() => {
                return !!(isEditMode.value && model.CompanyId > 0 && normalizedPublicSlug.value);
            });
            const validationSummary = computed(() => Object.values(validationErrors));

            function loadConfig() {
                return new Promise((resolve, reject) => {
                    getConfig(dnnConfig, model.PortalId, true, (result) => {
                        config.Portals = result?.Portals || [];
                        config.Categories = result?.Categories || [];
                        config.IsSuperUser = !!result?.IsSuperUser;
                        resolve();
                    }, reject);
                });
            }

            function loadBusiness() {
                if (!isEditMode.value) {
                    return Promise.resolve();
                }

                isLoading.value = true;

                return new Promise((resolve, reject) => {
                    getBusinessForEdit(dnnConfig, model.PortalId, Number(route.params.companyId), (result) => {
                        Object.assign(model, result || {});

                        model.PaidThroughDate = formatDateForInput(model.PaidThroughDate);
                        model.RenewalDate = formatDateForInput(model.RenewalDate);

                        if (!Array.isArray(model.CategoryIds)) {
                            model.CategoryIds = [];
                        }

                        isLoading.value = false;
                        resolve();
                    }, (error) => {
                        isLoading.value = false;
                        errorMessage.value = error || 'Unable to load the business.';
                        reject(error);
                    });
                });
            }

            function reloadConfigForPortal() {
                model.CategoryIds = [];
                loadConfig().catch(() => { });
            }

            function onCompanyNameBlur() {
                if (!model.Slug) {
                    model.Slug = normalizeSlug(model.CompanyName);
                }
            }

            function checkSlug() {
                model.Slug = normalizeSlug(model.Slug);

                if (!model.Slug) {
                    slugIsAvailable.value = false;
                    slugMessage.value = 'Slug is required.';
                    return;
                }

                checkSlugAvailability(
                    dnnConfig,
                    model.PortalId,
                    model.Slug,
                    model.CompanyId || null,
                    (result) => {
                        slugIsAvailable.value = !!result?.IsAvailable;
                        slugMessage.value = slugIsAvailable.value ? 'Slug is available.' : 'Slug is already in use.';
                    },
                    (error) => {
                        slugIsAvailable.value = false;
                        slugMessage.value = error || 'Unable to validate slug.';
                    });
            }

            function openPublicView() {
                if (!canViewPublicListing.value) {
                    return;
                }

                const url = getPublicCompanyUrl(normalizedPublicSlug.value);
                window.open(url, '_blank');
            }

            function validateClient() {
                const maxSeoTitleLength = 255;
                const maxSeoDescriptionLength = 500;
                const maxMetaRobotsLength = 100;
                const currentYear = new Date().getFullYear() + 1;

                attemptedSave.value = true;
                clearValidationErrors();

                model.Slug = normalizeSlug(model.Slug || model.CompanyName);

                setFieldError('PortalId',
                    !model.PortalId || model.PortalId <= 0
                        ? 'A valid portal is required.'
                        : '');

                setFieldError('CompanyName',
                    !model.CompanyName || !model.CompanyName.trim()
                        ? 'Company name is required.'
                        : '');

                setFieldError('Slug',
                    !model.Slug
                        ? 'Slug is required.'
                        : '');

                setFieldError('WebsiteUrl',
                    !isValidUrl(model.WebsiteUrl)
                        ? 'Website URL must be a valid http or https URL.'
                        : '');

                setFieldError('LogoUrl',
                    !isValidUrl(model.LogoUrl)
                        ? 'Logo URL must be a valid http or https URL.'
                        : '');

                setFieldError('CanonicalUrl',
                    !isValidUrl(model.CanonicalUrl)
                        ? 'Canonical URL must be a valid http or https URL.'
                        : '');

                setFieldError('OgImageUrl',
                    !isValidUrl(model.OgImageUrl)
                        ? 'Open Graph Image URL must be a valid http or https URL.'
                        : '');

                setFieldError('LinkedInUrl',
                    !isValidUrl(model.LinkedInUrl)
                        ? 'LinkedIn URL must be a valid http or https URL.'
                        : '');

                setFieldError('FacebookUrl',
                    !isValidUrl(model.FacebookUrl)
                        ? 'Facebook URL must be a valid http or https URL.'
                        : '');

                setFieldError('InstagramUrl',
                    !isValidUrl(model.InstagramUrl)
                        ? 'Instagram URL must be a valid http or https URL.'
                        : '');

                setFieldError('TwitterUrl',
                    !isValidUrl(model.TwitterUrl)
                        ? 'Twitter/X URL must be a valid http or https URL.'
                        : '');

                setFieldError('TikTokUrl',
                    !isValidUrl(model.TikTokUrl)
                        ? 'TikTok URL must be a valid http or https URL.'
                        : '');

                setFieldError('Email',
                    !isValidEmail(model.Email)
                        ? 'Email must be a valid email address.'
                        : '');

                setFieldError('PrimaryBusinessEmail',
                    !isValidEmail(model.PrimaryBusinessEmail)
                        ? 'Primary business email must be a valid email address.'
                        : '');

                setFieldError('MemberSinceYear',
                    !isValidYear(model.MemberSinceYear, 1900, currentYear)
                        ? `Member since year must be between 1900 and ${currentYear}.`
                        : '');

                setFieldError('Latitude',
                    !isValidLatitude(model.Latitude)
                        ? 'Latitude must be between -90 and 90.'
                        : '');

                setFieldError('Longitude',
                    !isValidLongitude(model.Longitude)
                        ? 'Longitude must be between -180 and 180.'
                        : '');

                setFieldError('SeoTitle',
                    model.SeoTitle && model.SeoTitle.length > maxSeoTitleLength
                        ? `SEO title cannot exceed ${maxSeoTitleLength} characters.`
                        : '');

                setFieldError('SeoDescription',
                    model.SeoDescription && model.SeoDescription.length > maxSeoDescriptionLength
                        ? `SEO description cannot exceed ${maxSeoDescriptionLength} characters.`
                        : '');

                setFieldError('MetaRobots',
                    model.MetaRobots && model.MetaRobots.length > maxMetaRobotsLength
                        ? `Meta robots cannot exceed ${maxMetaRobotsLength} characters.`
                        : '');

                return Object.keys(validationErrors).length === 0;
            }

            function setFieldError(fieldName, message) {
                if (message) {
                    validationErrors[fieldName] = message;
                }
                else {
                    delete validationErrors[fieldName];
                }
            }

            function getFieldError(fieldName) {
                return validationErrors[fieldName] || '';
            }

            function touchField(fieldName) {
                touchedFields[fieldName] = true;
            }

            function hasFieldError(fieldName) {
                return !!validationErrors[fieldName] && (touchedFields[fieldName] || attemptedSave.value);
            }

            function clearValidationErrors() {
                Object.keys(validationErrors).forEach((key) => {
                    delete validationErrors[key];
                });
            }

            function save() {
                if (!validateClient()) {
                    errorMessage.value = 'Please review the highlighted fields.';

                    nextTick(() => {
                        const firstInvalid = document.querySelector('.bdm-page .is-invalid');
                        if (firstInvalid && typeof firstInvalid.focus === 'function') {
                            firstInvalid.focus();
                        }
                    });

                    return;
                }

                isSaving.value = true;
                errorMessage.value = '';
                successMessage.value = '';

                saveBusiness(dnnConfig, model, (result) => {
                    model.CompanyId = result?.CompanyId || model.CompanyId;
                    isSaving.value = false;
                    successMessage.value = 'Business saved successfully.';
                    router.push('/');
                }, (error) => {
                    isSaving.value = false;
                    errorMessage.value = error || 'Unable to save the business.';
                });
            }

            onMounted(() => {
                loadConfig()
                    .then(loadBusiness)
                    .catch(() => { });
            });

            return {
                model,
                config,
                isLoading,
                isSaving,
                errorMessage,
                successMessage,
                slugMessage,
                slugIsAvailable,
                isEditMode,
                publicUrlPreview,
                membershipStatusOptions,
                onCompanyNameBlur,
                checkSlug,
                save,
                reloadConfigForPortal,
                validationErrors,
                validationSummary,
                touchedFields,
                attemptedSave,
                hasFieldError,
                getFieldError,
                touchField,
                normalizedPublicSlug,
                canViewPublicListing,
                openPublicView
            };
        }
    };
</script>