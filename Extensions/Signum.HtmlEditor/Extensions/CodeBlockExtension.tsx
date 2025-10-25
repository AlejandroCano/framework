import { CodeHighlightNode, CodeNode, registerCodeHighlighting } from "@lexical/code";
import { HtmlEditorController } from "../HtmlEditorController";
import {
  HtmlEditorExtension,
  LexicalConfigNode,
  OptionalCallback
} from "./types";
import { LexicalHtmlEditorController } from "../LexicalHtmlEditorController";

export class CodeBlockExtension implements HtmlEditorExtension {
  registerExtension(controller: LexicalHtmlEditorController): OptionalCallback {
      return registerCodeHighlighting(controller.editor);
  }

  getNodes(): LexicalConfigNode {
      return [CodeNode, CodeHighlightNode]
  }
}
