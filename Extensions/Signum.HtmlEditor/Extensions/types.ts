import { InitialConfigType } from "@lexical/react/LexicalComposer";
import { HtmlEditorController } from "../HtmlEditorController";
import { LexicalHtmlEditorController } from "../LexicalHtmlEditorController";

export interface HtmlEditorExtension {
  getToolbarButtons?(controller: LexicalHtmlEditorController): React.ReactNode;
  registerExtension?(controller: LexicalHtmlEditorController): OptionalCallback;
  getNodes?(): LexicalConfigNode;
  getBuiltInComponent?(): ComponentAndProps;
}

export type ComponentAndProps<
  T extends React.FC<P> = React.FC<any>,
  P extends {} = React.ComponentProps<T>
> = {
  component: T;
  props?: P;
};

export type OptionalCallback = (() => void) | null | undefined;
export type LexicalConfigNode = InitialConfigType["nodes"];
