<template>
    <div class="rich-editor">
        <div class="btn-toolbar gap-2 mb-2" role="toolbar" aria-label="Full Description formatting toolbar">
            <div class="btn-group btn-group-sm" role="group" aria-label="Text formatting">
                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor?.isActive('bold') }"
                        @click="toggleBold"
                        :disabled="!editor"
                        aria-label="Bold">
                    <strong>B</strong>
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor?.isActive('italic') }"
                        @click="toggleItalic"
                        :disabled="!editor"
                        aria-label="Italic">
                    <em>I</em>
                </button>
            </div>

            <div class="btn-group btn-group-sm" role="group" aria-label="Lists">
                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor?.isActive('bulletList') }"
                        @click="toggleBulletList"
                        :disabled="!editor"
                        aria-label="Bulleted list">
                    • List
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor?.isActive('orderedList') }"
                        @click="toggleOrderedList"
                        :disabled="!editor"
                        aria-label="Numbered list">
                    1. List
                </button>
            </div>

            <div class="btn-group btn-group-sm" role="group" aria-label="Links">
                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor?.isActive('link') }"
                        @click="setLink"
                        :disabled="!editor"
                        aria-label="Add or edit link">
                    Link
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        @click="unsetLink"
                        :disabled="!editor || !editor.isActive('link')"
                        aria-label="Remove link">
                    Unlink
                </button>
            </div>

            <div class="btn-group btn-group-sm" role="group" aria-label="History">
                <button type="button"
                        class="btn btn-outline-secondary"
                        @click="undo"
                        :disabled="!editor || !editor.can().chain().focus().undo().run()"
                        aria-label="Undo">
                    Undo
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        @click="redo"
                        :disabled="!editor || !editor.can().chain().focus().redo().run()"
                        aria-label="Redo">
                    Redo
                </button>
            </div>
        </div>

        <div class="form-control rich-editor-shell" :class="{ 'is-invalid': invalid }">
            <EditorContent :editor="editor" />
        </div>

        <div v-if="helpText" class="form-text">
            {{ helpText }}
        </div>

        <div v-if="errorText" class="invalid-feedback d-block">
            {{ errorText }}
        </div>
    </div>
</template>

<script setup>
    import { onBeforeUnmount, watch } from 'vue';
    import { Editor, EditorContent } from '@tiptap/vue-3';
    import StarterKit from '@tiptap/starter-kit';
    import { Placeholder } from '@tiptap/extensions';

    const props = defineProps({
        modelValue: {
            type: String,
            default: ''
        },
        placeholder: {
            type: String,
            default: 'Add the full event description.'
        },
        helpText: {
            type: String,
            default: ''
        },
        errorText: {
            type: String,
            default: ''
        },
        invalid: {
            type: Boolean,
            default: false
        }
    });

    const emit = defineEmits(['update:modelValue', 'blur']);

    const editor = new Editor({
        content: props.modelValue || '',
        extensions: [
            StarterKit.configure({
                heading: false,
                blockquote: false,
                code: false,
                codeBlock: false,
                horizontalRule: false,
                strike: false,
                underline: false
            }),
            Placeholder.configure({
                placeholder: props.placeholder
            })
        ],
        editorProps: {
            attributes: {
                class: 'tiptap-editor',
                'aria-label': 'Full Description editor'
            }
        },
        onUpdate: ({ editor }) => {
            emit('update:modelValue', editor.getHTML());
        },
        onBlur: () => {
            emit('blur');
        }
    });

    watch(
        () => props.modelValue,
        (newValue) => {
            const incoming = newValue || '';
            const current = editor.getHTML();

            if (incoming !== current) {
                editor.commands.setContent(incoming, false);
            }
        }
    );

    function toggleBold() {
        editor.chain().focus().toggleBold().run();
    }

    function toggleItalic() {
        editor.chain().focus().toggleItalic().run();
    }

    function toggleBulletList() {
        editor.chain().focus().toggleBulletList().run();
    }

    function toggleOrderedList() {
        editor.chain().focus().toggleOrderedList().run();
    }

    function setLink() {
        const previousUrl = editor.getAttributes('link').href || '';
        const url = window.prompt('Enter the URL', previousUrl);

        if (url === null) {
            return;
        }

        const trimmed = url.trim();

        if (!trimmed) {
            editor.chain().focus().unsetLink().run();
            return;
        }

        editor.chain().focus().setLink({ href: trimmed }).run();
    }

    function unsetLink() {
        editor.chain().focus().unsetLink().run();
    }

    function undo() {
        editor.chain().focus().undo().run();
    }

    function redo() {
        editor.chain().focus().redo().run();
    }

    onBeforeUnmount(() => {
        editor.destroy();
    });
</script>

<style scoped>
    .rich-editor-shell {
        min-height: 14rem;
        padding: 0;
        overflow: hidden;
    }

    :deep(.tiptap-editor) {
        min-height: 14rem;
        padding: 0.75rem 1rem;
        outline: 0;
    }

    :deep(.tiptap-editor p) {
        margin-bottom: 0.75rem;
    }

    :deep(.tiptap-editor p:last-child) {
        margin-bottom: 0;
    }

    :deep(.tiptap-editor ul),
    :deep(.tiptap-editor ol) {
        padding-left: 1.5rem;
        margin-bottom: 0.75rem;
    }

    :deep(.tiptap-editor a) {
        text-decoration: underline;
    }

    :deep(.tiptap-editor p.is-editor-empty:first-child::before) {
        color: #6c757d;
        content: attr(data-placeholder);
        float: left;
        height: 0;
        pointer-events: none;
    }
</style>