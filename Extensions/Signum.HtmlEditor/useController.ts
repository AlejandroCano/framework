import { IBinding } from "@framework/Reflection";
import React from "react";
import { BasicCommandsExtensions } from "./Extensions/BasicCommandsExtension";
import { CodeBlockExtension } from "./Extensions/CodeBlockExtension";
import { ListExtension } from "./Extensions/ListExtension";
import { OnChangeExtension } from "./Extensions/OnChangeExtension";
import {
  ComponentAndProps,
  HtmlEditorExtension,
  LexicalConfigNode,
} from "./Extensions/types";
import {
  HtmlContentStateConverter,
  ITextConverter,
} from "./HtmlContentStateConverter";
import { HtmlEditorProps } from "./HtmlEditor";
import { HtmlEditorController, HtmlEditorControllerProps } from "./HtmlEditorController";
import { useRegisterExtensions } from "./useRegisterExtensions";
import { useRegisterKeybindings } from "./useRegisterKeybindings";
import { Options } from "./HtmlEditorClient";

type ControllerReturnType = {
  controller: HtmlEditorController;
  nodes: LexicalConfigNode;
  builtinComponents: ComponentAndProps[];
};

export const useController = (p: HtmlEditorControllerProps): ControllerReturnType => {
  const {
    binding,
    editableId,
    readOnly,
    small,
    converter,
    innerRef,
    plugins,
    initiallyFocused,
    handleKeybindings
  } = p;

  const factory = Options.ControllerFactory;

  const controller = React.useMemo(() => {

    const ctrlr = factory ? factory() : null;

    if (!ctrlr) {
      const lexicalModule = require("./LexicalHtmlEditorController") as typeof import("./LexicalHtmlEditorController");
      ctrlr = new lexicalModule.LexicalHtmlEditorController();      
    }

//    ctrlr!.init(p)

    return ctrlr!;

  }, [factory]);

  const textConverter = converter ?? new HtmlContentStateConverter();

  const extensions: HtmlEditorExtension[] = factory ? [] : React.useMemo(() => {
    const defaultPlugins = [
      new BasicCommandsExtensions(),
      new ListExtension(),
      new OnChangeExtension(),
      new CodeBlockExtension(),
    ];

    return plugins ? [...defaultPlugins, ...plugins] : defaultPlugins;
  }, [plugins]);

  React.useEffect(() => {

    if (!controller?.editor)
      return;

    if (typeof controller.editor.setEditable === "function")
      controller.editor.setEditable(!readOnly);

  }, [controller, readOnly]);

  // Move controller initialization (side-effect) into useEffect to avoid mutating during render.
  React.useEffect(() => {
    if (!controller)
      return;

    controller.init({
      ...p,
      converter: textConverter,
      plugins: extensions,
    });
    // Intentionally not returning a cleanup because controller lifecycle is managed elsewhere.
    // Depend on values that affect init to re-run if they change.
  }, [
    controller,
    textConverter,
    extensions,
    binding,
    editableId,
    small,
    innerRef,
    initiallyFocused,
    handleKeybindings
  ]); 

  const nodes = React.useMemo(() => {
    return extensions.flatMap((plugin) => plugin.getNodes?.() ?? []);
  }, [extensions]);

  const builtinComponents = React.useMemo(() => {
    return extensions.map((plugin) => plugin.getBuiltInComponent?.()).notNull();
  }, [extensions]);

  return { controller, nodes, builtinComponents };
};
