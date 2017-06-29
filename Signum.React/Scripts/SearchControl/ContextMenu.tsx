﻿import * as React from 'react'
import { Dic, classes, combineFunction, DomUtils } from '../Globals'
import * as RootCloseWrapper from 'react-overlays/lib/RootCloseWrapper'


export interface ContextMenuPosition {
    left: number;
    top: number;
    width: number; //Necessary for RTL
}

export interface ContextMenuProps extends React.Props<ContextMenu>, React.HTMLAttributes<HTMLUListElement> {
    position: ContextMenuPosition;
    onHide: () => void;
}

export default class ContextMenu extends React.Component<ContextMenuProps, {}> {

    static getPosition(e: React.MouseEvent<any>, container: HTMLElement): ContextMenuPosition{

        const op = DomUtils.offsetParent(container);

        const rec = op && op.getBoundingClientRect();

        var result = ({
            left: e.clientX - (rec ? rec.left : 0),
            top: e.clientY - (rec ? rec.top : 0),
            width: (op ? op.offsetWidth : window.innerWidth)
        }) as ContextMenuPosition;

        return result;
    }
    
    render() {
        
        const { position, onHide, ref, ...props } = this.props;

        const style: React.CSSProperties = { zIndex: 999, display: "block", position: "absolute" };

        style.top = position.top + "px";
        if (document.body.className.contains("rtl-mode"))
            style.right = (position.width - position.left) + "px";
        else
            style.left = position.left + "px";

        const childrens = React.Children.map(this.props.children,
            (c: React.ReactElement<any>) => c && React.cloneElement(c, { "onSelect": combineFunction(c.props.onSelect, onHide) }));

        const ul = (
            <ul {...props as any} className={classes(props.className, "dropdown-menu sf-context-menu") } style={style}>
                {childrens}
            </ul>
        );

        return <RootCloseWrapper onRootClose={onHide}>{ul}</RootCloseWrapper>;
    }
}