import * as React from 'react'
import { DropdownButton, MenuItem, OverlayTrigger, Tooltip } from 'react-bootstrap'
import { Dic, DomUtils, classes } from '../Globals'
import * as Finder from '../Finder'
import {
    ResultTable, ResultRow, FindOptions, FindOptionsParsed, FilterOption, QueryDescription, ColumnOption, ColumnOptionsMode, ColumnDescription,
    toQueryToken, Pagination, PaginationMode, OrderType, OrderOption, SubTokensOptions, filterOperations, QueryToken, QueryCountRequest, QueryRequest, QueryTokenType
} from '../FindOptions'
import { SearchMessage, JavascriptMessage, Lite, liteKey, is, Entity, isEntity, EntityControlMessage, isLite } from '../Signum.Entities'
import { getTypeInfos, IsByAll, getQueryKey, TypeInfo, EntityData, getQueryNiceName  } from '../Reflection'
import * as Navigator from '../Navigator'
import { StyleContext, TypeContext } from '../Typecontext'
import ValueSearchControl from './ValueSearchControl'
import { LineBase, LineBaseProps, FormGroup, FormControlStatic, runTasks } from '../Lines/LineBase'

export interface ValueSearchControlLineProps extends React.Props<ValueSearchControlLine> {
    ctx: StyleContext;
    findOptions?: FindOptions;
    valueToken?: string;
    labelText?: React.ReactChild;
    labelProps?: React.HTMLAttributes;
    formGroupHtmlProps?: React.HTMLAttributes;
    initialValue?: any;
    isLink?: boolean;
    isBadge?: boolean | "MoreThanZero";
    isFormControl?: boolean;
    findButton?: boolean;
    onViewEntity?: (entity: Lite<Entity>)=> void;
    viewEntityButton?: boolean;
    avoidAutoRefresh?: boolean;
    extraButtons?: (valueSearchControl: ValueSearchControl) => React.ReactNode | undefined
}


export default class ValueSearchControlLine extends React.Component<ValueSearchControlLineProps, void> {


    valueSearchControl?: ValueSearchControl;

    handleValueSearchControlLoaded = (vsc: ValueSearchControl | undefined) => {

        if (vsc != this.valueSearchControl)
            this.forceUpdate();

        this.valueSearchControl = vsc;
    }

    getFindOptions(props: ValueSearchControlLineProps): FindOptions {
        if (props.findOptions)
            return props.findOptions;

        var ctx = props.ctx as TypeContext<any>;

        if (isEntity(ctx.value))
            return {
                queryName: ctx.value.Type,
                parentColumn: "Entity",
                parentValue: ctx.value
            };

        throw new Error("Impossible to determine 'findOptions' because 'ctx' is not a 'TypeContext<Entity>'. Set it explicitly");
    }

    refreshValue() {
        this.valueSearchControl && this.valueSearchControl.refreshValue()
    }

    render() {

        var fo = this.getFindOptions(this.props);

        if (!Finder.isFindable(fo.queryName))
            return null;

        var token = this.valueSearchControl && this.valueSearchControl.state.token;

        let isQuery = this.props.valueToken == undefined || token && token.queryTokenType == "Aggregate";

        let isBadge = coallesce(this.props.isBadge, this.props.valueToken == undefined ? "MoreThanZero" as "MoreThanZero" : false);
        let isFormControl = coallesce(this.props.isFormControl, this.props.valueToken != undefined);

        let unit = isFormControl && token && token.unit && <span className="input-group-addon">{token.unit}</span>;


        let value = this.valueSearchControl && this.valueSearchControl.state.value;
        let find = value != undefined && coallesce(this.props.findButton, isQuery) &&
            <a className={classes("sf-line-button", "sf-find", isFormControl ? "btn btn-default" : undefined)}
                onClick={this.valueSearchControl!.handleClick}
                title={EntityControlMessage.Find.niceToString()}>
                <span className="glyphicon glyphicon-search" />
            </a>;


        let view = value != undefined && coallesce(this.props.viewEntityButton, isLite(value) && Navigator.isViewable(value.EntityType)) &&
            <a className={classes("sf-line-button", "sf-view", isFormControl ? "btn btn-default" : undefined)}
                onClick={this.handleViewEntityClick}
                title={EntityControlMessage.View.niceToString()}>
                <span className="glyphicon glyphicon-arrow-right" />
            </a>

        let extra = this.valueSearchControl && this.props.extraButtons && this.props.extraButtons(this.valueSearchControl);

        return (
            <FormGroup ctx={this.props.ctx}
                labelText={this.props.labelText || (token ? token.niceName : getQueryNiceName(fo.queryName))}
                labelProps={this.props.labelProps}
                htmlProps={this.props.formGroupHtmlProps}>
                <div className={isFormControl && (unit ||view || extra || find) ? "input-group" : undefined}>
                    <ValueSearchControl
                        ref={this.handleValueSearchControlLoaded}
                        findOptions={fo}
                        initialValue={this.props.initialValue}
                        isBadge={isBadge}
                        isLink={this.props.isLink}
                        formControlClass={isFormControl ? this.props.ctx.formControlClassReadonly : undefined}
                        valueToken={this.props.valueToken}
                        onValueChange={() => this.forceUpdate()}
                        onTokenLoaded={() => this.forceUpdate()}
                        />
                    {unit}
                    {(view || extra || find) && (isFormControl ?
                        <div className="input-group-btn">
                            {view}
                            {find}
                            {extra}
                        </div> : <span>
                            {view}
                            {find}
                            {extra}
                        </span>) }
                </div>
            </FormGroup>
        );
    }

    handleViewEntityClick = (e: React.MouseEvent) => {
        e.preventDefault();

        var entity = this.valueSearchControl!.state.value as Lite<Entity>;
        if (this.props.onViewEntity)
            this.props.onViewEntity(entity);
        
        Navigator.navigate(entity);
    }
}

function coallesce<T>(propValue: T | undefined, defaultValue: T): T {
    if (propValue !== undefined)
        return propValue;

    return defaultValue;
}
