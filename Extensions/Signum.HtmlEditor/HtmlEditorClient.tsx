import * as React from "react";
import { IBinding } from "@framework/Reflection";
import { HtmlEditorController } from "./HtmlEditorController";

export type HtmlEditorFactoryProps = {
  binding: IBinding<string | null | undefined>;
  readOnly?: boolean;
  small?: boolean;
  editorType?: string;
  config?: any;
  onEditorFocus?: (e: React.FocusEvent, controller?: HtmlEditorController) => void;
  onEditorBlur?: (e: React.FocusEvent, controller?: HtmlEditorController) => void;
  innerRef?: React.Ref<any>;
  controller?: HtmlEditorController;
};

export namespace Options {
  export let EditorFactory: ((props: HtmlEditorFactoryProps) => React.ReactElement | null) | undefined = undefined
  export let ControllerFactory: (() => HtmlEditorController) | undefined = undefined;
}
