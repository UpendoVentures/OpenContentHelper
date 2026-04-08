<template>
    <div class="bdm-editor">
        <div class="bdm-editor-toolbar btn-toolbar mb-2" role="toolbar" :aria-label="toolbarLabel">
            <div class="btn-group btn-group-sm me-2 mb-2" role="group" aria-label="Text formatting">
                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor && editor.isActive('bold') }"
                        @click="editor.chain().focus().toggleBold().run()"
                        :disabled="!editor">
                    <i class="fas fa-bold" aria-hidden="true"></i>
                    <span class="visually-hidden">Bold</span>
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor && editor.isActive('italic') }"
                        @click="editor.chain().focus().toggleItalic().run()"
                        :disabled="!editor">
                    <i class="fas fa-italic" aria-hidden="true"></i>
                    <span class="visually-hidden">Italic</span>
                </button>
            </div>

            <div v-if="isStandard" class="btn-group btn-group-sm me-2 mb-2" role="group" aria-label="Lists">
                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor && editor.isActive('bulletList') }"
                        @click="editor.chain().focus().toggleBulletList().run()"
                        :disabled="!editor">
                    <i class="fas fa-list-ul" aria-hidden="true"></i>
                    <span class="visually-hidden">Bullet List</span>
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor && editor.isActive('orderedList') }"
                        @click="editor.chain().focus().toggleOrderedList().run()"
                        :disabled="!editor">
                    <i class="fas fa-list-ol" aria-hidden="true"></i>
                    <span class="visually-hidden">Numbered List</span>
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        :class="{ active: editor && editor.isActive('blockquote') }"
                        @click="editor.chain().focus().toggleBlockquote().run()"
                        :disabled="!editor">
                    <i class="fas fa-quote-right" aria-hidden="true"></i>
                    <span class="visually-hidden">Blockquote</span>
                </button>
            </div>

            <div class="btn-group btn-group-sm mb-2" role="group" aria-label="Links and cleanup">
                <button type="button"
                        class="btn btn-outline-secondary"
                        @click="setLink"
                        :disabled="!editor">
                    <i class="fas fa-link" aria-hidden="true"></i>
                    <span class="visually-hidden">Set Link</span>
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        @click="unsetLink"
                        :disabled="!editor">
                    <i class="fas fa-unlink" aria-hidden="true"></i>
                    <span class="visually-hidden">Remove Link</span>
                </button>

                <button type="button"
                        class="btn btn-outline-secondary"
                        @click="clearFormatting"
                        :disabled="!editor">
                    <i class="fas fa-eraser" aria-hidden="true"></i>
                    <span class="visually-hidden">Clear Formatting</span>
                </button>
            </div>
        </div>

        <div class="bdm-editor-shell" :class="{ 'is-invalid': invalid }">
            <EditorContent :editor="editor" />
        </div>

        <div v-if="helpText && !invalid" class="form-text">
            {{ helpText }}
        </div>

        <div v-if="invalid && validationMessage" class="invalid-feedback d-block">
            {{ validationMessage }}
        </div>
    </div>
</template>

<script>
    import { computed, watch, onBeforeUnmount } from 'vue';
    import { EditorContent, useEditor } from '@tiptap/vue-3';
    import StarterKit from '@tiptap/starter-kit';
    import Link from '@tiptap/extension-link';
    import Placeholder from '@tiptap/extension-placeholder';

    export default {
        name: 'RichTextEditor',
        components: {
            EditorContent
        },
        props: {
            modelValue: {
                type: String,
                default: ''
            },
            mode: {
                type: String,
                default: 'standard'
            },
            placeholder: {
                type: String,
                default: ''
            },
            invalid: {
                type: Boolean,
                default: false
            },
            validationMessage: {
                type: String,
                default: ''
            },
            helpText: {
                type: String,
                default: ''
            },
            toolbarLabel: {
                type: String,
                default: 'Rich text editor toolbar'
            }
        },
        emits: ['update:modelValue', 'blur'],
        setup(props, { emit }) {
            const isStandard = computed(() => props.mode === 'standard');

            const editor = useEditor({
                content: props.modelValue || '',
                extensions: [
                    StarterKit.configure({
                        heading: false,
                        codeBlock: false,
                        horizontalRule: false
                    }),
                    Link.configure({
                        openOnClick: false,
                        autolink: true,
                        protocols: ['http', 'https', 'mailto']
                    }),
                    Placeholder.configure({
                        placeholder: props.placeholder
                    })
                ],
                editorProps: {
                    attributes: {
                        class: 'form-control bdm-editor-content'
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
                (value) => {
                    if (!editor.value) {
                        return;
                    }

                    const currentHtml = editor.value.getHTML();
                    const nextHtml = value || '';

                    if (currentHtml !== nextHtml) {
                        editor.value.commands.setContent(nextHtml, false);
                    }
                }
            );

            function setLink() {
                if (!editor.value) {
                    return;
                }

                const previousUrl = editor.value.getAttributes('link').href || '';
                const url = window.prompt('Enter a URL', previousUrl);

                if (url === null) {
                    return;
                }

                if (!url.trim()) {
                    editor.value.chain().focus().extendMarkRange('link').unsetLink().run();
                    return;
                }

                editor.value.chain().focus().extendMarkRange('link').setLink({ href: url.trim() }).run();
            }

            function unsetLink() {
                if (!editor.value) {
                    return;
                }

                editor.value.chain().focus().extendMarkRange('link').unsetLink().run();
            }

            function clearFormatting() {
                if (!editor.value) {
                    return;
                }

                editor.value.chain().focus().clearNodes().unsetAllMarks().run();
            }

            onBeforeUnmount(() => {
                if (editor.value) {
                    editor.value.destroy();
                }
            });

            return {
                editor,
                isStandard,
                setLink,
                unsetLink,
                clearFormatting
            };
        }
    };
</script>