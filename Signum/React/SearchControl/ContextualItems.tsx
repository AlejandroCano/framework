import * as React from 'react'
import { QueryDescription, } from '../FindOptions'
import { Lite, Entity } from '../Signum.Entities'
import SearchControlLoaded from "./SearchControlLoaded";
import { StyleContext } from '../TypeContext';
import { Dropdown } from 'react-bootstrap';


export interface ContextualMenuItem {
  fullText?: string; //used for filtering
  menu : React.ReactElement<any>;
}

export interface MenuItemBlock {
  header: string;
  menuItems: ContextualMenuItem[];
}

export interface ContextualItemsContext<T extends Entity> {
  lites: Lite<T>[];
  queryDescription: QueryDescription;
  markRows: (dictionary: MarkedRowsDictionary) => void;
  container?: SearchControlLoaded | React.Component<any, any>;
  styleContext?: StyleContext;
}

export interface MarkedRowsDictionary {
  [liteKey: string]: string | MarkedRow;
}

export interface MarkedRow {
  status: "Error" | "Warning" | "Success" | "Muted";
  message?: string;
}

export function clearContextualItems() {
  onContextualItems.clear();
}

export const onContextualItems: ((ctx: ContextualItemsContext<Entity>) => Promise<MenuItemBlock | undefined> | undefined)[] = [];

export function renderContextualItems(ctx: ContextualItemsContext<Entity>): Promise<ContextualMenuItem[]> {

  const blockPromises = onContextualItems.map(func => func(ctx));

  return Promise.all(blockPromises).then(blocks => {

    const result: ContextualMenuItem[] = []
    blocks.forEach(block => {

      if (block == undefined || block.menuItems == undefined || block.menuItems.length == 0)
        return;

      if (result.length)
        result.push({ menu: <Dropdown.Divider /> });

      if (block.header)
        result.push({ menu: <Dropdown.Header>{block.header}</Dropdown.Header> });

      if (block.header)
        result.splice(result.length, 0, ...block.menuItems);
    });

    return result;
  });
}




