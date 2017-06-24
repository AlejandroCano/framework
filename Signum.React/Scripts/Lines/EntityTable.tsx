﻿import * as React from 'react'
import { classes, Dic } from '../Globals'
import * as Navigator from '../Navigator'
import * as Constructor from '../Constructor'
import * as Finder from '../Finder'
import { FindOptions } from '../FindOptions'
import { TypeContext, StyleContext, StyleOptions, FormGroupStyle, mlistItemContext, EntityFrame } from '../TypeContext'
import { PropertyRoute, PropertyRouteType, MemberInfo, getTypeInfo, getTypeInfos, TypeInfo, IsByAll, ReadonlyBinding, LambdaMemberType, Type } from '../Reflection'
import { LineBase, LineBaseProps, FormGroup, FormControlStatic, runTasks, } from '../Lines/LineBase'
import { ModifiableEntity, Lite, Entity, MList, MListElement, EntityControlMessage, JavascriptMessage, toLite, is, liteKey, getToString } from '../Signum.Entities'
import Typeahead from '../Lines/Typeahead'
import { EntityBase } from './EntityBase'
import { EntityListBase, EntityListBaseProps } from './EntityListBase'
import DynamicComponent from './DynamicComponent'
import { RenderEntity } from './RenderEntity'

export interface EntityTableProps extends EntityListBaseProps {
    createAsLink?: boolean | ((er: EntityTable) => React.ReactElement<any>);
    /**Consider using EntityTable.typedColumns to get Autocompletion**/
    columns?: EntityTableColumn<ModifiableEntity, any>[],
    fetchRowState?: (ctx: TypeContext<ModifiableEntity>, row: EntityTableRow) => Promise<any>;
    onRowHtmlAttributes?: (ctx: TypeContext<ModifiableEntity>, row: EntityTableRow, rowState: any) => React.HTMLAttributes<any> | null | undefined;
}

export interface EntityTableColumn<T, RS> {
    property?: ((a: T) => any) | string;
    header?: React.ReactNode | null;
    headerHtmlAttributes?: React.HTMLAttributes<any>;
    cellHtmlAttributes?: (ctx: TypeContext<T>, row: EntityTableRow, rowState: RS) => React.HTMLAttributes<any> | null | undefined;
    template?: (ctx: TypeContext<T>, row: EntityTableRow, rowState: RS) => React.ReactChild | null | undefined;
}

export class EntityTable extends EntityListBase<EntityTableProps, EntityTableProps> {

    static typedColumns<T extends ModifiableEntity>(columns: (EntityTableColumn<T, any> | null | undefined)[]): EntityTableColumn<T, any>[] {
        return columns.filter(a => a != null).map(a => a!);
    }

    static typedColumnsWithRowState<T extends ModifiableEntity, RS>(columns: (EntityTableColumn<T, RS> | null | undefined)[]): EntityTableColumn<T, RS>[]{
        return columns.filter(a => a != null).map(a => a!);
    }

    calculateDefaultState(state: EntityTableProps) {
        super.calculateDefaultState(state);
        state.viewOnCreate = false;
        state.createAsLink = true;
    }

    overrideProps(state: EntityTableProps, overridenProps: EntityTableProps) {
        super.overrideProps(state, overridenProps);
        
        if (!state.columns) {
            var elementPr = state.ctx.propertyRoute.add(a => a[0].element);

            state.columns = Dic.getKeys(elementPr.subMembers())
                .filter(a => a != "Id")
                .map(memberName => ({
                    property: eval("(function(e){ return e." + memberName.firstLower() + "; })")
                }) as EntityTableColumn<ModifiableEntity, any>);
        }
    }


