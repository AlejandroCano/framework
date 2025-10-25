import { COMMAND_PRIORITY_NORMAL, KEY_DOWN_COMMAND } from "lexical";
import { LexicalHtmlEditorController } from "./LexicalHtmlEditorController";
import { useEffect } from 'react';

export const useRegisterKeybindings = (controller: LexicalHtmlEditorController, keybindingFn?: (event: KeyboardEvent) => boolean): void => {
  useEffect(() => {
    if (!controller?.editor || !keybindingFn) return;

    return controller.editor.registerCommand?.(KEY_DOWN_COMMAND, keybindingFn, COMMAND_PRIORITY_NORMAL);
  }, [controller.editor, keybindingFn])
}
