import * as React from "react";
import { IBinding } from "@framework/Reflection";
import { HtmlEditorExtension } from "./Extensions/types";
import { ITextConverter, HtmlContentStateConverter } from "./HtmlContentStateConverter";
import { Separator } from "./HtmlEditorButtons";

export interface HtmlEditorControllerProps {
  binding: IBinding<string | null | undefined>;
  editableId: string;
  readOnly?: boolean;
  small?: boolean;
  converter: ITextConverter;
  innerRef?: React.Ref<any>;
  plugins?: HtmlEditorExtension[];
  initiallyFocused?: boolean | number;
  // allow controllers to receive keybinding handler
  handleKeybindings?: (event: KeyboardEvent) => boolean;
}

/**
 * Base abstract controller.
 * Subclasses should implement `init` and `saveHtml` according to the concrete editor.
 */
export abstract class HtmlEditorController {
  // Editor instance (concrete type provided by subclass)
  editor: any;
  editableElement: HTMLElement | null = null;
  editorState: any;

  overrideToolbar!: React.ReactElement | undefined;
  setOverrideToolbar!: (newState: React.ReactElement | undefined) => void;

  converter!: ITextConverter;
  plugins!: HtmlEditorExtension[];
  binding!: IBinding<string | null | undefined>;
  readOnly?: boolean;
  small?: boolean;
  initialEditorContent?: string;

  lastSavedString?: { str: string | null };

  /**
   * Initialize controller lifecycle. Implemented by subclasses.
   * `p.plugins` contains the computed `extensions` array from useController.
   */
  abstract init(p: HtmlEditorControllerProps): void;


  extraButtons(): React.ReactElement | null {
    const buttons = (this.plugins ?? [])
      .map((p) => p.getToolbarButtons?.(this))
      .notNull();

    if (buttons.length == 0) return null;

    return React.createElement(React.Fragment, undefined, <Separator/>, ...buttons);
  }

  setRefs!: (editor: any | null) => void;
}
