﻿import * as React from 'react'
import { FormGroup, FormControlStatic, ValueLine, ValueLineType, EntityLine, EntityCombo, EntityList, EntityRepeater } from '../../../../Framework/Signum.React/Scripts/Lines'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { QueryDescription, SubTokensOptions } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName } from '../../../../Framework/Signum.React/Scripts/Reflection'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import { TypeContext, FormGroupStyle } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import CodeMirrorComponent from '../../../../Extensions/Signum.React.Extensions/Codemirror/CodeMirrorComponent'
import { EvalEntity } from '../Signum.Entities.Dynamic'
import * as CodeMirror from 'codemirror'
import { Expression } from './Nodes'

require("!style!css!codemirror/lib/codemirror.css");
require("!style!css!codemirror/addon/dialog/dialog.css");
require("!style!css!codemirror/addon/display/fullscreen.css");
require("!style!css!codemirror/addon/hint/show-hint.css");


require("codemirror/lib/codemirror");
require("codemirror/mode/javascript/javascript");
require("codemirror/addon/comment/comment");
require("codemirror/addon/comment/continuecomment");
require("codemirror/addon/dialog/dialog");
require("codemirror/addon/display/fullscreen");
require("codemirror/addon/edit/closebrackets");
require("codemirror/addon/edit/matchbrackets");
require("codemirror/addon/hint/show-hint");
require("codemirror/addon/hint/javascript-hint");
require("codemirror/addon/search/match-highlighter");
require("codemirror/addon/search/search");
require("codemirror/addon/search/searchcursor");

interface ExpressionComponentProps {
    expression: Expression<any>;
    typeName: string;
    onChange?: () => void;
}

export default class ExpressionComponent extends React.Component<ExpressionComponentProps, void> {

    handleOnChange = (newValue: string) => {
        this.props.expression.code = newValue;

        if (this.props.onChange)
            this.props.onChange();
    };


    codeMirrorComponent: CodeMirrorComponent;

    render() {
        const ctx = this.props.expression;

        const options = {
            lineNumbers: true,
            mode: "javascript",
            extraKeys: {
                "Ctrl-Space": "autocomplete",
                "Ctrl-K": (cm: any) => cm.lineComment(cm.getCursor(true), cm.getCursor(false)),
                "Ctrl-U": (cm: any) => cm.uncomment(cm.getCursor(true), cm.getCursor(false)),
                "Ctrl-I": (cm: any) => cm.autoFormatRange(cm.getCursor(true), cm.getCursor(false)),
                "F11": (cm: any) => cm.setOption("fullScreen", !cm.getOption("fullScreen")),
                "Esc": (cm: any) => {
                    if (cm.getOption("fullScreen"))
                        cm.setOption("fullScreen", false);
                }
            }
        } as CodeMirror.EditorConfiguration;

        (options as any).highlightSelectionMatches = true;
        (options as any).matchBrackets = true;

        return (
            <div>
                <pre>{"function (e: " + this.props.typeName + ") {"}</pre>
                <CodeMirrorComponent value={this.props.expression.code} ref={cm => this.codeMirrorComponent = cm}
                    options={options}
                    onChange={this.handleOnChange} />
                <pre>{"}"}</pre>
            </div>
        );
    }
}