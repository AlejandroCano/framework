import { COMMAND_PRIORITY_NORMAL, KEY_DOWN_COMMAND } from "lexical";
import { LexicalHtmlEditorController } from "../LexicalHtmlEditorController";
import { HtmlEditorExtension } from "./types";

export class BasicCommandsExtensions implements HtmlEditorExtension {
  registerExtension(controller: LexicalHtmlEditorController): () => void {
    return controller.editor.registerCommand(
      KEY_DOWN_COMMAND,
      (event) => {
        if (event.ctrlKey && event.key === "s") {
          controller.saveHtml();
          return true;
        }

        return false;
      },
      COMMAND_PRIORITY_NORMAL
    );
  }
}
