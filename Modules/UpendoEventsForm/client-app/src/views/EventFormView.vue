<template>
    <div class="container-fluid py-3">
        <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
            <div>
                <h2 class="h3 mb-1">{{ isEditMode ? 'Edit Event' : 'Create Event' }}</h2>
                <p class="text-muted mb-0">
                    {{ isEditMode ? 'Update the event details below and save your changes.' : 'Fill out the event details below to create a new event.' }}
                </p>
            </div>

            <div class="d-flex align-items-center gap-2">
                <span v-if="isSaving" class="text-muted small">Saving...</span>

                <button type="button"
                        class="btn btn-outline-secondary"
                        :disabled="isSaving"
                        @click="cancelAndReturn">
                    Cancel
                </button>

                <button type="button"
                        class="btn btn-primary px-4"
                        :disabled="isSaving"
                        @click="save">
                    <span v-if="isSaving" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    {{ isSaving ? 'Saving Event...' : 'Save Event' }}
                </button>
            </div>
        </div>

        <div class="alert alert-success" v-if="saveMessage">
            <div class="fw-semibold">{{ saveMessage }}</div>

            <div class="mt-2" v-if="showReturnAfterSave">
                <div>
                    Returning to the previous Events page in
                    <strong>{{ returnCountdown }}</strong>
                    second<span v-if="returnCountdown !== 1">s</span>.
                </div>

                <div class="mt-2 d-flex flex-wrap gap-2">
                    <button type="button"
                            class="btn btn-sm btn-outline-success"
                            @click="redirectBackToEventsPage">
                        Return now
                    </button>

                    <button type="button"
                            class="btn btn-sm btn-outline-secondary"
                            @click="cancelReturnRedirect">
                        Stay here
                    </button>
                </div>
            </div>
        </div>

        <div class="alert alert-danger" v-if="globalError">
            {{ globalError }}
        </div>

        <div class="alert alert-info" v-if="isLoadingEvent">
            Loading event details...
        </div>

        <div class="row g-3">
            <div class="col-12 col-lg-8">
                <div class="card shadow-sm border-0 mb-4">
                    <div class="card-body px-4">
                        <h3 class="h5 mb-3">Basics</h3>
                        <p class="text-muted small mb-3">
                            Core event details used for event listings and the public detail page.
                        </p>

                        <div class="mb-3" v-if="config.IsSuperUser">
                            <label class="form-label">Website</label>
                            <select class="form-select"
                                    :class="getInputClass('PortalId')"
                                    v-model.number="model.PortalId"
                                    @change="onPortalChanged(); markTouched('PortalId');">
                                <option v-for="portal in config.Portals" :key="portal.PortalId" :value="portal.PortalId">
                                    {{ portal.PortalName }}
                                </option>
                            </select>
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('PortalId')">
                                {{ getVisibleFieldError('PortalId') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Title</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('Title')"
                                     @mouseleave="closeFieldHelp('Title')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('Title') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('Title')"
                                            aria-label="Title field guidance"
                                            @click="toggleFieldHelp('Title')"
                                            @focus="openFieldHelp('Title')"
                                            @blur="closeFieldHelp('Title')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('Title')"
                                         :id="getFieldHelpPopoverId('Title')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('Title') }}
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <input class="form-control"
                                   :class="getInputClass('Title')"
                                   v-model="model.Title"
                                   @blur="onTitleBlur(); markTouched('Title');" />
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('Title')">
                                {{ getVisibleFieldError('Title') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Slug</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('Slug')"
                                     @mouseleave="closeFieldHelp('Slug')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('Slug') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('Slug')"
                                            aria-label="Slug field guidance"
                                            @click="toggleFieldHelp('Slug')"
                                            @focus="openFieldHelp('Slug')"
                                            @blur="closeFieldHelp('Slug')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('Slug')"
                                         :id="getFieldHelpPopoverId('Slug')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('Slug') }}
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <input class="form-control"
                                   :class="getInputClass('Slug')"
                                   v-model="model.Slug"
                                   maxlength="220"
                                   @blur="onSlugBlur(); markTouched('Slug');" />
                            <div class="form-text" v-if="slugStatusMessage && !getVisibleFieldError('Slug')">
                                {{ slugStatusMessage }}
                            </div>
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('Slug')">
                                {{ getVisibleFieldError('Slug') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Short Summary</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('ShortSummary')"
                                     @mouseleave="closeFieldHelp('ShortSummary')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('ShortSummary') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('ShortSummary')"
                                            aria-label="Short summary field guidance"
                                            @click="toggleFieldHelp('ShortSummary')"
                                            @focus="openFieldHelp('ShortSummary')"
                                            @blur="closeFieldHelp('ShortSummary')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('ShortSummary')"
                                         :id="getFieldHelpPopoverId('ShortSummary')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('ShortSummary') }}
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <textarea class="form-control" rows="3" v-model="model.ShortSummary"></textarea>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="fullDescriptionEditor">Full Description</label>
                            <MinimalRichTextEditor v-model="model.FullDescription"
                                                   placeholder="Add the full event description. Use simple formatting only."
                                                   help-text="Recommended: paragraphs, bold, italic, lists, and links only."
                                                   :invalid="!!getVisibleFieldError('FullDescription')"
                                                   :error-text="getVisibleFieldError('FullDescription')"
                                                   @blur="markTouched('FullDescription')" />
                        </div>

                        <div class="row g-3">
                            <div class="col-md-6">
                                <div class="d-flex align-items-center gap-2 mb-1">
                                    <label class="form-label mb-0">Start</label>

                                    <div class="position-relative d-inline-flex"
                                         @mouseenter="openFieldHelp('StartDateTimeLocalText')"
                                         @mouseleave="closeFieldHelp('StartDateTimeLocalText')">
                                        <button type="button"
                                                class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                                :aria-expanded="isFieldHelpOpen('StartDateTimeLocalText') ? 'true' : 'false'"
                                                :aria-controls="getFieldHelpPopoverId('StartDateTimeLocalText')"
                                                aria-label="Start field guidance"
                                                @click="toggleFieldHelp('StartDateTimeLocalText')"
                                                @focus="openFieldHelp('StartDateTimeLocalText')"
                                                @blur="closeFieldHelp('StartDateTimeLocalText')"
                                                @keydown.esc.prevent="closeAllFieldHelp">
                                            <span aria-hidden="true">?</span>
                                        </button>

                                        <div v-if="isFieldHelpOpen('StartDateTimeLocalText')"
                                             :id="getFieldHelpPopoverId('StartDateTimeLocalText')"
                                             class="card shadow-sm image-help-popover"
                                             role="tooltip">
                                            <div class="card-body p-3 small">
                                                {{ getFieldHelpText('StartDateTimeLocalText') }}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <input type="datetime-local"
                                       class="form-control"
                                       :class="getInputClass('StartDateTimeLocalText')"
                                       v-model="model.StartDateTimeLocalText"
                                       @blur="markTouched('StartDateTimeLocalText')" />
                                <div class="invalid-feedback d-block" v-if="getVisibleFieldError('StartDateTimeLocalText')">
                                    {{ getVisibleFieldError('StartDateTimeLocalText') }}
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="d-flex align-items-center gap-2 mb-1">
                                    <label class="form-label mb-0">End</label>

                                    <div class="position-relative d-inline-flex"
                                         @mouseenter="openFieldHelp('EndDateTimeLocalText')"
                                         @mouseleave="closeFieldHelp('EndDateTimeLocalText')">
                                        <button type="button"
                                                class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                                :aria-expanded="isFieldHelpOpen('EndDateTimeLocalText') ? 'true' : 'false'"
                                                :aria-controls="getFieldHelpPopoverId('EndDateTimeLocalText')"
                                                aria-label="End field guidance"
                                                @click="toggleFieldHelp('EndDateTimeLocalText')"
                                                @focus="openFieldHelp('EndDateTimeLocalText')"
                                                @blur="closeFieldHelp('EndDateTimeLocalText')"
                                                @keydown.esc.prevent="closeAllFieldHelp">
                                            <span aria-hidden="true">?</span>
                                        </button>

                                        <div v-if="isFieldHelpOpen('EndDateTimeLocalText')"
                                             :id="getFieldHelpPopoverId('EndDateTimeLocalText')"
                                             class="card shadow-sm image-help-popover"
                                             role="tooltip">
                                            <div class="card-body p-3 small">
                                                {{ getFieldHelpText('EndDateTimeLocalText') }}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <input type="datetime-local"
                                       class="form-control"
                                       :class="getInputClass('EndDateTimeLocalText')"
                                       v-model="model.EndDateTimeLocalText"
                                       @blur="markTouched('EndDateTimeLocalText')" />
                                <div class="invalid-feedback d-block" v-if="getVisibleFieldError('EndDateTimeLocalText')">
                                    {{ getVisibleFieldError('EndDateTimeLocalText') }}
                                </div>
                            </div>
                        </div>

                        <div class="row g-3 mt-1">
                            <div class="col-md-6">
                                <label class="form-label" for="eventTimeZoneId">Time Zone</label>
                                <select id="eventTimeZoneId"
                                        class="form-select"
                                        :class="getInputClass('TimeZoneId')"
                                        v-model="model.TimeZoneId"
                                        @blur="markTouched('TimeZoneId')">
                                    <option value="">Select a time zone...</option>
                                    <option v-for="tz in config.TimeZoneOptions"
                                            :key="tz.TimeZoneId"
                                            :value="tz.TimeZoneId">
                                        {{ tz.DisplayName }}
                                    </option>
                                </select>
                                <div class="form-text">
                                    Choose the time zone the event should use for its start and end times.
                                </div>
                                <div class="invalid-feedback d-block" v-if="getVisibleFieldError('TimeZoneId')">
                                    {{ getVisibleFieldError('TimeZoneId') }}
                                </div>
                            </div>
                            <div class="col-md-6 d-flex align-items-end">
                                <div class="form-check d-flex align-items-center gap-2">
                                    <input class="form-check-input" type="checkbox" id="isAllDay" v-model="model.IsAllDay" />
                                    <label class="form-check-label" for="isAllDay">All Day</label>

                                    <div class="position-relative d-inline-flex"
                                         @mouseenter="openFieldHelp('IsAllDay')"
                                         @mouseleave="closeFieldHelp('IsAllDay')">
                                        <button type="button"
                                                class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                                :aria-expanded="isFieldHelpOpen('IsAllDay') ? 'true' : 'false'"
                                                :aria-controls="getFieldHelpPopoverId('IsAllDay')"
                                                aria-label="All Day field guidance"
                                                @click="toggleFieldHelp('IsAllDay')"
                                                @focus="openFieldHelp('IsAllDay')"
                                                @blur="closeFieldHelp('IsAllDay')"
                                                @keydown.esc.prevent="closeAllFieldHelp">
                                            <span aria-hidden="true">?</span>
                                        </button>

                                        <div v-if="isFieldHelpOpen('IsAllDay')"
                                             :id="getFieldHelpPopoverId('IsAllDay')"
                                             class="card shadow-sm image-help-popover"
                                             role="tooltip">
                                            <div class="card-body p-3 small">
                                                {{ getFieldHelpText('IsAllDay') }}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <hr class="my-4" />

                        <h3 class="h5 mb-3">Classification</h3>
                        <p class="text-muted small mb-3">
                            Organize the event by owner, category, and tags for filtering and display.
                        </p>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Organizer</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('OrganizerUserId')"
                                     @mouseleave="closeFieldHelp('OrganizerUserId')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('OrganizerUserId') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('OrganizerUserId')"
                                            aria-label="Organizer field guidance"
                                            @click="toggleFieldHelp('OrganizerUserId')"
                                            @focus="openFieldHelp('OrganizerUserId')"
                                            @blur="closeFieldHelp('OrganizerUserId')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('OrganizerUserId')"
                                         :id="getFieldHelpPopoverId('OrganizerUserId')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('OrganizerUserId') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <select class="form-select"
                                    :class="getInputClass('OrganizerUserId')"
                                    v-model.number="model.OrganizerUserId"
                                    @change="markTouched('OrganizerUserId')">
                                <option :value="null">Select organizer</option>
                                <option v-for="user in config.Organizers" :key="user.UserId" :value="user.UserId">
                                    {{ user.DisplayName }}{{ user.Email ? ` (${user.Email})` : '' }}
                                </option>
                            </select>
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('OrganizerUserId')">
                                {{ getVisibleFieldError('OrganizerUserId') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Category</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('EventCategoryId')"
                                     @mouseleave="closeFieldHelp('EventCategoryId')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('EventCategoryId') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('EventCategoryId')"
                                            aria-label="Category field guidance"
                                            @click="toggleFieldHelp('EventCategoryId')"
                                            @focus="openFieldHelp('EventCategoryId')"
                                            @blur="closeFieldHelp('EventCategoryId')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('EventCategoryId')"
                                         :id="getFieldHelpPopoverId('EventCategoryId')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('EventCategoryId') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="input-group has-validation">
                                <select class="form-select"
                                        :class="getInputClass('EventCategoryId')"
                                        v-model.number="model.EventCategoryId"
                                        @change="markTouched('EventCategoryId')">
                                    <option :value="null">Select category</option>
                                    <option v-for="item in config.Categories" :key="item.EventCategoryId" :value="item.EventCategoryId">
                                        {{ item.CategoryName }}
                                    </option>
                                </select>
                                <button type="button" class="btn btn-outline-secondary" @click="openCategoryModal">Add</button>
                            </div>

                            <div class="alert alert-warning py-2 px-3 mt-2 mb-0" v-if="showCategoryGuidance">
                                This portal currently has {{ categoryCount }} categories. It is ideal to keep the total number of categories between 3 and 5.
                            </div>

                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('EventCategoryId')">
                                {{ getVisibleFieldError('EventCategoryId') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Tags</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('TagIds')"
                                     @mouseleave="closeFieldHelp('TagIds')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('TagIds') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('TagIds')"
                                            aria-label="Tags field guidance"
                                            @click="toggleFieldHelp('TagIds')"
                                            @focus="openFieldHelp('TagIds')"
                                            @blur="closeFieldHelp('TagIds')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('TagIds')"
                                         :id="getFieldHelpPopoverId('TagIds')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('TagIds') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="input-group">
                                <select class="form-select"
                                        multiple
                                        size="6"
                                        v-model="model.TagIds"
                                        @change="markTouched('TagIds')">
                                    <option v-for="tag in config.Tags" :key="tag.TagId" :value="tag.TagId">
                                        {{ tag.TagName }}
                                    </option>
                                </select>
                                <button type="button" class="btn btn-outline-secondary" @click="openTagModal">Add</button>
                            </div>

                            <div class="form-text">
                                Hold Ctrl or Command to select multiple tags.
                            </div>

                            <div class="alert alert-warning py-2 px-3 mt-2 mb-0" v-if="showTagCountWarning">
                                Choosing more than 5 tags is not advised.
                            </div>

                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('TagIds')">
                                {{ getVisibleFieldError('TagIds') }}
                            </div>
                        </div>

                        <hr class="my-4" />

                        <h3 class="h5 mb-3">Venue</h3>

                        <div class="mb-3 position-relative">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Venue Name</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('VenueName')"
                                     @mouseleave="closeFieldHelp('VenueName')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('VenueName') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('VenueName')"
                                            aria-label="Venue name field guidance"
                                            @click="toggleFieldHelp('VenueName')"
                                            @focus="openFieldHelp('VenueName')"
                                            @blur="closeFieldHelp('VenueName')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('VenueName')"
                                         :id="getFieldHelpPopoverId('VenueName')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('VenueName') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <input class="form-control"
                                   v-model="model.VenueName"
                                   autocomplete="off"
                                   @input="onVenueAutocompleteInput('VenueName')"
                                   @focus="onVenueAutocompleteFocus('VenueName')"
                                   @blur="scheduleVenueAutocompleteClose" />

                            <div class="dropdown-menu d-block w-100 shadow-sm"
                                 v-if="venueAutocomplete.IsOpen && venueAutocomplete.ActiveField === 'VenueName'"
                                 @mousedown.prevent="cancelVenueAutocompleteClose">

                                <div class="dropdown-item-text text-muted small" v-if="venueAutocomplete.IsLoading">
                                    Loading suggestions...
                                </div>

                                <div class="dropdown-item-text text-danger small" v-else-if="venueAutocomplete.Error">
                                    {{ venueAutocomplete.Error }}
                                </div>

                                <button type="button"
                                        class="dropdown-item text-wrap"
                                        v-for="(item, index) in venueAutocomplete.Results"
                                        :key="`venue-name-suggestion-${index}`"
                                        @mousedown.prevent="selectVenueSuggestion(item)">
                                    <div class="fw-semibold">{{ item.VenueName || item.DisplayName }}</div>
                                    <div class="small text-muted">{{ item.DisplayName }}</div>
                                </button>

                                <div class="dropdown-item-text text-muted small"
                                     v-if="!venueAutocomplete.IsLoading && !venueAutocomplete.Error && venueAutocomplete.Results.length === 0">
                                    No suggestions found.
                                </div>
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Public Location Text</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('PublicLocationText')"
                                     @mouseleave="closeFieldHelp('PublicLocationText')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('PublicLocationText') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('PublicLocationText')"
                                            aria-label="Public location text field guidance"
                                            @click="toggleFieldHelp('PublicLocationText')"
                                            @focus="openFieldHelp('PublicLocationText')"
                                            @blur="closeFieldHelp('PublicLocationText')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('PublicLocationText')"
                                         :id="getFieldHelpPopoverId('PublicLocationText')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('PublicLocationText') }}
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <input class="form-control" v-model="model.PublicLocationText" />
                        </div>

                        <div class="row g-3">
                            <div class="col-md-6 position-relative">
                                <div class="d-flex align-items-center gap-2 mb-1">
                                    <label class="form-label mb-0">Address Line 1</label>

                                    <div class="position-relative d-inline-flex"
                                         @mouseenter="openFieldHelp('AddressLine1')"
                                         @mouseleave="closeFieldHelp('AddressLine1')">
                                        <button type="button"
                                                class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                                :aria-expanded="isFieldHelpOpen('AddressLine1') ? 'true' : 'false'"
                                                :aria-controls="getFieldHelpPopoverId('AddressLine1')"
                                                aria-label="Address Line 1 field guidance"
                                                @click="toggleFieldHelp('AddressLine1')"
                                                @focus="openFieldHelp('AddressLine1')"
                                                @blur="closeFieldHelp('AddressLine1')"
                                                @keydown.esc.prevent="closeAllFieldHelp">
                                            <span aria-hidden="true">?</span>
                                        </button>

                                        <div v-if="isFieldHelpOpen('AddressLine1')"
                                             :id="getFieldHelpPopoverId('AddressLine1')"
                                             class="card shadow-sm image-help-popover"
                                             role="tooltip">
                                            <div class="card-body p-3 small">
                                                {{ getFieldHelpText('AddressLine1') }}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <input class="form-control"
                                       v-model="model.AddressLine1"
                                       autocomplete="off"
                                       @input="onVenueAutocompleteInput('AddressLine1')"
                                       @focus="onVenueAutocompleteFocus('AddressLine1')"
                                       @blur="scheduleVenueAutocompleteClose" />

                                <div class="dropdown-menu d-block w-100 shadow-sm"
                                     v-if="venueAutocomplete.IsOpen && venueAutocomplete.ActiveField === 'AddressLine1'"
                                     @mousedown.prevent="cancelVenueAutocompleteClose">

                                    <div class="dropdown-item-text text-muted small" v-if="venueAutocomplete.IsLoading">
                                        Loading suggestions...
                                    </div>

                                    <div class="dropdown-item-text text-danger small" v-else-if="venueAutocomplete.Error">
                                        {{ venueAutocomplete.Error }}
                                    </div>

                                    <button type="button"
                                            class="dropdown-item text-wrap"
                                            v-for="(item, index) in venueAutocomplete.Results"
                                            :key="`address-line1-suggestion-${index}`"
                                            @mousedown.prevent="selectVenueSuggestion(item)">
                                        <div class="fw-semibold">{{ item.VenueName || item.AddressLine1 || item.DisplayName }}</div>
                                        <div class="small text-muted">{{ item.DisplayName }}</div>
                                    </button>

                                    <div class="dropdown-item-text text-muted small"
                                         v-if="!venueAutocomplete.IsLoading && !venueAutocomplete.Error && venueAutocomplete.Results.length === 0">
                                        No suggestions found.
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">Address Line 2</label>
                                <input class="form-control" v-model="model.AddressLine2" />
                            </div>
                        </div>

                        <div class="row g-3 mt-1">
                            <div class="col-md-4">
                                <label class="form-label">City</label>
                                <input class="form-control" v-model="model.City" />
                            </div>
                            <div class="col-md-4">
                                <div class="d-flex align-items-center gap-2 mb-1">
                                    <label class="form-label mb-0">Region</label>

                                    <div class="position-relative d-inline-flex"
                                         @mouseenter="openFieldHelp('Region')"
                                         @mouseleave="closeFieldHelp('Region')">
                                        <button type="button"
                                                class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                                :aria-expanded="isFieldHelpOpen('Region') ? 'true' : 'false'"
                                                :aria-controls="getFieldHelpPopoverId('Region')"
                                                aria-label="Region field guidance"
                                                @click="toggleFieldHelp('Region')"
                                                @focus="openFieldHelp('Region')"
                                                @blur="closeFieldHelp('Region')"
                                                @keydown.esc.prevent="closeAllFieldHelp">
                                            <span aria-hidden="true">?</span>
                                        </button>

                                        <div v-if="isFieldHelpOpen('Region')"
                                             :id="getFieldHelpPopoverId('Region')"
                                             class="card shadow-sm image-help-popover"
                                             role="tooltip">
                                            <div class="card-body p-3 small">
                                                {{ getFieldHelpText('Region') }}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <input class="form-control" v-model="model.Region" />
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Postal Code</label>
                                <input class="form-control" v-model="model.PostalCode" />
                            </div>
                        </div>

                        <div class="row g-3 mt-1">
                            <div class="col-md-4">
                                <div class="d-flex align-items-center gap-2 mb-1">
                                    <label class="form-label mb-0">Country Code</label>

                                    <div class="position-relative d-inline-flex"
                                         @mouseenter="openFieldHelp('CountryCode')"
                                         @mouseleave="closeFieldHelp('CountryCode')">
                                        <button type="button"
                                                class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                                :aria-expanded="isFieldHelpOpen('CountryCode') ? 'true' : 'false'"
                                                :aria-controls="getFieldHelpPopoverId('CountryCode')"
                                                aria-label="Country code field guidance"
                                                @click="toggleFieldHelp('CountryCode')"
                                                @focus="openFieldHelp('CountryCode')"
                                                @blur="closeFieldHelp('CountryCode')"
                                                @keydown.esc.prevent="closeAllFieldHelp">
                                            <span aria-hidden="true">?</span>
                                        </button>

                                        <div v-if="isFieldHelpOpen('CountryCode')"
                                             :id="getFieldHelpPopoverId('CountryCode')"
                                             class="card shadow-sm image-help-popover"
                                             role="tooltip">
                                            <div class="card-body p-3 small">
                                                {{ getFieldHelpText('CountryCode') }}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <input class="form-control" v-model="model.CountryCode" />
                            </div>
                            <div class="col-md-4">
                                <div class="d-flex align-items-center gap-2 mb-1">
                                    <label class="form-label mb-0">Latitude</label>

                                    <div class="position-relative d-inline-flex"
                                         @mouseenter="openFieldHelp('Latitude')"
                                         @mouseleave="closeFieldHelp('Latitude')">
                                        <button type="button"
                                                class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                                :aria-expanded="isFieldHelpOpen('Latitude') ? 'true' : 'false'"
                                                :aria-controls="getFieldHelpPopoverId('Latitude')"
                                                aria-label="Latitude field guidance"
                                                @click="toggleFieldHelp('Latitude')"
                                                @focus="openFieldHelp('Latitude')"
                                                @blur="closeFieldHelp('Latitude')"
                                                @keydown.esc.prevent="closeAllFieldHelp">
                                            <span aria-hidden="true">?</span>
                                        </button>

                                        <div v-if="isFieldHelpOpen('Latitude')"
                                             :id="getFieldHelpPopoverId('Latitude')"
                                             class="card shadow-sm image-help-popover"
                                             role="tooltip">
                                            <div class="card-body p-3 small">
                                                {{ getFieldHelpText('Latitude') }}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <input class="form-control"
                                       :class="getInputClass('Latitude')"
                                       v-model.number="model.Latitude"
                                       @blur="markTouched('Latitude')" />
                                <div class="invalid-feedback d-block" v-if="getVisibleFieldError('Latitude')">
                                    {{ getVisibleFieldError('Latitude') }}
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="d-flex align-items-center gap-2 mb-1">
                                    <label class="form-label mb-0">Longitude</label>

                                    <div class="position-relative d-inline-flex"
                                         @mouseenter="openFieldHelp('Longitude')"
                                         @mouseleave="closeFieldHelp('Longitude')">
                                        <button type="button"
                                                class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                                :aria-expanded="isFieldHelpOpen('Longitude') ? 'true' : 'false'"
                                                :aria-controls="getFieldHelpPopoverId('Longitude')"
                                                aria-label="Longitude field guidance"
                                                @click="toggleFieldHelp('Longitude')"
                                                @focus="openFieldHelp('Longitude')"
                                                @blur="closeFieldHelp('Longitude')"
                                                @keydown.esc.prevent="closeAllFieldHelp">
                                            <span aria-hidden="true">?</span>
                                        </button>

                                        <div v-if="isFieldHelpOpen('Longitude')"
                                             :id="getFieldHelpPopoverId('Longitude')"
                                             class="card shadow-sm image-help-popover"
                                             role="tooltip">
                                            <div class="card-body p-3 small">
                                                {{ getFieldHelpText('Longitude') }}
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <input class="form-control"
                                       :class="getInputClass('Longitude')"
                                       v-model.number="model.Longitude"
                                       @blur="markTouched('Longitude')" />
                                <div class="invalid-feedback d-block" v-if="getVisibleFieldError('Longitude')">
                                    {{ getVisibleFieldError('Longitude') }}
                                </div>
                            </div>
                        </div>

                        <hr class="my-4" />

                        <h3 class="h5 mb-3">Calls to Action</h3>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Registration URL</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('RegistrationUrl')"
                                     @mouseleave="closeFieldHelp('RegistrationUrl')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('RegistrationUrl') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('RegistrationUrl')"
                                            aria-label="Registration URL field guidance"
                                            @click="toggleFieldHelp('RegistrationUrl')"
                                            @focus="openFieldHelp('RegistrationUrl')"
                                            @blur="closeFieldHelp('RegistrationUrl')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('RegistrationUrl')"
                                         :id="getFieldHelpPopoverId('RegistrationUrl')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('RegistrationUrl') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <input class="form-control"
                                   :class="getInputClass('RegistrationUrl')"
                                   v-model="model.RegistrationUrl"
                                   @blur="markTouched('RegistrationUrl')" />
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('RegistrationUrl')">
                                {{ getVisibleFieldError('RegistrationUrl') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Registration Button Text</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('RegistrationButtonText')"
                                     @mouseleave="closeFieldHelp('RegistrationButtonText')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('RegistrationButtonText') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('RegistrationButtonText')"
                                            aria-label="Registration button text field guidance"
                                            @click="toggleFieldHelp('RegistrationButtonText')"
                                            @focus="openFieldHelp('RegistrationButtonText')"
                                            @blur="closeFieldHelp('RegistrationButtonText')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('RegistrationButtonText')"
                                         :id="getFieldHelpPopoverId('RegistrationButtonText')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('RegistrationButtonText') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <input class="form-control"
                                   :class="getInputClass('RegistrationButtonText')"
                                   v-model="model.RegistrationButtonText"
                                   @blur="markTouched('RegistrationButtonText')" />
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('RegistrationButtonText')">
                                {{ getVisibleFieldError('RegistrationButtonText') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Secondary CTA URL</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('SecondaryCtaUrl')"
                                     @mouseleave="closeFieldHelp('SecondaryCtaUrl')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('SecondaryCtaUrl') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('SecondaryCtaUrl')"
                                            aria-label="Secondary CTA URL field guidance"
                                            @click="toggleFieldHelp('SecondaryCtaUrl')"
                                            @focus="openFieldHelp('SecondaryCtaUrl')"
                                            @blur="closeFieldHelp('SecondaryCtaUrl')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('SecondaryCtaUrl')"
                                         :id="getFieldHelpPopoverId('SecondaryCtaUrl')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('SecondaryCtaUrl') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <input class="form-control"
                                   :class="getInputClass('SecondaryCtaUrl')"
                                   v-model="model.SecondaryCtaUrl"
                                   @blur="markTouched('SecondaryCtaUrl')" />
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('SecondaryCtaUrl')">
                                {{ getVisibleFieldError('SecondaryCtaUrl') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Secondary CTA Text</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('SecondaryCtaText')"
                                     @mouseleave="closeFieldHelp('SecondaryCtaText')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('SecondaryCtaText') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('SecondaryCtaText')"
                                            aria-label="Secondary CTA text field guidance"
                                            @click="toggleFieldHelp('SecondaryCtaText')"
                                            @focus="openFieldHelp('SecondaryCtaText')"
                                            @blur="closeFieldHelp('SecondaryCtaText')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('SecondaryCtaText')"
                                         :id="getFieldHelpPopoverId('SecondaryCtaText')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('SecondaryCtaText') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <input class="form-control"
                                   :class="getInputClass('SecondaryCtaText')"
                                   v-model="model.SecondaryCtaText"
                                   @blur="markTouched('SecondaryCtaText')" />
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('SecondaryCtaText')">
                                {{ getVisibleFieldError('SecondaryCtaText') }}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="col-12 col-lg-4">

                <div class="card shadow-sm border-0 mb-4">
                    <div class="card-body px-4">
                        <h3 class="h5 mb-3">Flags</h3>

                        <div class="mb-3">
                            <label class="form-label">Status</label>
                            <select class="form-select" v-model.number="model.EventStatusId">
                                <option :value="1">Draft</option>
                                <option :value="2">Published</option>
                                <option :value="3">Archived</option>
                                <option :value="4">Cancelled</option>
                            </select>
                        </div>

                        <div class="form-check mb-2">
                            <input class="form-check-input" type="checkbox" id="allowPublic" v-model="model.AllowPublicDetailPage" />
                            <label class="form-check-label" for="allowPublic">Allow Public Detail Page</label>
                        </div>

                        <div class="form-check mb-2">
                            <input class="form-check-input" type="checkbox" id="featured" v-model="model.IsFeatured" />
                            <label class="form-check-label" for="featured">Featured</label>
                        </div>

                        <div class="form-check">
                            <input class="form-check-input" type="checkbox" id="free" v-model="model.IsFree" />
                            <label class="form-check-label" for="free">Free Event</label>
                        </div>
                    </div>
                </div>

                <div class="card shadow-sm border-0 mb-4">
                    <div class="card-body px-4">
                        <h3 class="h5 mb-3">SEO</h3>
                        <p class="text-muted small mb-3">
                            These fields help search engines and AI-driven answer engines understand the event.
                        </p>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">SEO Title</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('SeoTitle')"
                                     @mouseleave="closeFieldHelp('SeoTitle')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('SeoTitle') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('SeoTitle')"
                                            aria-label="SEO title field guidance"
                                            @click="toggleFieldHelp('SeoTitle')"
                                            @focus="openFieldHelp('SeoTitle')"
                                            @blur="closeFieldHelp('SeoTitle')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('SeoTitle')"
                                         :id="getFieldHelpPopoverId('SeoTitle')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('SeoTitle') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <input class="form-control" v-model="model.SeoTitle" />
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">SEO Description</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('SeoDescription')"
                                     @mouseleave="closeFieldHelp('SeoDescription')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('SeoDescription') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('SeoDescription')"
                                            aria-label="SEO description field guidance"
                                            @click="toggleFieldHelp('SeoDescription')"
                                            @focus="openFieldHelp('SeoDescription')"
                                            @blur="closeFieldHelp('SeoDescription')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('SeoDescription')"
                                         :id="getFieldHelpPopoverId('SeoDescription')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('SeoDescription') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <textarea class="form-control" rows="4" v-model="model.SeoDescription"></textarea>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Canonical URL</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('CanonicalUrl')"
                                     @mouseleave="closeFieldHelp('CanonicalUrl')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('CanonicalUrl') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('CanonicalUrl')"
                                            aria-label="Canonical URL field guidance"
                                            @click="toggleFieldHelp('CanonicalUrl')"
                                            @focus="openFieldHelp('CanonicalUrl')"
                                            @blur="closeFieldHelp('CanonicalUrl')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('CanonicalUrl')"
                                         :id="getFieldHelpPopoverId('CanonicalUrl')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('CanonicalUrl') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <input class="form-control"
                                   :class="getInputClass('CanonicalUrl')"
                                   v-model="model.CanonicalUrl"
                                   @blur="markTouched('CanonicalUrl')" />
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('CanonicalUrl')">
                                {{ getVisibleFieldError('CanonicalUrl') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0">Meta Robots</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('MetaRobots')"
                                     @mouseleave="closeFieldHelp('MetaRobots')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('MetaRobots') ? 'true' : 'false'"
                                            :aria-controls="getFieldHelpPopoverId('MetaRobots')"
                                            aria-label="Meta robots field guidance"
                                            @click="toggleFieldHelp('MetaRobots')"
                                            @focus="openFieldHelp('MetaRobots')"
                                            @blur="closeFieldHelp('MetaRobots')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('MetaRobots')"
                                         :id="getFieldHelpPopoverId('MetaRobots')"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            {{ getFieldHelpText('MetaRobots') }}
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <input class="form-control" v-model="model.MetaRobots" />
                        </div>
                    </div>
                </div>

                <div class="card shadow-sm border-0 mb-4">
                    <div class="card-body px-4">
                        <h3 class="h5 mb-3">Files &amp; Images</h3>
                        <p class="text-muted small mb-3">
                            Use image and file references to help promote the event.
                        </p>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0" for="heroImageUrl">Hero Image URL</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('HeroImageUrl')"
                                     @mouseleave="closeFieldHelp('HeroImageUrl')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('HeroImageUrl') ? 'true' : 'false'"
                                            aria-controls="heroImageUrlHelpPopover"
                                            aria-label="Hero image field guidance"
                                            @click="toggleFieldHelp('HeroImageUrl')"
                                            @focus="openFieldHelp('HeroImageUrl')"
                                            @blur="closeFieldHelp('HeroImageUrl')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('HeroImageUrl')"
                                         id="heroImageUrlHelpPopover"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            Used on the event detail page hero area. Best results come from a wide landscape image that can survive center-cropping. Recommended aspect ratio: 16:9 or similar. Recommended size: 1600×900 or larger. Minimum practical size: 1200×675. Avoid text near the edges because object-fit: cover may crop it.
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <input id="heroImageUrl"
                                   class="form-control"
                                   :class="getInputClass('HeroImageUrl')"
                                   v-model="model.HeroImageUrl"
                                   @blur="markTouched('HeroImageUrl')"
                                   aria-describedby="heroImageUrlError" />
                            <div id="heroImageUrlError" class="invalid-feedback d-block" v-if="getVisibleFieldError('HeroImageUrl')">
                                {{ getVisibleFieldError('HeroImageUrl') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0" for="listImageUrl">List Image URL</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('ListImageUrl')"
                                     @mouseleave="closeFieldHelp('ListImageUrl')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('ListImageUrl') ? 'true' : 'false'"
                                            aria-controls="listImageUrlHelpPopover"
                                            aria-label="List image field guidance"
                                            @click="toggleFieldHelp('ListImageUrl')"
                                            @focus="openFieldHelp('ListImageUrl')"
                                            @blur="closeFieldHelp('ListImageUrl')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('ListImageUrl')"
                                         id="listImageUrlHelpPopover"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            Used on event list and card views. This image is cropped with object-fit: cover inside a card area, so use a landscape image with the main subject centered. Recommended aspect ratio: 16:9 or 3:2. Recommended size: 1200×675 or larger. Minimum practical size: 800×450.
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <input id="listImageUrl"
                                   class="form-control"
                                   :class="getInputClass('ListImageUrl')"
                                   v-model="model.ListImageUrl"
                                   @blur="markTouched('ListImageUrl')"
                                   aria-describedby="listImageUrlError" />
                            <div id="listImageUrlError" class="invalid-feedback d-block" v-if="getVisibleFieldError('ListImageUrl')">
                                {{ getVisibleFieldError('ListImageUrl') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <div class="d-flex align-items-center gap-2 mb-1">
                                <label class="form-label mb-0" for="thumbnailImageUrl">Thumbnail Image URL</label>

                                <div class="position-relative d-inline-flex"
                                     @mouseenter="openFieldHelp('ThumbnailImageUrl')"
                                     @mouseleave="closeFieldHelp('ThumbnailImageUrl')">
                                    <button type="button"
                                            class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                            :aria-expanded="isFieldHelpOpen('ThumbnailImageUrl') ? 'true' : 'false'"
                                            aria-controls="thumbnailImageUrlHelpPopover"
                                            aria-label="Thumbnail image field guidance"
                                            @click="toggleFieldHelp('ThumbnailImageUrl')"
                                            @focus="openFieldHelp('ThumbnailImageUrl')"
                                            @blur="closeFieldHelp('ThumbnailImageUrl')"
                                            @keydown.esc.prevent="closeAllFieldHelp">
                                        <span aria-hidden="true">?</span>
                                    </button>

                                    <div v-if="isFieldHelpOpen('ThumbnailImageUrl')"
                                         id="thumbnailImageUrlHelpPopover"
                                         class="card shadow-sm image-help-popover"
                                         role="tooltip">
                                        <div class="card-body p-3 small">
                                            Used for smaller preview contexts and may also be used as a fallback when no list image is provided. Even though this is a thumbnail field, do not use a tiny image. Recommended aspect ratio: 16:9 or 3:2. Recommended size: 800×450 or larger. Use a simple, readable image that still works when cropped and scaled down.
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <input id="thumbnailImageUrl"
                                   class="form-control"
                                   :class="getInputClass('ThumbnailImageUrl')"
                                   v-model="model.ThumbnailImageUrl"
                                   @blur="markTouched('ThumbnailImageUrl')"
                                   aria-describedby="thumbnailImageUrlError" />
                            <div id="thumbnailImageUrlError" class="invalid-feedback d-block" v-if="getVisibleFieldError('ThumbnailImageUrl')">
                                {{ getVisibleFieldError('ThumbnailImageUrl') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="imageAltText">Image Alt Text</label>
                            <input id="imageAltText"
                                   class="form-control"
                                   v-model="model.ImageAltText"
                                   aria-describedby="imageAltTextHelp" />
                        </div>

                        <div class="d-flex align-items-center gap-2 mt-4 mb-2">
                            <h4 class="h6 mb-0">Download File</h4>

                            <div class="position-relative d-inline-flex"
                                 @mouseenter="openFieldHelp('DownloadFileSection')"
                                 @mouseleave="closeFieldHelp('DownloadFileSection')">
                                <button type="button"
                                        class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                        :aria-expanded="isFieldHelpOpen('DownloadFileSection') ? 'true' : 'false'"
                                        :aria-controls="getFieldHelpPopoverId('DownloadFileSection')"
                                        aria-label="Download file section guidance"
                                        @click="toggleFieldHelp('DownloadFileSection')"
                                        @focus="openFieldHelp('DownloadFileSection')"
                                        @blur="closeFieldHelp('DownloadFileSection')"
                                        @keydown.esc.prevent="closeAllFieldHelp">
                                    <span aria-hidden="true">?</span>
                                </button>

                                <div v-if="isFieldHelpOpen('DownloadFileSection')"
                                     :id="getFieldHelpPopoverId('DownloadFileSection')"
                                     class="card shadow-sm image-help-popover"
                                     role="tooltip">
                                    <div class="card-body p-3 small">
                                        {{ getFieldHelpText('DownloadFileSection') }}
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Download File Folder</label>
                            <select class="form-select"
                                    v-model.number="selectedDownloadFolderId"
                                    @change="onDownloadFolderChanged">
                                <option :value="null">Select folder</option>
                                <option v-for="folder in config.Folders" :key="folder.FolderId" :value="folder.FolderId">
                                    {{ folder.FolderName }}
                                </option>
                            </select>
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Download File</label>
                            <select class="form-select"
                                    :class="getInputClass('DownloadFileId')"
                                    v-model.number="model.DownloadFileId"
                                    @change="markTouched('DownloadFileId')"
                                    :disabled="!selectedDownloadFolderId">
                                <option :value="null">Select file</option>
                                <option v-for="file in config.Files" :key="file.FileId" :value="file.FileId">
                                    {{ formatFileOptionLabel(file) }}
                                </option>
                            </select>
                            <div class="form-text" v-if="selectedDownloadFolderId && !(config.Files || []).length">
                                No files were found in the selected folder.
                            </div>
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('DownloadFileId')">
                                {{ getVisibleFieldError('DownloadFileId') }}
                            </div>
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Download File URL</label>
                            <input class="form-control"
                                   :class="getInputClass('DownloadFileUrl')"
                                   v-model="model.DownloadFileUrl"
                                   @blur="markTouched('DownloadFileUrl')" />
                            <div class="invalid-feedback d-block" v-if="getVisibleFieldError('DownloadFileUrl')">
                                {{ getVisibleFieldError('DownloadFileUrl') }}
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card shadow-sm border-0 mb-4">
                    <div class="card-body px-4">
                        <div class="d-flex align-items-center gap-2 mb-3">
                            <h3 class="h5 mb-0">Features and Audiences</h3>

                            <div class="position-relative d-inline-flex"
                                 @mouseenter="openFieldHelp('FeaturesAndAudiencesSection')"
                                 @mouseleave="closeFieldHelp('FeaturesAndAudiencesSection')">
                                <button type="button"
                                        class="btn btn-sm btn-outline-secondary rounded-circle image-help-trigger"
                                        :aria-expanded="isFieldHelpOpen('FeaturesAndAudiencesSection') ? 'true' : 'false'"
                                        :aria-controls="getFieldHelpPopoverId('FeaturesAndAudiencesSection')"
                                        aria-label="Features and Audiences section guidance"
                                        @click="toggleFieldHelp('FeaturesAndAudiencesSection')"
                                        @focus="openFieldHelp('FeaturesAndAudiencesSection')"
                                        @blur="closeFieldHelp('FeaturesAndAudiencesSection')"
                                        @keydown.esc.prevent="closeAllFieldHelp">
                                    <span aria-hidden="true">?</span>
                                </button>

                                <div v-if="isFieldHelpOpen('FeaturesAndAudiencesSection')"
                                     :id="getFieldHelpPopoverId('FeaturesAndAudiencesSection')"
                                     class="card shadow-sm image-help-popover"
                                     role="tooltip">
                                    <div class="card-body p-3 small">
                                        {{ getFieldHelpText('FeaturesAndAudiencesSection') }}
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="mb-3">
                            <button type="button" class="btn btn-outline-secondary btn-sm me-2" @click="addFeature">Add Feature</button>
                            <button type="button" class="btn btn-outline-secondary btn-sm" @click="addAudience">Add Audience</button>
                        </div>

                        <div class="mb-3" v-for="(feature, index) in model.Features" :key="'feature-' + index">
                            <label class="form-label">Feature {{ index + 1 }}</label>
                            <input class="form-control mb-2"
                                   :class="getInputClass(`Feature-${index}`)"
                                   placeholder="Title"
                                   v-model="feature.Title"
                                   @blur="markTouched(`Feature-${index}`)" />
                            <input class="form-control mb-2" placeholder="Icon Class" v-model="feature.IconClass" />
                            <textarea class="form-control mb-2" rows="2" placeholder="Description" v-model="feature.Description"></textarea>
                            <div class="invalid-feedback d-block mb-2" v-if="getVisibleFieldError(`Feature-${index}`)">
                                {{ getVisibleFieldError(`Feature-${index}`) }}
                            </div>
                            <button type="button" class="btn btn-sm btn-outline-danger" @click="removeFeature(index)">Remove</button>
                        </div>

                        <div class="mb-3" v-for="(audience, index) in model.Audiences" :key="'audience-' + index">
                            <label class="form-label">Audience {{ index + 1 }}</label>
                            <input class="form-control mb-2"
                                   :class="getInputClass(`Audience-${index}`)"
                                   placeholder="Title"
                                   v-model="audience.Title"
                                   @blur="markTouched(`Audience-${index}`)" />
                            <textarea class="form-control mb-2" rows="2" placeholder="Description" v-model="audience.Description"></textarea>
                            <div class="invalid-feedback d-block mb-2" v-if="getVisibleFieldError(`Audience-${index}`)">
                                {{ getVisibleFieldError(`Audience-${index}`) }}
                            </div>
                            <button type="button" class="btn btn-sm btn-outline-danger" @click="removeAudience(index)">Remove</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div v-if="categoryModal.IsOpen"
         class="modal fade show d-block"
         tabindex="-1"
         role="dialog"
         aria-modal="true"
         aria-labelledby="eventCategoryModalLabel"
         @click.self="closeCategoryModal">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content shadow">
                <div class="modal-header">
                    <h5 class="modal-title" id="eventCategoryModalLabel">Add Category</h5>
                    <button type="button" class="btn-close" aria-label="Close" @click="closeCategoryModal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="newCategoryName">Category Name</label>
                        <input id="newCategoryName"
                               class="form-control"
                               v-model="categoryModal.Name"
                               :disabled="categoryModal.IsSaving"
                               maxlength="200"
                               autofocus
                               @keydown.enter.prevent="submitCategoryModal" />
                    </div>

                    <div class="alert alert-danger mb-0" v-if="categoryModal.Error">
                        {{ categoryModal.Error }}
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-secondary" :disabled="categoryModal.IsSaving" @click="closeCategoryModal">
                        Cancel
                    </button>
                    <button type="button" class="btn btn-primary" :disabled="categoryModal.IsSaving" @click="submitCategoryModal">
                        {{ categoryModal.IsSaving ? 'Saving...' : 'Add Category' }}
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div v-if="categoryModal.IsOpen" class="modal-backdrop fade show"></div>

    <div v-if="tagModal.IsOpen"
         class="modal fade show d-block"
         tabindex="-1"
         role="dialog"
         aria-modal="true"
         aria-labelledby="eventTagModalLabel"
         @click.self="closeTagModal">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content shadow">
                <div class="modal-header">
                    <h5 class="modal-title" id="eventTagModalLabel">Add Tag</h5>
                    <button type="button" class="btn-close" aria-label="Close" @click="closeTagModal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="newTagName">Tag Name</label>
                        <input id="newTagName"
                               class="form-control"
                               v-model="tagModal.Name"
                               :disabled="tagModal.IsSaving"
                               maxlength="200"
                               autofocus
                               @keydown.enter.prevent="submitTagModal" />
                    </div>

                    <div class="alert alert-danger mb-0" v-if="tagModal.Error">
                        {{ tagModal.Error }}
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-secondary" :disabled="tagModal.IsSaving" @click="closeTagModal">
                        Cancel
                    </button>
                    <button type="button" class="btn btn-primary" :disabled="tagModal.IsSaving" @click="submitTagModal">
                        {{ tagModal.IsSaving ? 'Saving...' : 'Add Tag' }}
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div v-if="tagModal.IsOpen" class="modal-backdrop fade show"></div>

</template>

<script setup>
    import { computed, inject, onBeforeUnmount, onMounted, reactive, ref, watch } from 'vue';
    import { checkSlugAvailability, createCategory, createTag, getConfig, getEventForEdit, getFileById, getFilesByFolder, getVenueSuggestions, saveEvent } from '../assets/api';
    import MinimalRichTextEditor from '../components/MinimalRichTextEditor.vue';

    const dnnConfig = inject('dnnConfig');

    function createDefaultModel() {
        return {
            EventId: null,
            PortalId: 0,
            EventStatusId: 2,
            OrganizerUserId: null,
            VenueId: null,
            EventCategoryId: null,
            Title: '',
            Slug: '',
            ShortSummary: '',
            FullDescription: '',
            StartDateTimeLocalText: '',
            EndDateTimeLocalText: '',
            TimeZoneId: '',
            IsAllDay: false,
            DisplayDateText: '',
            DisplayTimeText: '',
            HeroImageUrl: '',
            ListImageUrl: '',
            ThumbnailImageUrl: '',
            ImageAltText: '',
            RegistrationUrl: '',
            RegistrationButtonText: '',
            SecondaryCtaUrl: '',
            SecondaryCtaText: '',
            PriceAmount: null,
            PriceCurrencyCode: 'USD',
            PriceLabel: '',
            IsFree: false,
            Capacity: null,
            CapacitySummaryText: '',
            ContactEmail: '',
            ContactPhone: '',
            ContactUrl: '',
            SeoTitle: '',
            SeoDescription: '',
            CanonicalUrl: '',
            MetaRobots: '',
            SortOrder: 0,
            IsFeatured: false,
            AllowPublicDetailPage: true,
            PublishStartLocalText: '',
            PublishEndLocalText: '',
            DownloadFileId: null,
            DownloadFileUrl: '',
            UseCustomLocationText: true,
            VenueName: '',
            PublicLocationText: '',
            AddressLine1: '',
            AddressLine2: '',
            City: '',
            Region: '',
            PostalCode: '',
            CountryCode: '',
            Latitude: null,
            Longitude: null,
            VenueWebsiteUrl: '',
            VenuePhoneNumber: '',
            TagIds: [],
            Features: [],
            Audiences: []
        };
    }

    const config = reactive({
        Portals: [],
        Categories: [],
        Tags: [],
        Organizers: [],
        Folders: [],
        Files: [],
        TimeZoneOptions: [],
        DefaultTimeZoneId: '',
        IsSuperUser: false
    });

    const model = reactive(createDefaultModel());

    const saveMessage = ref('');
    const isEditMode = computed(() => !!model.EventId);
    const globalError = ref('');
    const isSaving = ref(false);
    const isCheckingSlug = ref(false);
    const slugStatusMessage = ref('');
    const fieldErrors = reactive({});
    const isLoadingEvent = ref(false);
    const touched = reactive({});
    const submitAttempted = ref(false);
    const selectedTagCount = computed(() => Array.isArray(model.TagIds) ? model.TagIds.length : 0);
    const showTagCountWarning = computed(() => selectedTagCount.value > 5);
    const categoryCount = computed(() => Array.isArray(config.Categories) ? config.Categories.length : 0);
    const showCategoryGuidance = computed(() => categoryCount.value >= 3);
    const returnToEventsUrl = ref('');
    const returnCountdown = ref(0);
    const showReturnAfterSave = ref(false);
    const selectedDownloadFolderId = ref(null);
    const venueAutocomplete = reactive({
        IsOpen: false,
        IsLoading: false,
        Error: '',
        ActiveField: '',
        Results: []
    });
    const fieldHelp = reactive({
        HeroImageUrl: false,
        ListImageUrl: false,
        ThumbnailImageUrl: false,
        ImageAltText: false,
        Title: false,
        Slug: false,
        ShortSummary: false,
        StartDateTimeLocalText: false,
        EndDateTimeLocalText: false,
        IsAllDay: false,
        OrganizerUserId: false,
        EventCategoryId: false,
        TagIds: false,
        VenueName: false,
        PublicLocationText: false,
        AddressLine1: false,
        CountryCode: false,
        Region: false,
        Latitude: false,
        Longitude: false,
        RegistrationUrl: false,
        RegistrationButtonText: false,
        SecondaryCtaUrl: false,
        SecondaryCtaText: false,
        SeoTitle: false,
        SeoDescription: false,
        CanonicalUrl: false,
        MetaRobots: false,
        DownloadFileSection: false,
        FeaturesAndAudiencesSection: false
    });
    const fieldHelpText = {
        Title: 'Use a clear name for the event. Keep it short, specific, and easy to understand.',
        Slug: 'This is the page URL name for the event. Use only lowercase letters, numbers, and hyphens. Example: member-breakfast-mixer',
        ShortSummary: 'Write 1 to 3 short sentences that explain what the event is and why someone should care.',
        StartDateTimeLocalText: 'Choose when the event begins. Be sure the date and time are correct for your event.',
        EndDateTimeLocalText: 'Choose when the event ends. Leave this blank only if the end time is unknown.',
        IsAllDay: 'Turn this on if the event lasts all day and should not show a start or end time.',
        OrganizerUserId: 'Choose the person who is responsible for this event. This may be shown on the event page.',
        EventCategoryId: 'Pick the main type of event. Choose the one that best matches the event.',
        TagIds: 'Add a few helpful keywords people might use to find this event. Do not add too many.',
        VenueName: 'Enter the name of the place where the event happens, if there is one. Example: Hiller Aviation Museum.',
        PublicLocationText: 'Use this to show a simple location message to visitors. Example: San Carlos, CA or Online via Zoom.',
        AddressLine1: 'Enter the street address of the event location, if you want to store the full address.',
        CountryCode: 'Enter the country for the event location. This helps keep address details accurate. Example: US',
        Region: 'Enter the state, province, or region for the event location. Example: CA.',
        Latitude: 'When valid, presents a clickable map to website visitors. Use latitude only when you know the exact location. It should be a decimal value between -90 and 90. Example: 37.507159',
        Longitude: 'When valid, presents a clickable map to website visitors. Use longitude only when you know the exact location. It should be a decimal value between -180 and 180. Example: -122.260522',
        RegistrationUrl: 'Add the web address people should visit to register, RSVP, buy tickets, or learn more.',
        RegistrationButtonText: 'Choose short button text that tells people what to do. Example: Register Now, RSVP, or Buy Tickets.',
        SecondaryCtaUrl: 'Add a second web address if you want to show another button or link for this event.',
        SecondaryCtaText: 'Use short text for the second button or link. Example: Learn More, View Agenda, or Contact Us.',
        SeoTitle: 'This title may be used by search engines. Keep it clear, useful, and close to the event title.',
        SeoDescription: 'Write a short summary for search engines. Aim for 1 to 2 clear sentences that make people want to click.',
        CanonicalUrl: 'Use this only if another page should be treated as the main version of this event page.',
        MetaRobots: 'This controls whether search engines should index this page or follow its links. Leave the default unless you know you need something different.',
        DownloadFileSection: 'Use this section if visitors should be able to download a file for the event, such as a flyer, agenda, map, or handout.',
        FeaturesAndAudiencesSection: 'Use this section to highlight key event benefits and who the event is for. Keep each item short and easy to scan.'
    };

    let venueAutocompleteDebounceHandle = 0;
    let venueAutocompleteBlurHandle = 0;
    let venueAutocompleteRequestToken = 0;

    let returnRedirectTimeoutHandle = 0;
    let returnRedirectIntervalHandle = 0;

    function formatFolderPath(folderPath) {
        if (!folderPath || !String(folderPath).trim()) {
            return '/';
        }

        let normalized = String(folderPath).replace(/\\/g, '/').trim();

        if (!normalized.startsWith('/')) {
            normalized = '/' + normalized;
        }

        if (normalized.length > 1 && normalized.endsWith('/')) {
            normalized = normalized.slice(0, -1);
        }

        return normalized + '/';
    }

    function formatFileOptionLabel(file) {
        if (!file) {
            return '';
        }

        return `${formatFolderPath(file.FolderPath)}${file.FileName || ''}`;
    }

    function getFieldHelpText(fieldName) {
        return fieldHelpText[fieldName] || '';
    }

    function getFieldHelpPopoverId(fieldName) {
        return `${String(fieldName || '').replace(/[^a-zA-Z0-9]+/g, '')}HelpPopover`;
    }

    function normalizePathName(pathName) {
        if (!pathName) {
            return '/';
        }

        let normalized = String(pathName).trim();

        if (!normalized.startsWith('/')) {
            normalized = `/${normalized}`;
        }

        if (normalized.length > 1 && normalized.endsWith('/')) {
            normalized = normalized.slice(0, -1);
        }

        return normalized.toLowerCase();
    }

    function isEventsPath(pathName) {
        const normalized = normalizePathName(pathName);

        return /(^|\/)events(\/|$)/i.test(normalized);
    }

    function detectReturnToEventsUrl() {
        const referrer = document.referrer;

        if (!referrer) {
            return '';
        }

        try {
            const currentUrl = new URL(window.location.href);
            const referrerUrl = new URL(referrer);

            if (referrerUrl.origin !== currentUrl.origin) {
                return '';
            }

            if (!isEventsPath(referrerUrl.pathname)) {
                return '';
            }

            return referrerUrl.href;
        } catch (error) {
            return '';
        }
    }

    function clearReturnRedirectTimers() {
        window.clearTimeout(returnRedirectTimeoutHandle);
        window.clearInterval(returnRedirectIntervalHandle);

        returnRedirectTimeoutHandle = 0;
        returnRedirectIntervalHandle = 0;
    }

    function cancelReturnRedirect() {
        clearReturnRedirectTimers();
        showReturnAfterSave.value = false;
        returnCountdown.value = 0;
    }

    function redirectBackToEventsPage() {
        if (!returnToEventsUrl.value) {
            return;
        }

        clearReturnRedirectTimers();
        window.location.href = returnToEventsUrl.value;
    }

    function cancelAndReturn() {
        clearReturnRedirectTimers();

        if (returnToEventsUrl.value) {
            window.location.href = returnToEventsUrl.value;
            return;
        }

        window.history.back();
    }

    function beginReturnRedirectCountdown() {
        clearReturnRedirectTimers();

        if (!returnToEventsUrl.value) {
            showReturnAfterSave.value = false;
            returnCountdown.value = 0;
            return;
        }

        returnCountdown.value = 5;
        showReturnAfterSave.value = true;

        returnRedirectIntervalHandle = window.setInterval(() => {
            if (returnCountdown.value > 1) {
                returnCountdown.value -= 1;
                return;
            }

            window.clearInterval(returnRedirectIntervalHandle);
            returnRedirectIntervalHandle = 0;
        }, 1000);

        returnRedirectTimeoutHandle = window.setTimeout(() => {
            redirectBackToEventsPage();
        }, 5000);
    }

    const categoryModal = reactive({
        IsOpen: false,
        Name: '',
        Error: '',
        IsSaving: false
    });

    const tagModal = reactive({
        IsOpen: false,
        Name: '',
        Error: '',
        IsSaving: false
    });

    function readEventIdFromQuery() {
        const query = new URLSearchParams(window.location.search);

        const candidates = [
            query.get('eventId'),
            query.get('eventid'),
            query.get('EventId'),
            query.get('id'),
            query.get('Id')
        ];

        for (const candidate of candidates) {
            const parsed = Number(candidate);
            if (!Number.isNaN(parsed) && parsed > 0) {
                return parsed;
            }
        }

        const pathParts = (window.location.pathname || '')
            .split('/')
            .map(x => (x || '').trim())
            .filter(Boolean);

        if (pathParts.length > 0) {
            const lastPart = pathParts[pathParts.length - 1];
            const parsed = Number(lastPart);

            if (!Number.isNaN(parsed) && parsed > 0) {
                return parsed;
            }
        }

        return null;
    }

    function closeAllFieldHelp() {
        Object.keys(fieldHelp).forEach((key) => {
            fieldHelp[key] = false;
        });
    }

    function isFieldHelpOpen(fieldName) {
        return !!fieldHelp[fieldName];
    }

    function openFieldHelp(fieldName) {
        closeAllFieldHelp();

        if (Object.prototype.hasOwnProperty.call(fieldHelp, fieldName)) {
            fieldHelp[fieldName] = true;
        }
    }

    function closeFieldHelp(fieldName) {
        if (Object.prototype.hasOwnProperty.call(fieldHelp, fieldName)) {
            fieldHelp[fieldName] = false;
        }
    }

    function toggleFieldHelp(fieldName) {
        const nextState = !isFieldHelpOpen(fieldName);
        closeAllFieldHelp();

        if (nextState && Object.prototype.hasOwnProperty.call(fieldHelp, fieldName)) {
            fieldHelp[fieldName] = true;
        }
    }

    function getVenueAutocompleteQuery(fieldName) {
        if (fieldName === 'AddressLine1') {
            return (model.AddressLine1 || '').trim();
        }

        return (model.VenueName || '').trim();
    }

    function fetchVenueSuggestionsForField(fieldName) {
        const query = getVenueAutocompleteQuery(fieldName);

        venueAutocomplete.ActiveField = fieldName;
        venueAutocomplete.Error = '';

        if (!model.PortalId || Number(model.PortalId) <= 0) {
            clearVenueAutocomplete();
            return;
        }

        if (!query || query.length < 3) {
            venueAutocomplete.IsOpen = false;
            venueAutocomplete.IsLoading = false;
            venueAutocomplete.Results = [];
            return;
        }

        const requestToken = ++venueAutocompleteRequestToken;

        venueAutocomplete.IsLoading = true;
        venueAutocomplete.IsOpen = true;

        getVenueSuggestions(
            dnnConfig,
            model.PortalId,
            query,
            8,
            (results) => {
                if (requestToken !== venueAutocompleteRequestToken) {
                    return;
                }

                venueAutocomplete.IsLoading = false;
                venueAutocomplete.Results = Array.isArray(results) ? results : [];
                venueAutocomplete.IsOpen = true;
            },
            (errorMessage) => {
                if (requestToken !== venueAutocompleteRequestToken) {
                    return;
                }

                venueAutocomplete.IsLoading = false;
                venueAutocomplete.Results = [];
                venueAutocomplete.Error = errorMessage || 'Unable to load venue suggestions.';
                venueAutocomplete.IsOpen = true;
            }
        );
    }

    function onVenueAutocompleteInput(fieldName) {
        venueAutocomplete.ActiveField = fieldName;
        venueAutocomplete.Error = '';

        window.clearTimeout(venueAutocompleteDebounceHandle);

        venueAutocompleteDebounceHandle = window.setTimeout(() => {
            fetchVenueSuggestionsForField(fieldName);
        }, 300);
    }

    function onVenueAutocompleteFocus(fieldName) {
        venueAutocomplete.ActiveField = fieldName;

        const query = getVenueAutocompleteQuery(fieldName);
        if (query && query.length >= 3) {
            fetchVenueSuggestionsForField(fieldName);
        }
    }

    function selectVenueSuggestion(suggestion) {
        if (!suggestion) {
            return;
        }

        model.UseCustomLocationText = true;

        if (suggestion.VenueName) {
            model.VenueName = suggestion.VenueName;
        }

        if (suggestion.PublicLocationText) {
            model.PublicLocationText = suggestion.PublicLocationText;
        } else if (suggestion.DisplayName) {
            model.PublicLocationText = suggestion.DisplayName;
        }

        model.AddressLine1 = suggestion.AddressLine1 || '';
        model.AddressLine2 = suggestion.AddressLine2 || '';
        model.City = suggestion.City || '';
        model.Region = suggestion.Region || '';
        model.PostalCode = suggestion.PostalCode || '';
        model.CountryCode = suggestion.CountryCode || '';

        model.Latitude = suggestion.Latitude != null && suggestion.Latitude !== ''
            ? Number(suggestion.Latitude)
            : null;

        model.Longitude = suggestion.Longitude != null && suggestion.Longitude !== ''
            ? Number(suggestion.Longitude)
            : null;

        clearVenueAutocomplete();
    }

    function applyConfig(result) {
        config.Portals = result?.Portals || [];
        config.Categories = result?.Categories || [];
        config.Tags = result?.Tags || [];
        config.Organizers = result?.Organizers || [];
        config.Folders = result?.Folders || [];
        config.Files = [];
        config.TimeZoneOptions = result?.TimeZoneOptions || [];
        config.DefaultTimeZoneId = result?.DefaultTimeZoneId || '';
        config.IsSuperUser = !!result?.IsSuperUser;
    }

    function enforceCurrentPortalForNonSuperUser() {
        if (!config.IsSuperUser) {
            model.PortalId = Number(dnnConfig?.portalId || 0);
        }
    }

    function clearFieldErrors() {
        Object.keys(fieldErrors).forEach((key) => {
            delete fieldErrors[key];
        });
    }

    function setFieldError(fieldName, message) {
        fieldErrors[fieldName] = message;
    }

    function getFieldError(fieldName) {
        return fieldErrors[fieldName] || '';
    }

    function getInputClass(fieldName) {
        const error = getFieldError(fieldName);

        if (!shouldShowFieldError(fieldName)) {
            return '';
        }

        return error ? 'is-invalid' : 'is-valid';
    }

    const eventSlugMaxLength = 220;
    function normalizeEventSlug(value, maxLength = eventSlugMaxLength) {
        if (!value) {
            return '';
        }

        let normalized = value
            .toLowerCase()
            .trim()
            .replace(/[^a-z0-9]+/g, '-')
            .replace(/-+/g, '-')
            .replace(/^-+|-+$/g, '');

        if (normalized.length > maxLength) {
            normalized = normalized.substring(0, maxLength).replace(/-+$/g, '');
        }

        return normalized;
    }

    function buildYearSuffixedSlug(title) {
        const year = new Date().getFullYear().toString();
        const suffix = `-${year}`;
        const maxBaseLength = eventSlugMaxLength - suffix.length;

        const baseSlug = normalizeEventSlug(title, maxBaseLength);
        if (!baseSlug) {
            return '';
        }

        return baseSlug.endsWith(suffix) ? baseSlug : `${baseSlug}${suffix}`;
    }

    function isValidOptionalUrl(value) {
        if (!value) {
            return true;
        }

        try {
            const url = new URL(value);
            return url.protocol === 'http:' || url.protocol === 'https:';
        } catch {
            return false;
        }
    }

    function isValidOptionalEmail(value) {
        if (!value) {
            return true;
        }

        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
    }

    function isValidLocalDateTimeInput(value) {
        if (!value) {
            return true;
        }

        if (!/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}$/.test(value)) {
            return false;
        }

        const parsed = new Date(value);
        return !Number.isNaN(parsed.getTime());
    }

    function isBlank(value) {
        return value == null || String(value).trim() === '';
    }

    function normalizeIntegerArray(values) {
        return (Array.isArray(values) ? values : [])
            .map(x => Number(x))
            .filter(x => !Number.isNaN(x) && x > 0);
    }

    function clearVenueAutocomplete() {
        venueAutocomplete.IsOpen = false;
        venueAutocomplete.IsLoading = false;
        venueAutocomplete.Error = '';
        venueAutocomplete.ActiveField = '';
        venueAutocomplete.Results = [];
    }

    function cancelVenueAutocompleteClose() {
        window.clearTimeout(venueAutocompleteBlurHandle);
        venueAutocompleteBlurHandle = 0;
    }

    function scheduleVenueAutocompleteClose() {
        cancelVenueAutocompleteClose();

        venueAutocompleteBlurHandle = window.setTimeout(() => {
            clearVenueAutocomplete();
        }, 150);
    }

    function validateClient() {
        clearFieldErrors();
        globalError.value = '';
        slugStatusMessage.value = '';

        model.TagIds = normalizeIntegerArray(model.TagIds);

        if (!model.PortalId || Number(model.PortalId) <= 0) {
            setFieldError('PortalId', 'Portal is required.');
        }

        if (isBlank(model.Title)) {
            setFieldError('Title', 'Title is required.');
        }

        if (isBlank(model.FullDescription))
        {
            setFieldError('FullDescription', 'Full description is required.');
        }

        if (isBlank(model.Slug)) {
            setFieldError('Slug', 'Slug is required.');
        } else if (model.Slug !== normalizeEventSlug(model.Slug)) {
            setFieldError('Slug', 'Slug may only contain lowercase letters, numbers, and hyphens.');
        }
        if (!isBlank(model.Slug) && model.Slug.length > eventSlugMaxLength) {
            setFieldError('Slug', `Slug must be ${eventSlugMaxLength} characters or fewer.`);
        }

        if (isBlank(model.StartDateTimeLocalText)) {
            setFieldError('StartDateTimeLocalText', 'Start date and time is required.');
        } else if (!isValidLocalDateTimeInput(model.StartDateTimeLocalText)) {
            setFieldError('StartDateTimeLocalText', 'Start date and time is invalid.');
        }

        if (!isBlank(model.EndDateTimeLocalText) && !isValidLocalDateTimeInput(model.EndDateTimeLocalText)) {
            setFieldError('EndDateTimeLocalText', 'End date and time is invalid.');
        }

        if (model.StartDateTimeLocalText && model.EndDateTimeLocalText &&
            isValidLocalDateTimeInput(model.StartDateTimeLocalText) &&
            isValidLocalDateTimeInput(model.EndDateTimeLocalText)) {
            const start = new Date(model.StartDateTimeLocalText);
            const end = new Date(model.EndDateTimeLocalText);

            if (end < start) {
                setFieldError('EndDateTimeLocalText', 'End date and time cannot be earlier than the start date and time.');
            }
        }

        if (isBlank(model.TimeZoneId)) {
            setFieldError('TimeZoneId', 'Time zone is required.');
        }

        if (!isBlank(model.PublishStartLocalText) && !isValidLocalDateTimeInput(model.PublishStartLocalText)) {
            setFieldError('PublishStartLocalText', 'Publish start is invalid.');
        }

        if (!isBlank(model.PublishEndLocalText) && !isValidLocalDateTimeInput(model.PublishEndLocalText)) {
            setFieldError('PublishEndLocalText', 'Publish end is invalid.');
        }

        if (model.PublishStartLocalText && model.PublishEndLocalText &&
            isValidLocalDateTimeInput(model.PublishStartLocalText) &&
            isValidLocalDateTimeInput(model.PublishEndLocalText)) {
            const publishStart = new Date(model.PublishStartLocalText);
            const publishEnd = new Date(model.PublishEndLocalText);

            if (publishEnd < publishStart) {
                setFieldError('PublishEndLocalText', 'Publish end cannot be earlier than publish start.');
            }
        }

        if (model.PriceAmount != null && model.PriceAmount !== '' && Number(model.PriceAmount) < 0) {
            setFieldError('PriceAmount', 'Price amount cannot be negative.');
        }

        if (model.Capacity != null && model.Capacity !== '' && Number(model.Capacity) < 0) {
            setFieldError('Capacity', 'Capacity cannot be negative.');
        }

        if (!isValidOptionalEmail(model.ContactEmail)) {
            setFieldError('ContactEmail', 'Contact email is invalid.');
        }

        if (!isValidOptionalUrl(model.RegistrationUrl)) {
            setFieldError('RegistrationUrl', 'Registration URL is invalid.');
        }

        if (!isBlank(model.RegistrationUrl) && isBlank(model.RegistrationButtonText)) {
            setFieldError('RegistrationButtonText', 'Registration button text is required when a registration URL is provided.');
        }

        if (!isValidOptionalUrl(model.SecondaryCtaUrl)) {
            setFieldError('SecondaryCtaUrl', 'Secondary CTA URL is invalid.');
        }

        if (!isBlank(model.SecondaryCtaUrl) && isBlank(model.SecondaryCtaText)) {
            setFieldError('SecondaryCtaText', 'Secondary CTA text is required when a secondary CTA URL is provided.');
        }

        if (!isValidOptionalUrl(model.ContactUrl)) {
            setFieldError('ContactUrl', 'Contact URL is invalid.');
        }

        if (!isValidOptionalUrl(model.CanonicalUrl)) {
            setFieldError('CanonicalUrl', 'Canonical URL is invalid.');
        }

        if (!isValidOptionalUrl(model.VenueWebsiteUrl)) {
            setFieldError('VenueWebsiteUrl', 'Venue website URL is invalid.');
        }

        if (!isValidOptionalUrl(model.HeroImageUrl)) {
            setFieldError('HeroImageUrl', 'Hero image URL is invalid.');
        }

        if (!isValidOptionalUrl(model.ListImageUrl)) {
            setFieldError('ListImageUrl', 'List image URL is invalid.');
        }

        if (!isValidOptionalUrl(model.ThumbnailImageUrl)) {
            setFieldError('ThumbnailImageUrl', 'Thumbnail image URL is invalid.');
        }

        if (!isValidOptionalUrl(model.DownloadFileUrl)) {
            setFieldError('DownloadFileUrl', 'Download file URL is invalid.');
        }

        if (model.Latitude != null && model.Latitude !== '' && (Number(model.Latitude) < -90 || Number(model.Latitude) > 90)) {
            setFieldError('Latitude', 'Latitude must be between -90 and 90.');
        }

        if (model.Longitude != null && model.Longitude !== '' && (Number(model.Longitude) < -180 || Number(model.Longitude) > 180)) {
            setFieldError('Longitude', 'Longitude must be between -180 and 180.');
        }

        (model.Features || []).forEach((feature, index) => {
            if ((feature?.Description || feature?.IconClass) && isBlank(feature?.Title)) {
                setFieldError(`Feature-${index}`, `Feature ${index + 1} must have a title.`);
            }
        });

        (model.Audiences || []).forEach((audience, index) => {
            if (audience?.Description && isBlank(audience?.Title)) {
                setFieldError(`Audience-${index}`, `Audience ${index + 1} must have a title.`);
            }
        });

        return Object.keys(fieldErrors).length === 0;
    }

    function markTouched(fieldName) {
        touched[fieldName] = true;
    }

    function isTouched(fieldName) {
        return !!touched[fieldName];
    }

    function shouldShowFieldError(fieldName) {
        return isTouched(fieldName) || submitAttempted.value;
    }

    function getVisibleFieldError(fieldName) {
        if (!shouldShowFieldError(fieldName)) {
            return '';
        }

        return getFieldError(fieldName);
    }

    function applyServerValidationErrors(message) {
        clearFieldErrors();

        const lines = String(message || '')
            .split(/\r?\n/)
            .map(x => x.trim())
            .filter(Boolean);

        const remaining = [];

        lines.forEach((line) => {
            if (line === 'Portal is required.') {
                setFieldError('PortalId', line);
            } else if (line === 'Title is required.') {
                setFieldError('Title', line);
            } else if (line === 'Slug is required.' || line === 'The slug must be unique within the selected portal.') {
                setFieldError('Slug', line);
            } else if (line === 'Start date and time is required.' || line === 'Start date and time is invalid.') {
                setFieldError('StartDateTimeLocalText', line);
            } else if (line === 'End date and time is invalid.' || line === 'End date and time cannot be earlier than the start date and time.') {
                setFieldError('EndDateTimeLocalText', line);
            } else if (line === 'Time zone is required.') {
                setFieldError('TimeZoneId', line);
            } else if (line === 'Publish start is invalid.') {
                setFieldError('PublishStartLocalText', line);
            } else if (line === 'Publish end is invalid.' || line === 'Publish end cannot be earlier than publish start.') {
                setFieldError('PublishEndLocalText', line);
            } else if (line === 'Price amount cannot be negative.') {
                setFieldError('PriceAmount', line);
            } else if (line === 'Capacity cannot be negative.') {
                setFieldError('Capacity', line);
            } else if (line === 'Contact email is invalid.') {
                setFieldError('ContactEmail', line);
            } else if (line === 'Registration URL is invalid.') {
                setFieldError('RegistrationUrl', line);
            } else if (line === 'Secondary CTA URL is invalid.') {
                setFieldError('SecondaryCtaUrl', line);
            } else if (line === 'Contact URL is invalid.') {
                setFieldError('ContactUrl', line);
            } else if (line === 'Canonical URL is invalid.') {
                setFieldError('CanonicalUrl', line);
            } else if (line === 'Venue website URL is invalid.') {
                setFieldError('VenueWebsiteUrl', line);
            } else if (line === 'The selected download file does not belong to the selected portal.') {
                setFieldError('DownloadFileId', line);
            } else if (line === 'The selected organizer is not valid for the selected portal.') {
                setFieldError('OrganizerUserId', line);
            } else if (line === 'The selected category is not valid for the selected portal.') {
                setFieldError('EventCategoryId', line);
            } else if (line === 'One or more selected tags are not valid for the selected portal.') {
                setFieldError('TagIds', line);
            } else if (line === 'The event being edited is not valid for the selected portal.') {
                remaining.push(line);
            } else {
                const featureMatch = /^Feature (\d+) must have a title\.$/.exec(line);
                if (featureMatch) {
                    setFieldError(`Feature-${Number(featureMatch[1]) - 1}`, line);
                    return;
                }

                const audienceMatch = /^Audience (\d+) must have a title\.$/.exec(line);
                if (audienceMatch) {
                    setFieldError(`Audience-${Number(audienceMatch[1]) - 1}`, line);
                    return;
                }

                remaining.push(line);
            }
        });

        const hasFieldErrors = Object.keys(fieldErrors).length > 0;

        if (hasFieldErrors) {
            globalError.value = remaining.length > 0
                ? remaining.join(' ')
                : 'Please correct the highlighted fields and try again.';
            return;
        }

        globalError.value = lines.join(' ') || 'The event could not be saved.';
    }

    async function onTitleBlur() {
        markTouched('Title');

        if ((!model.Slug || !model.Slug.trim()) && model.Title) {
            model.Slug = buildYearSuffixedSlug(model.Title);
            markTouched('Slug');
            await runSlugAvailabilityCheck(false);
        }
    }

    async function onSlugBlur() {
        markTouched('Slug');

        if (model.Slug && model.Slug.trim()) {
            model.Slug = normalizeEventSlug(model.Slug);
            await runSlugAvailabilityCheck(true);
        }
    }

    function runSlugAvailabilityCheck(showSuccessMessage) {
        return new Promise((resolve) => {
            if (!model.PortalId || !model.Slug || !model.Slug.trim()) {
                resolve(false);
                return;
            }

            isCheckingSlug.value = true;

            checkSlugAvailability(
                dnnConfig,
                model.PortalId,
                model.Slug.trim(),
                model.EventId,
                (result) => {
                    isCheckingSlug.value = false;

                    if (result && result.IsAvailable) {
                        delete fieldErrors.Slug;
                        slugStatusMessage.value = showSuccessMessage ? 'Slug is available.' : '';
                        resolve(true);
                        return;
                    }

                    setFieldError('Slug', 'The slug must be unique within the selected portal.');
                    slugStatusMessage.value = '';
                    resolve(false);
                },
                (errorMessage) => {
                    isCheckingSlug.value = false;
                    globalError.value = errorMessage || 'Unable to validate the slug right now.';
                    resolve(false);
                }
            );
        });
    }

    function reloadConfig(portalId, onComplete) {
        getConfig(
            dnnConfig,
            portalId,
            (result) => {
                applyConfig(result);
                enforceCurrentPortalForNonSuperUser();

                if (typeof onComplete === 'function') {
                    onComplete(result);
                }
            },
            (errorMessage) => {
                globalError.value = errorMessage || 'Unable to load form configuration.';
            }
        );
    }

    function reconcilePortalDependentSelections() {
        const organizerIds = new Set((config.Organizers || []).map(x => Number(x.UserId)));
        const categoryIds = new Set((config.Categories || []).map(x => Number(x.EventCategoryId)));
        const tagIds = new Set((config.Tags || []).map(x => Number(x.TagId)));

        if (model.OrganizerUserId != null && !organizerIds.has(Number(model.OrganizerUserId))) {
            model.OrganizerUserId = null;
        }

        if (model.EventCategoryId != null && !categoryIds.has(Number(model.EventCategoryId))) {
            model.EventCategoryId = null;
        }

        model.TagIds = (model.TagIds || []).filter(tagId => tagIds.has(Number(tagId)));

        if (model.DownloadFileId != null && Number(model.DownloadFileId) > 0) {
            ensureSelectedDownloadFileLoaded();
        } else {
            selectedDownloadFolderId.value = null;
            config.Files = [];
            model.DownloadFileUrl = '';
        }
    }

    function syncDownloadFileUrlFromSelection() {
        if (model.DownloadFileId == null || Number(model.DownloadFileId) <= 0) {
            return;
        }

        const selectedFile = (config.Files || []).find(x => Number(x.FileId) === Number(model.DownloadFileId));
        if (!selectedFile) {
            return;
        }

        model.DownloadFileUrl = selectedFile.RelativeUrl || '';
    }

    function loadFilesForFolder(folderId, selectedFileId = null, onComplete = null) {
        if (!folderId || Number(folderId) <= 0) {
            config.Files = [];
            if (typeof onComplete === 'function') {
                onComplete();
            }
            return;
        }

        getFilesByFolder(
            dnnConfig,
            model.PortalId,
            folderId,
            (result) => {
                config.Files = result || [];

                if (selectedFileId != null && Number(selectedFileId) > 0) {
                    const fileExists = (config.Files || []).some(x => Number(x.FileId) === Number(selectedFileId));
                    if (!fileExists) {
                        model.DownloadFileId = null;
                        model.DownloadFileUrl = '';
                    }
                } else if (model.DownloadFileId != null && Number(model.DownloadFileId) > 0) {
                    const fileExists = (config.Files || []).some(x => Number(x.FileId) === Number(model.DownloadFileId));
                    if (!fileExists) {
                        model.DownloadFileId = null;
                        model.DownloadFileUrl = '';
                    }
                }

                syncDownloadFileUrlFromSelection();

                if (typeof onComplete === 'function') {
                    onComplete();
                }
            },
            (errorMessage) => {
                config.Files = [];
                globalError.value = errorMessage || 'Unable to load files for the selected folder.';

                if (typeof onComplete === 'function') {
                    onComplete();
                }
            }
        );
    }

    function ensureSelectedDownloadFileLoaded() {
        if (model.DownloadFileId == null || Number(model.DownloadFileId) <= 0) {
            selectedDownloadFolderId.value = null;
            config.Files = [];
            return;
        }

        getFileById(
            dnnConfig,
            model.PortalId,
            model.DownloadFileId,
            (result) => {
                if (!result) {
                    selectedDownloadFolderId.value = null;
                    config.Files = [];
                    model.DownloadFileId = null;
                    model.DownloadFileUrl = '';
                    return;
                }

                selectedDownloadFolderId.value = Number(result.FolderId);
                config.Files = [result];
                syncDownloadFileUrlFromSelection();

                loadFilesForFolder(selectedDownloadFolderId.value, model.DownloadFileId);
            },
            () => {
                selectedDownloadFolderId.value = null;
                config.Files = [];
                model.DownloadFileId = null;
                model.DownloadFileUrl = '';
            }
        );
    }

    function onDownloadFolderChanged() {
        if (!selectedDownloadFolderId.value || Number(selectedDownloadFolderId.value) <= 0) {
            config.Files = [];
            model.DownloadFileId = null;
            model.DownloadFileUrl = '';
            markTouched('DownloadFileId');
            return;
        }

        model.DownloadFileId = null;
        model.DownloadFileUrl = '';
        markTouched('DownloadFileId');

        loadFilesForFolder(selectedDownloadFolderId.value);
    }

    async function onPortalChanged() {
        if (!config.IsSuperUser) {
            model.PortalId = Number(dnnConfig?.portalId || 0);
            return;
        }

        saveMessage.value = '';
        globalError.value = '';
        slugStatusMessage.value = '';
        clearVenueAutocomplete();

        reloadConfig(model.PortalId, async () => {
            selectedDownloadFolderId.value = null;
            config.Files = [];
            model.DownloadFileId = null;
            model.DownloadFileUrl = '';

            reconcilePortalDependentSelections();

            delete fieldErrors.OrganizerUserId;
            delete fieldErrors.EventCategoryId;
            delete fieldErrors.TagIds;
            delete fieldErrors.DownloadFileId;

            if (model.Slug && model.Slug.trim()) {
                await runSlugAvailabilityCheck(false);
            }
        });
    }

    function loadEvent() {
        const eventId = readEventIdFromQuery();

        if (!eventId) {
            return;
        }

        isLoadingEvent.value = true;

        getEventForEdit(dnnConfig, model.PortalId, eventId, (result, error) => {
            isLoadingEvent.value = false;

            if (error) {
                globalError.value = error;
                return;
            }

            if (!result) {
                globalError.value = 'The requested event could not be loaded.';
                return;
            }

            const targetPortalId = Number(result.PortalId || model.PortalId || 0);

            const applyEditModel = () => {
                const merged = Object.assign({}, createDefaultModel(), result);

                merged.TagIds = Array.isArray(result.TagIds) ? result.TagIds : [];
                merged.Features = Array.isArray(result.Features) ? result.Features : [];
                merged.Audiences = Array.isArray(result.Audiences) ? result.Audiences : [];

                if (!config.IsSuperUser) {
                    merged.PortalId = Number(dnnConfig?.portalId || 0);
                }

                Object.assign(model, merged);
                reconcilePortalDependentSelections();

                if (!model.TimeZoneId) {
                    model.TimeZoneId = config.DefaultTimeZoneId || 'Pacific Standard Time';
                }
            };

            if (targetPortalId > 0 && targetPortalId !== model.PortalId) {
                model.PortalId = targetPortalId;

                reloadConfig(model.PortalId, () => {
                    applyEditModel();
                });
            } else {
                applyEditModel();
            }
        });
    }

    async function save() {
        saveMessage.value = '';
        globalError.value = '';
        submitAttempted.value = true;

        cancelReturnRedirect();

        const isClientValid = validateClient();
        if (!isClientValid) {
            globalError.value = 'Please correct the highlighted fields and try again.';
            return;
        }

        const isSlugValid = await runSlugAvailabilityCheck(false);
        if (!isSlugValid) {
            globalError.value = getFieldError('Slug') || 'Please correct the highlighted fields and try again.';
            return;
        }

        isSaving.value = true;

        const payload = JSON.parse(JSON.stringify(model));
        payload.TagIds = normalizeIntegerArray(model.TagIds);

        saveEvent(dnnConfig, payload, (result, error) => {
            isSaving.value = false;

            if (error) {
                applyServerValidationErrors(error);
                return;
            }

            if (result && result.EventId) {
                const wasEditMode = !!model.EventId;
                model.EventId = result.EventId;
                saveMessage.value = wasEditMode
                    ? 'Event updated successfully.'
                    : 'Event created successfully.';

                beginReturnRedirectCountdown();
            } else {
                globalError.value = 'The event could not be saved.';
            }
        });
    }

    function openCategoryModal() {
        globalError.value = '';

        if (!model.PortalId || Number(model.PortalId) <= 0) {
            globalError.value = 'Please select a portal before adding a category.';
            return;
        }

        categoryModal.IsOpen = true;
        categoryModal.Name = '';
        categoryModal.Error = '';
        categoryModal.IsSaving = false;

        focusElementById('newCategoryName');
    }

    function closeCategoryModal() {
        categoryModal.IsOpen = false;
        categoryModal.Name = '';
        categoryModal.Error = '';
        categoryModal.IsSaving = false;
    }

    function submitCategoryModal() {
        categoryModal.Error = '';

        if (!categoryModal.Name || !categoryModal.Name.trim()) {
            categoryModal.Error = 'Category name is required.';
            return;
        }

        categoryModal.IsSaving = true;

        createCategory(
            dnnConfig,
            { PortalId: model.PortalId, Name: categoryModal.Name.trim() },
            (result) => {
                categoryModal.IsSaving = false;

                if (!result) {
                    categoryModal.Error = 'Unable to create the category.';
                    return;
                }

                config.Categories = [...config.Categories, result]
                    .sort((a, b) => (a.CategoryName || '').localeCompare(b.CategoryName || ''));

                model.EventCategoryId = result.EventCategoryId;
                closeCategoryModal();
            },
            (errorMessage) => {
                categoryModal.IsSaving = false;
                categoryModal.Error = errorMessage || 'Unable to create the category.';
            }
        );
    }

    function openTagModal() {
        globalError.value = '';

        if (!model.PortalId || Number(model.PortalId) <= 0) {
            globalError.value = 'Please select a portal before adding a tag.';
            return;
        }

        tagModal.IsOpen = true;
        tagModal.Name = '';
        tagModal.Error = '';
        tagModal.IsSaving = false;

        focusElementById('newTagName');
    }

    function closeTagModal() {
        tagModal.IsOpen = false;
        tagModal.Name = '';
        tagModal.Error = '';
        tagModal.IsSaving = false;
    }

    function submitTagModal() {
        tagModal.Error = '';

        if (!tagModal.Name || !tagModal.Name.trim()) {
            tagModal.Error = 'Tag name is required.';
            return;
        }

        tagModal.IsSaving = true;

        createTag(
            dnnConfig,
            { PortalId: model.PortalId, Name: tagModal.Name.trim() },
            (result) => {
                tagModal.IsSaving = false;

                if (!result) {
                    tagModal.Error = 'Unable to create the tag.';
                    return;
                }

                config.Tags = [...config.Tags, result]
                    .sort((a, b) => (a.TagName || '').localeCompare(b.TagName || ''));

                if (!Array.isArray(model.TagIds)) {
                    model.TagIds = [];
                }

                if (!model.TagIds.includes(result.TagId)) {
                    model.TagIds.push(result.TagId);
                }

                closeTagModal();
            },
            (errorMessage) => {
                tagModal.IsSaving = false;
                tagModal.Error = errorMessage || 'Unable to create the tag.';
            }
        );
    }

    function addFeature() {
        model.Features.push({
            Title: '',
            Description: '',
            IconClass: '',
            SortOrder: model.Features.length
        });
    }

    function removeFeature(index) {
        model.Features.splice(index, 1);
        model.Features.forEach((item, i) => {
            item.SortOrder = i;
        });
    }

    function addAudience() {
        model.Audiences.push({
            Title: '',
            Description: '',
            SortOrder: model.Audiences.length
        });
    }

    function removeAudience(index) {
        model.Audiences.splice(index, 1);
        model.Audiences.forEach((item, i) => {
            item.SortOrder = i;
        });
    }

    watch(
        () => categoryModal.IsOpen || tagModal.IsOpen,
        (isOpen) => {
            if (isOpen) {
                document.body.classList.add('modal-open');
            } else {
                document.body.classList.remove('modal-open');
            }
        }
    );

    watch(
        () => model.DownloadFileId,
        () => {
            if (model.DownloadFileId == null || Number(model.DownloadFileId) <= 0) {
                model.DownloadFileUrl = '';
                return;
            }

            syncDownloadFileUrlFromSelection();
        }
    );

    watch(
        () => model.Slug,
        () => {
            slugStatusMessage.value = '';
            delete fieldErrors.Slug;
        }
    );

    watch(
        () => model.Title,
        () => {
            if (!model.Slug || !model.Slug.trim()) {
                delete fieldErrors.Slug;
                slugStatusMessage.value = '';
            }
        }
    );

    function handleDocumentKeydown(event) {
        if (event.key !== 'Escape') {
            return;
        }

        if (tagModal.IsOpen) {
            closeTagModal();
            return;
        }

        if (categoryModal.IsOpen) {
            closeCategoryModal();
        }
    }

    function focusElementById(elementId) {
        window.setTimeout(() => {
            const element = document.getElementById(elementId);
            if (element) {
                element.focus();
                element.select?.();
            }
        }, 0);
    }

    onMounted(() => {
        document.addEventListener('keydown', handleDocumentKeydown);

        returnToEventsUrl.value = detectReturnToEventsUrl();

        const portalIdFromDom = dnnConfig?.portalId || 0;
        model.PortalId = portalIdFromDom;

        reloadConfig(model.PortalId, () => {
            if (!model.TimeZoneId) {
                model.TimeZoneId = config.DefaultTimeZoneId || 'Pacific Standard Time';
            }

            loadEvent();
        });
    });

    onBeforeUnmount(() => {
        document.removeEventListener('keydown', handleDocumentKeydown);
        document.body.classList.remove('modal-open');

        window.clearTimeout(venueAutocompleteDebounceHandle);
        window.clearTimeout(venueAutocompleteBlurHandle);

        clearReturnRedirectTimers();
        closeAllFieldHelp();
    });
</script>
<style scoped>
    .image-help-trigger {
        width: 1.8rem;
        height: 1.8rem;
        padding: 0;
        line-height: 1;
        font-weight: 600;
    }

    .image-help-popover {
        position: absolute;
        top: calc(100% + 0.4rem);
        left: 0;
        z-index: 20;
        width: min(26rem, 80vw);
    }

    @media (max-width: 575.98px) {
        .image-help-popover {
            width: min(20rem, 85vw);
        }
    }
</style>