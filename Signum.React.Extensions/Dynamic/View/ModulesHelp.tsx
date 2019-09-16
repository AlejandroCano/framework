import * as React from 'react'
import { Dic } from '@framework/Globals';
import { DynamicViewMessage } from '../Signum.Entities.Dynamic';
import ValueLineModal from '@framework/ValueLineModal';
import { UncontrolledDropdown, DropdownToggle, DropdownMenu, DropdownItem } from '@framework/Components';
import { globalModules } from '../../Dynamic/View/GlobalModules';

export function ModulesHelp(p: { cleanName: string; clientCode?: boolean; }) {

  var modules: { [name: string]: string; } = {};

  modules["numbro"] = "";
  modules["moment"] = `modules.moment(dateValue1).diff(modules.moment(dateValue2), "days");
modules.moment(dateValue).fromNow();
ctx.value.applyDate = modules.moment.format();
modules.moment().format("L LT"); /* "2019/06/25 14:21", other formats are:  L LL LLL LLLL */
modules.moment("date string");

/* See: https://momentjs.com/docs/ */
`;
  modules["React"] = `const [count, setCount] = modules.React.useState(0);

const element = modules.React.useRef(null); 
/* Usage in code */
locals.element.current && locals.element.current.[Method Name];

modules.React.useEffect(() => {
  // do something here...
});`;
  modules["Components"] = "";
  modules["Globals"] = "";
  modules["Navigator"] = `modules.Navigator.navigate(e);
modules.Navigaor.view(e);
modules.Navigator.API.fetchEntity("${p.cleanName}", [id]).then(entity => { /* do something here ... */ }).done();
modules.Navigator.API.fetchAndForget([lite]).then(entity => { /* do something here ... */ }).done();

NOTE: fetchAndRemember stores the result entity in lite.Entity.
modules.Navigator.API.fetchAndRemember([lite]).then(entity => { /* do something here ... */ }).done();
`;
  modules["Finder"] = `modules.Finder.find("${p.cleanName}");
modules.Finder.findMany("${p.cleanName}");
modules.Finder.fetchEntitiesWithFilters("${p.cleanName}", 
  [{ token: "...", operation: "...", value: "..." }] ,  /* filterOptions */ 
  [{ token: "...", orderType: "..." }],  /* orderOptions */
  1 /* count */);

//type OrderType =
//  "Ascending" |
//  "Descending";

//type FilterOperation =
//  "EqualTo" |
//  "DistinctTo" |
//  "GreaterThan" |
//  "GreaterThanOrEqual" |
//  "LessThan" |
//  "LessThanOrEqual" |
//  "Contains" |
//  "StartsWith" |
//  "EndsWith" |
//  "Like" |
//  "NotContains" |
//  "NotStartsWith" |
//  "NotEndsWith" |
//  "NotLike" |
//  "IsIn" |
//  "IsNotIn";
`;
  modules["Reflection"] = `modules.Reflection.getTypeInfo("${p.cleanName}").niceName;
modules.Reflection.getTypeInfo("${p.cleanName}").nicePluralName;
new modules.Reflection.MessageKey("${p.cleanName}Message", "Enum Member Name").niceToString();
modules.Reflection.New("${p.cleanName}");
`;
  modules["Entities"] = `modules.Entities.toLite(entity);
modules.Entities.is(entityOrLite1, entityOrLite2);`;
  modules["AuthClient"] = "modules.AuthClient.currentUser();";
  modules["Operations"] = "";
  modules["WorkflowClient"] = "";
  modules["Constructor"] = `modules.Constructor.construct("${p.cleanName}").then(entity => { // do something here }).done();
modules.Constructor.constructPack("${p.cleanName}").then(pack => // do something here).done();`;
  modules["Services"] = `modules.Services.ajaxGet({ url: '~/api/dynamic/getData/{param1}/{param2}/...' })
  .then(result => /* do something here */)
  .then(() => locals.forceUpdate())
  .done();

modules.Services.ajaxPost({ url: '~/api/dynamic/getData' }, data: null)
  .then(result => /* do something here */)
  .then(() => locals.forceUpdate())
  .done();
`;
  modules["TreeClient"] = "";
  modules["AutoCompleteConfig"] = `new modules.AutoCompleteConfig.LiteAutocompleteConfig((signal, subStr) => [Custom API call here ...], requiresInitialLoad: false, showType: false)`;
  modules["Hooks"] = `const forceUpdate = modules.Hooks.useForceUpdate();
const value = modules.Hooks.useAPI(undefined /*default value*/, [/*Keys*/], signal => Your calling API is here);
`;
  modules["FontAwesomeIcon"] = `modules.React.createElement(modules.FontAwesomeIcon, { icon: "...", color: "..." })`;
  modules["SelectorModal"] = `modules.SelectorModal.default.chooseElement<T>(options: T[], config? /*: SelectorConfig<T>*/)
.then(option => {
  if (!option)
    return undefined;
  /* do something here ... */
}).done();

modules.SelectorModal.default.chooseType(options: ["${p.cleanName}", ....].map(tn => modules.Reflection.getTypeInfo(tn)))
.then(ti => {
  if (!ti)
    return undefined;
  /* do something here ... */
}).done();

//export interface SelectorConfig<T> {
//  buttonName?: (val: T) => string;
//  buttonDisplay?: (val: T) => modules.React.ReactNode;
//  buttonHtmlAttributes?: (val: T) => modules.React.HTMLAttributes<HTMLButtonElement>;
//  title?: modules.React.ReactNode;
//  message?: modules.React.ReactNode;
//  size?: modules.Components.Basic.BsSize; /* "xl" | "lg" | "md" | "sm" | "xs" */
//  dialogClassName?: string;
//  forceShow?: boolean;
//}
`;
  var clientModules: {
    [name: string]: string;
  } = {};
  if (p.clientCode) {
    clientModules["Navigator"] = `modules.Navigator.addSettings(modules.Navigator.EntitySettings("${p.cleanName}", undefined, {...} /*: EntitySettingsOptions<${p.cleanName}>*/)
modules.Navigator.getSettings("${p.cleanName}") /*: EntitySettingsOptions<${p.cleanName}>*/)
modules.Navigator.getOrAddSettings("${p.cleanName}") /*: EntitySettingsOptions<${p.cleanName}>*/)

//export interface EntitySettingsOptions<T extends ModifiableEntity> {
//  isCreable?: EntityWhen;
//  isFindable?: boolean;
//  isViewable?: boolean;
//  isNavigable?: EntityWhen;
//  isReadOnly?: boolean;
//  avoidPopup?: boolean;
//  modalSize?: BsSize;
//  autocomplete?: AutocompleteConfig<any>;
//  autocompleteDelay?: number;
//  getViewPromise?: (entity: T) => ViewPromise<T>;
//  onNavigateRoute?: (typeName: string, id: string | number) => string;
//  onNavigate?: (entityOrPack: Lite<Entity & T> | T | EntityPack<T>, navigateOptions?: NavigateOptions) => Promise<void>;
//  onView?: (entityOrPack: Lite<Entity & T> | T | EntityPack<T>, viewOptions?: ViewOptions) => Promise<T | undefined>;
//  namedViews?: NamedViewSettings<T>[];
//}
`;
    clientModules["Operations"] = `modules.Operations.addSettings(modules.Operations.EntityOperationSettings("${p.cleanName}.Save", {...} /*: EntityOperationOptions<${p.cleanName}>*/)

//export interface EntityOperationOptions<T extends Entity> {
//  contextual?: ContextualOperationOptions<T>;
//  contextualFromMany?: ContextualOperationOptions<T>;

//  text?: () => string;
//  isVisible?: (ctx: EntityOperationContext<T>) => boolean;
//  confirmMessage?: (ctx: EntityOperationContext<T>) => string | undefined | null;
//  onClick?: (ctx: EntityOperationContext<T>) => void;
//  hideOnCanExecute?: boolean;
//  showOnReadOnly?: boolean;
//  group?: EntityOperationGroup | null;
//  order?: number;
//  color?: BsColor;
//  classes?: string;
//  icon?: IconProp;
//  iconAlign?: "start" | "end";
//  iconColor?: string;
//  keyboardShortcut?: KeyboardShortcut | null;
//  alternatives?: (ctx: EntityOperationContext<T>) => AlternativeOperationSetting<T>[];
//}

//export interface ContextualOperationOptions<T extends Entity> {
//  text?: () => string;
//  isVisible?: (coc: ContextualOperationContext<T>) => boolean;
//  hideOnCanExecute?: boolean;
//  showOnReadOnly?: boolean;
//  confirmMessage?: (coc: ContextualOperationContext<T>) => string | undefined | null;
//  onClick?: (coc: ContextualOperationContext<T>) => void;
//  color?: BsColor;
//  icon?: IconProp;
//  iconColor?: string;
//  order?: number;
//}


//export interface AlternativeOperationSetting<T extends Entity> {
//  name: string;
//  text: () => string;
//  color?: BsColor;
//  classes?: string;
//  icon?: IconProp;
//  iconAlign?: "start" | "end";
//  iconColor?: string;
//  isVisible?: boolean;
//  inDropdown?: boolean;
//  confirmMessage?: (eoc: EntityOperationContext<T>) => string | undefined | null;
//  onClick: (eoc: EntityOperationContext<T>) => void;
//  keyboardShortcut?: KeyboardShortcut;
//}

//export interface KeyboardShortcut{
//  ctrlKey?: boolean;
//  altKey?: boolean;
//  shiftKey?: boolean;
//  key?: string;
//  keyCode?: number; //lowercase
//}

//export type BsColor = "primary" | "secondary" | "success" | "info" | "warning" | "danger" | "light" | "dark";

//export interface EntityOperationGroup {
//  key: string;
//  text: () => string;
//  simplifyName?: (complexName: string) => string;
//  cssClass?: string;
//  color?: BsColor;
//  order?: number;
//}
`;
    clientModules["Finder"] = `modules.Finder.addSettings({ queryName: "${p.cleanName}"...} /*: QuerySettings*/)
modules.Finder.getSettings("${p.cleanName}") /*: QuerySettings*/
modules.Finder.getOrAddSettings("${p.cleanName}") /*: QuerySettings*/

//export interface QuerySettings {
//  queryName: PseudoType | QueryKey;
//  pagination?: Pagination;
//  allowSystemTime?: boolean;
//  defaultOrderColumn?: string | QueryTokenString<any>;
//  defaultOrderType?: OrderType;
//  defaultFilters?: FilterOption[];
//  hiddenColumns?: ColumnOption[];
//  formatters?: { [token: string]: CellFormatter };
//  rowAttributes?: (row: ResultRow, columns: string[]) => React.HTMLAttributes<HTMLTableRowElement> | undefined;
//  entityFormatter?: EntityFormatter;
//  inPlaceNavigation?: boolean;
//  showContextMenu?: (fop: FindOptionsParsed) => boolean | "Basic";
//  getViewPromise?: (e: ModifiableEntity | null) => (undefined | string | Navigator.ViewPromise<ModifiableEntity>);
//  onDoubleClick?: (e: React.MouseEvent<any>, row: ResultRow, sc?: SearchControlLoaded) => void;
//  simpleFilterBuilder?: (sfbc: SimpleFilterBuilderContext) => React.ReactElement<any> | undefined;
//  onFind?: (fo: FindOptions, mo?: ModalFindOptions) => Promise<Lite<Entity> | undefined>;
//  onFindMany?: (fo: FindOptions, mo?: ModalFindOptions) => Promise<Lite<Entity>[] | undefined>;
//  onExplore?: (fo: FindOptions, mo?: ModalFindOptions) => Promise<void>;
//  extraButtons?: (searchControl: SearchControlLoaded) => (ButtonBarElement | null | undefined | false)[];
//}

//export class CellFormatter {
//  constructor(
//    public formatter: (cell: any, ctx: CellFormatterContext) => React.ReactChild | undefined,
//    public cellClass?: string) {
//  }
//}

//export interface CellFormatterContext {
//  refresh?: () => void;
//  systemTime?: SystemTime;
//}
`;
  }
  return (
    <UncontrolledDropdown size="xs">
      <DropdownToggle color="info" caret>{DynamicViewMessage.ModulesHelp.niceToString()}</DropdownToggle>
      <DropdownMenu>
        {Dic.getKeys(globalModules)
          .orderBy(a => p.clientCode && !clientModules[a])
          .map((moduleName, i) => <DropdownItem style={{ paddingTop: "0", paddingBottom: "0" }} key={i} onClick={() => handleModulesClick(moduleName)}>
          {p.clientCode && !clientModules[moduleName] ? <span className="text-muted">{moduleName}</span> : moduleName}
        </DropdownItem>)}
      </DropdownMenu>
    </UncontrolledDropdown>
  );

  function handleModulesClick(key: string) {
    var text = [clientModules[key], modules[key]].filter(a => a).join("\r\n\r\n");
    if (text == "")
      return;
    ValueLineModal.show({
      type: { name: "string" },
      initialValue: text,
      valueLineType: "TextArea",
      title: `${DynamicViewMessage.ModulesHelp.niceToString()}.${key}`,
      message: "Copy to clipboard: Ctrl+C, ESC",
      valueHtmlAttributes: { style: { height: "400px" } },
    }).done();
  }
}