    renderInternal() {

        const buttons = (
            <span className="pull-right">
                {this.state.createAsLink == false && this.renderCreateButton(false)}
                {this.renderFindButton(false)}
            </span>
        );

        if (this.state.type!.isLite)
            throw new Error("Lite not supported");

        let ctx = (this.state.ctx as TypeContext<MList<ModifiableEntity>>).subCtx({ formGroupStyle: "SrOnly" });

        const readOnly = this.state.ctx.readOnly;

        const elementPr = ctx.propertyRoute.add(a => a[0].element);

        return (
            <fieldset className={classes("SF-table-field SF-control-container", ctx.errorClass)} {...this.baseHtmlAttributes() } {...this.state.formGroupHtmlAttributes}>
                <legend>
                    <div>
                        <span>{this.state.labelText}</span>
                        {EntityBase.hasChildrens(buttons) ? buttons : undefined}
                    </div>
                </legend>
                <table className="table table-condensed form-vertical">
                    <thead>
                        <tr>
                            <th></th>
                            {
                                this.state.columns!.map((c, i) => <th key={i} {...c.headerHtmlAttributes}>
                                    {c.header === undefined && c.property ? elementPr.add(c.property).member!.niceName : c.header}
                                </th>)
                            }
                        </tr>
                    </thead>
                    <tbody>
                        {
                            mlistItemContext(ctx).map((mlec, i) =>
                                (<EntityTableRow key={i} index={i}
                                    onRowHtmlAttributes={this.props.onRowHtmlAttributes}
                                    fetchRowState={this.props.fetchRowState}
                                    onRemove={this.canRemove(mlec.value) && !readOnly ? e => this.handleRemoveElementClick(e, i) : undefined}
                                    onMoveDown={this.canMove(mlec.value) && !readOnly ? e => this.moveDown(i) : undefined}
                                    onMoveUp={this.canMove(mlec.value) && !readOnly ? e => this.moveUp(i) : undefined}
                                    columns={this.state.columns!}
                                    ctx={mlec} />))
                        }
                        {
                            this.state.createAsLink && this.state.create && !readOnly &&
                            <tr>
                                <td colSpan={1 + this.state.columns!.length}>
                                    {typeof this.state.createAsLink == "function" ? this.state.createAsLink(this) :
                                        <a title={EntityControlMessage.Create.niceToString()}
                                            className="sf-line-button sf-create"
                                            onClick={this.handleCreateClick}>
                                            <span className="glyphicon glyphicon-plus sf-create sf-create-label" />{EntityControlMessage.Create.niceToString()}
                                        </a>}
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </fieldset>
        );
    }
}


export interface EntityTableRowProps {
    ctx: TypeContext<ModifiableEntity>;
    index: number;
    columns: EntityTableColumn<ModifiableEntity, any>[],
    onRemove?: (event: React.MouseEvent<any>) => void;
    onMoveUp?: (event: React.MouseEvent<any>) => void;
    onMoveDown?: (event: React.MouseEvent<any>) => void;
    fetchRowState?: (ctx: TypeContext<ModifiableEntity>, row: EntityTableRow) => Promise<any>;
    onRowHtmlAttributes?: (ctx: TypeContext<ModifiableEntity>, row: EntityTableRow, rowState: any) => React.HTMLAttributes<any> | null | undefined;
}

export class EntityTableRow extends React.Component<EntityTableRowProps, { rowState?: any }> {

    constructor(props: EntityTableRowProps) {
        super(props);

        this.state = {}; 

        if (props.fetchRowState)
            props.fetchRowState(this.props.ctx, this)
                .then(val => this.setState({ rowState: val }))
                .done();
    }

    render() {
        var ctx = this.props.ctx;
        var rowAtts = this.props.onRowHtmlAttributes && this.props.onRowHtmlAttributes(ctx, this, this.state.rowState);
        return (
            <tr style={{ backgroundColor: rowAtts && rowAtts.style && rowAtts.style.backgroundColor }}>
                <td>
                    <div className="item-group">
                        {this.props.onRemove && <a className={classes("sf-line-button", "sf-remove")}
                            onClick={this.props.onRemove}
                            title={EntityControlMessage.Remove.niceToString()}>
                            <span className="glyphicon glyphicon-remove"/>
                        </a>}

                        {this.props.onMoveUp && <a className={classes("sf-line-button", "move-up")}
                            onClick={this.props.onMoveUp}
                            title={EntityControlMessage.MoveUp.niceToString()}>
                            <span className="glyphicon glyphicon-chevron-up"/>
                        </a>}

                        {this.props.onMoveDown && <a className={classes("sf-line-button", "move-down")}
                            onClick={this.props.onMoveDown}
                            title={EntityControlMessage.MoveDown.niceToString()}>
                            <span className="glyphicon glyphicon-chevron-down"/>
                        </a>}
                    </div>
                </td>
                {this.props.columns.map((c, i) => <td key={i} {...c.cellHtmlAttributes && c.cellHtmlAttributes(ctx, this, this.state.rowState) }>{this.getTemplate(c)}</td>)}
            </tr>
        );
    }


    getTemplate(col: EntityTableColumn<ModifiableEntity, any>): React.ReactChild | undefined | null {

        if (col.template === null)
            return null;

        if (col.template !== undefined)
            return col.template(this.props.ctx, this, this.state.rowState);

        if (col.property == null)
            throw new Error("Column has no property and no template");

        return DynamicComponent.getAppropiateComponent(this.props.ctx.subCtx(col.property));
    }
}