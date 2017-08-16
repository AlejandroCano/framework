﻿import * as React from "react";
import { TypeContext, LineBaseProps, LineBase, FormGroup } from "../Lines";
import { SearchMessage, MList, newMListElement } from "../Signum.Entities";
import * as Constructor from "../Constructor";
import { Binding, New } from "../Reflection";
import { mlistItemContext } from "../TypeContext";



interface MultiValueLineProps extends LineBaseProps {
    ctx: TypeContext<MList<any>>;    
    onRenderItem: (ctx: TypeContext<any>) => React.ReactElement<any>;
    onCreate?: () => Promise<any>;
    addValueText?: string;
}

export class MultiValueLine extends LineBase<MultiValueLineProps, MultiValueLineProps> {
    calculateDefaultState(state: MultiValueLineProps) {

        if (state.ctx.value == undefined)
            state.ctx.value = [];

        super.calculateDefaultState(state);
    }


    handleDeleteValue = (index: number) => {
        const list = this.state.ctx.value;
        list.removeAt(index);
        this.setValue(list);
    }

    handleAddValue = () => {
        const list = this.state.ctx.value;
        const newValuePromise = this.state.onCreate == null ? this.defaultCreate() : this.state.onCreate();
        newValuePromise.then(v => {
            if (v === undefined)
                return undefined;

            list.push(newMListElement(v));
            this.setValue(list);
        }).done();
    }

    defaultCreate() {
        return Constructor.construct(this.state.type!.name).then(a => a && a.entity);
    }

    renderInternal() {

        const s = this.state;
        const list = this.state.ctx.value!;        

        return (
            <FormGroup ctx={s.ctx} labelText={s.labelText}
                htmlAttributes={{ ...this.baseHtmlAttributes(), ...this.state.formGroupHtmlAttributes }}
                helpBlock={this.state.helpBlock}
                labelHtmlAttributes={s.labelHtmlAttributes}>
                <table className="sf-multi-value">
                    <tbody>
                        {
                            mlistItemContext(s.ctx.subCtx({ formGroupStyle: "None" })).map((mlec, i) =>
                                <tr key={i}>
                                    <td>
                                        {!s.ctx.readOnly &&
                                            <a title={SearchMessage.DeleteFilter.niceToString()}
                                                className="sf-line-button sf-remove"
                                                onClick={() => this.handleDeleteValue(i)}>
                                                <span className="glyphicon glyphicon-remove" />
                                            </a>}
                                    </td>
                                    <td>
                                        { this.props.onRenderItem(mlec) }
                                    </td>
                                </tr>)
                        }
                        <tr >
                            <td colSpan={4}>
                                {!s.ctx.readOnly &&
                                    <a title={this.props.addValueText || SearchMessage.AddValue.niceToString()}
                                        className="sf-line-button sf-create"
                                        onClick={this.handleAddValue}>
                                    <span className="glyphicon glyphicon-plus sf-create sf-create-label" />{this.props.addValueText || SearchMessage.AddValue.niceToString()}
                                    </a>}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </FormGroup>
        );
    }
}


