import { EditorState } from "lexical";
import { HtmlEditorController } from "../HtmlEditorController";
import { HtmlEditorExtension, OptionalCallback } from "./types";
import { LexicalHtmlEditorController } from "../LexicalHtmlEditorController";

type OnChangeCallback = (editorState?: EditorState) => void
type OnChangeExtensionProps = { onChange?: OnChangeCallback }


export class OnChangeExtension implements HtmlEditorExtension {
    props: OnChangeExtensionProps;

    constructor(onChange?: OnChangeCallback) {
        this.props = { onChange };
    }

  registerExtension(controller: LexicalHtmlEditorController): OptionalCallback {
        if(!controller.editor) return;

        return controller.editor.registerUpdateListener(({editorState}) => {
            this.props.onChange?.();
        });
    }
}
