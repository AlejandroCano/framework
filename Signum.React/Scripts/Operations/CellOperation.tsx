import * as React from "react"
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { Entity, JavascriptMessage, OperationMessage, SearchMessage, Lite, External } from '../Signum.Entities';
import { getTypeInfo, OperationType } from '../Reflection';
import { classes } from '../Globals';
import * as Navigator from '../Navigator';
import MessageModal from '../Modals/MessageModal'
import {
  operationInfos, getSettings, notifySuccess, API, Defaults, CellOperationContext, EntityOperationSettings
} from '../Operations'
import * as Operations from "../Operations";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { Button, ButtonProps, Dropdown, OverlayTrigger, Tooltip } from "react-bootstrap";
import { MultiPropertySetterModal, PropertySetterComponentProps } from "./MultiPropertySetter";
import { BsColor } from "../Components";
import Exception from "../Exceptions/Exception";
import { OutlineBsColor, withIcon } from "./EntityOperations";
import { CellFormatter } from "../Finder";
import { useDoubleClick } from "../Hooks";


export interface CellOperationProps extends ButtonProps {
  coc: CellOperationContext;
  onOperationClick?: (coc: CellOperationContext) => Promise<void>;
  variant?: BsColor;
  className?: string;
  children?: React.ReactNode;
  color?: BsColor;
  icon?: IconProp;
  iconColor?: string;
  iconAlign?: "start" | "end";
  outline?: boolean;
}

export function CellOperationButton({ coc: cocOrNull, onOperationClick, outline, color, icon, iconColor, iconAlign, ...props }: CellOperationProps) {

  if (cocOrNull == null)
    return null;

  const coc = cocOrNull;

  if (!coc.isVisibleInCell())
    return null;

  const canExecute = coc.settings?.overrideCanExecute ? coc.settings.overrideCanExecute(coc) : coc.canExecute;
  const disabled = !!canExecute;

  const eos = coc.entityOperationSettings;

  if (color == null)
    color = coc.settings?.color ?? eos?.color ?? Defaults.getColor(coc.operationInfo);

  if (icon == null)
    icon = coalesceIcon(coc.settings?.icon, eos?.icon);

  if (iconColor == null)
    iconColor = coc.settings?.iconColor || eos?.iconColor;

  if (outline == null)
    outline = coc.outline ?? coc.settings?.outline;

  if (iconAlign == null)
    iconAlign = coc.iconAlign ?? coc.settings?.iconAlign;

  const customOrSettingsOrDefaultClick = onOperationClick ?? coc.settings?.onClick ?? eos?.commonOnClick ?? defaultCellOperationClick

  const handleOnClick = (event: React.MouseEvent<any>) => {
    coc.event = event;
    event.persist();
    customOrSettingsOrDefaultClick(coc)
      .done();
  }

  const onlySingleClick = useDoubleClick((e) => { }, handleOnClick);

  var button = <Button variant={(outline ? ("outline-" + color) as OutlineBsColor : color)}
    {...props}
    key="button"
    //title={icoc.operationInfo.niceName}
    className={classes(disabled ? "disabled" : undefined, props?.className, coc.settings && coc.settings.classes)}
    onClick={disabled ? undefined : onlySingleClick}
    data-operation={coc.operationInfo.key}>
    {renderChildren()}
  </Button>;

  if (canExecute) {
    return (
      <OverlayTrigger overlay={<Tooltip id={coc.operationInfo.key + "_tooltip"} placement={"bottom"}>{canExecute}</Tooltip>}>
        {button}
      </OverlayTrigger>
    );
  }

  return button;

  function renderChildren() {
    if (props.children)
      return props.children;

    let text: string = coc.settings?.text ? coc.settings.text(coc) : coc.operationInfo.niceName;

    return withIcon(text, icon, iconColor, iconAlign);
  }
}

CellOperationButton.getText = (icoc: CellOperationContext): React.ReactNode => {

  if (icoc.settings && icoc.settings.text)
    return icoc.settings.text(icoc);

  return <>{CellOperationButton.simplifyName(icoc.operationInfo.niceName)}{icoc.operationInfo.canBeModified ? <small className="ms-2">{OperationMessage.MultiSetter.niceToString()}</small> : null}</>;

};

CellOperationButton.simplifyName = (niceName: string) => {
  const array = new RegExp(OperationMessage.CreateFromRegex.niceToString()).exec(niceName);
  return array ? (niceName.tryBefore(array[1]) ?? "") + array[1].firstUpper() : niceName;
}

function confirmInNecessary(coc: CellOperationContext): Promise<boolean> {

  const confirmMessage = getConfirmMessage(coc);

  if (confirmMessage == undefined)
    return Promise.resolve(true);

  return MessageModal.show({
    title: OperationMessage.Confirm.niceToString(),
    message: confirmMessage,
    buttons: "yes_no",
    icon: "warning",
    style: "warning",
  }).then(result => { return result == "yes"; });
}

function getConfirmMessage(coc: CellOperationContext) {
  if (coc.settings && coc.settings.confirmMessage === null)
    return undefined;

  if (coc.settings && coc.settings.confirmMessage != undefined)
    return coc.settings.confirmMessage(coc);

  if (coc.operationInfo.operationType == "Delete") {
    const lite = coc.lite;
    if (lite) {
      return OperationMessage.PleaseConfirmYouWouldLikeToDelete0FromTheSystem.niceToString().formatHtml(<strong>{lite.toStr} ({getTypeInfo(lite.EntityType).niceName} {lite.id})</strong>);;
    }
  }

  return undefined;
}


export function defaultCellOperationClick(coc: CellOperationContext, ...args: any[]): Promise<void> {

  coc.event!.persist();

  return confirmInNecessary(coc).then(conf => {
    if (!conf)
      return;

    switch (coc.operationInfo.operationType) {
      case "ConstructorFromMany":
        throw new Error("ConstructorFromMany operation can not be in column");

      case "ConstructorFrom":
        if (coc.lite) {
          return API.constructFromLite(coc.lite, coc.operationInfo.key, ...args)
            .then(coc.onConstructFromSuccess ?? (pack => {
              if (pack?.entity.id != null)
                Navigator.raiseEntityChanged(pack.entity);
              notifySuccess();
              return Navigator.createNavigateOrTab(pack, coc.event!)
                .then(() => { })
            }));
        };
      case "Execute":
        return API.executeLite(coc.lite, coc.operationInfo.key, ...args)
          .then(coc.onExecuteSuccess ?? (pack => {
            coc.cellContext.refresh?.();
            coc.raiseEntityChanged();
            notifySuccess();
          }));
      case "Delete":
        return API.deleteLite(coc.lite, coc.operationInfo.key, ...args)
          .then(coc.onDeleteSuccess ?? (() => {
            coc.cellContext.refresh?.();
            coc.raiseEntityChanged();
            notifySuccess();
          }));
    }
  });

}


export function coalesceIcon(icon: IconProp | undefined, icon2: IconProp | undefined): IconProp | undefined{ //Till the error is fixed

  if (icon === null)
    return undefined;

  if (icon === undefined)
    return icon2

  return icon;
}

