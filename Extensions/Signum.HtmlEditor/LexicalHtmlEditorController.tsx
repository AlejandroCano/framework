import { IBinding } from "@framework/Reflection";
import { $getRoot, EditorState } from "lexical";
import { LexicalEditor } from "lexical/LexicalEditor";
import * as React from "react";
import { HtmlEditorExtension } from "./Extensions/types";
import { ITextConverter } from "./HtmlContentStateConverter";
import { isEmpty } from "./Utils/editorState";
import { HtmlEditorController, HtmlEditorControllerProps } from "./HtmlEditorController";
import { useRegisterExtensions } from "./useRegisterExtensions";
import { useRegisterKeybindings } from "./useRegisterKeybindings";

export class LexicalHtmlEditorController extends HtmlEditorController {
  editor!: LexicalEditor;
  editorState!: EditorState;

  init(p: HtmlEditorControllerProps): void {
    this.binding = p.binding;
    this.readOnly = p.readOnly;
    this.small = p.small;
    this.converter = p.converter;
    this.plugins = p.plugins ?? [];
    this.editableElement = document.getElementById(p.editableId);

    [this.overrideToolbar, this.setOverrideToolbar] = React.useState<React.ReactElement | undefined>(undefined);

    useRegisterExtensions(this, this.plugins);
    useRegisterKeybindings(this, p.handleKeybindings);

    React.useEffect(() => {
      if (p.initiallyFocused) {
        window.setTimeout(
          () => {
            if (this.editor) this.editor.focus();
          },
          p.initiallyFocused == true ? 0 : (p.initiallyFocused as number)
        );
      }
    }, []);

    const newValue = this.binding.getValue();
    React.useEffect(() => {
      if (!this.editor) return;

      if (this.lastSavedString && this.lastSavedString.str === newValue) {
        this.lastSavedString = undefined;
        return;
      }

      const newState = this.converter.$convertFromText(this.editor, newValue || "");

      queueMicrotask(() => {
        if (newState.isEmpty && newState.isEmpty()) {
          this.editor.update(() => {
            $getRoot().clear();
          });
        } else {
          this.editor.setEditorState(newState);
        }
        const htmlString = this.converter.$convertToText(this.editor);
        this.initialEditorContent = htmlString;
      });
    }, [newValue, this.editor]);

    React.useEffect(() => {
      return () => this.saveHtml();
    }, []);

    this.setRefs = React.useCallback(
      (editor: LexicalEditor | null) => {
        this.editor = editor!;
        if (p.innerRef) {
          if (typeof p.innerRef == "function") p.innerRef(editor);
          else (p.innerRef as React.MutableRefObject<LexicalEditor | null>).current = editor;
        }
      },
      [p.innerRef]
    );
  }

  saveHtml(): void {
    if (this.readOnly) return;

    const newContentString = this.converter.$convertToText(this.editor);

    if (newContentString !== this.initialEditorContent) {
      const value = isEmpty(this.editorState) ? null : newContentString;
      this.lastSavedString = { str: value };
      this.binding.setValue(value);
    }
  }
}
