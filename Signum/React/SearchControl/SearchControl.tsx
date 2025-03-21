import * as React from 'react'
import { Finder } from '../Finder'
import { ResultTable, ResultRow, FindOptions, FindOptionsParsed, FilterOptionParsed, FilterOption, QueryDescription, QueryRequest } from '../FindOptions'
import { Lite, Entity, ModifiableEntity, EntityPack } from '../Signum.Entities'
import { tryGetTypeInfos, getQueryKey, getTypeInfos, QueryTokenString, getQueryNiceName } from '../Reflection'
import { Navigator, ViewPromise } from '../Navigator'
import SearchControlLoaded, { OnDrilldownOptions, SearchControlMobileOptions, SearchControlViewMode, ShowBarExtensionOption } from './SearchControlLoaded'
import { ErrorBoundary } from '../Components';
import { Property } from 'csstype';
import "./Search.css"
import { ButtonBarElement, StyleContext } from '../TypeContext';
import { areEqualDeps, useForceUpdate, usePrevious, useStateWithPromise } from '../Hooks'
import { RefreshMode } from '../Signum.DynamicQuery';
import { HeaderType, Title } from '../Lines/GroupHeader'

export interface SimpleFilterBuilderProps {
  findOptions: FindOptions;
}

export interface SearchControlProps {
  findOptions: FindOptions;
  formatters?: { [token: string]: Finder.CellFormatter };
  rowAttributes?: (row: ResultRow, columns: string[]) => React.HTMLAttributes<HTMLTableRowElement> | undefined;
  entityFormatter?: Finder.EntityFormatter;
  selectionFromatter?: (searchControl: SearchControlLoaded, row: ResultRow, rowIndex: number) => React.ReactElement | undefined;

  extraButtons?: (searchControl: SearchControlLoaded) => (ButtonBarElement | null | undefined | false)[];
  getViewPromise?: (e: any /*Entity*/) => undefined | string | ViewPromise<any /*Entity*/>;
  maxResultsHeight?: Property.MaxHeight<string | number> | any;
  tag?: string | {};
  searchOnLoad?: boolean;
  allowSelection?: boolean | "single";
  showContextMenu?: (fop: FindOptionsParsed) => boolean | "Basic";
  hideButtonBar?: boolean;
  hideFullScreenButton?: boolean;
  defaultIncludeDefaultFilters?: boolean;
  showHeader?: boolean | "PinnedFilters";
  pinnedFilterVisible?: (fop: FilterOptionParsed) => boolean;
  showBarExtension?: boolean;
  showBarExtensionOption?: ShowBarExtensionOption;
  showFilters?: boolean;
  showSimpleFilterBuilder?: boolean;
  showFilterButton?: boolean;
  showSelectedButton?: boolean;
  showSystemTimeButton?: boolean;
  showGroupButton?: boolean;
  showFooter?: boolean;
  allowChangeColumns?: boolean;
  allowChangeOrder?: boolean;
  create?: boolean;
  createButtonClass?: string;
  view?: boolean | "InPlace";
  largeToolbarButtons?: boolean;
  defaultRefreshMode?: RefreshMode;
  avoidChangeUrl?: boolean;
  throwIfNotFindable?: boolean;
  deps?: React.DependencyList;
  extraOptions?: any;
  enableAutoFocus?: boolean;
  simpleFilterBuilder?: (sfbc: Finder.SimpleFilterBuilderContext) => React.ReactElement | undefined;
  onNavigated?: (lite: Lite<Entity>) => void;
  onDoubleClick?: (e: React.MouseEvent<any>, row: ResultRow) => void;
  onSelectionChanged?: (rows: ResultRow[]) => void;
  onFiltersChanged?: (filters: FilterOptionParsed[]) => void;
  onHeighChanged?: () => void;
  onSearch?: (fo: FindOptionsParsed, dataChange: boolean, scl: SearchControlLoaded) => void;
  onResult?: (table: ResultTable, dataChange: boolean, scl: SearchControlLoaded) => void;
  //Return "no_change" to prevent refresh. Navigator.view won't be called by search control, but returning an entity allows to return it immediatly in a SearchModal in find mode.  
  onCreate?: (scl: SearchControlLoaded) => Promise<undefined | void | EntityPack<any> | ModifiableEntity | "no_change">;
  onCreateFinished?: (entity: EntityPack<Entity> | ModifiableEntity | Lite<Entity> | undefined | void, scl: SearchControlLoaded) => void;
  ctx?: StyleContext;
  customRequest?: (req: QueryRequest, fop: FindOptionsParsed) => Promise<ResultTable>;
  onPageSubTitleChanged?: () => void;
  mobileOptions?: (fop: FindOptionsParsed) => SearchControlMobileOptions;
  onDrilldown?: (scl: SearchControlLoaded, row: ResultRow, options?: OnDrilldownOptions) => Promise<boolean | undefined>;
  showTitle?: HeaderType;
}

export interface SearchControlState {
  queryDescription: QueryDescription;
  findOptionsParsed?: FindOptionsParsed;
  deps?: React.DependencyList;
  message?: string;
}

function is_touch_device(): boolean {
  return 'ontouchstart' in window        // works on most browsers 
    || Boolean(navigator.maxTouchPoints);       // works on IE10/11 and Surface
}

export interface SearchControlHandler {
  findOptions: FindOptions;
  state?: SearchControlState;
  doSearch(opts: { dataChanged?: boolean, force?: boolean }): void;
  doSearchPage1(force?: boolean): void;
  searchControlLoaded: SearchControlLoaded | null;
}

export namespace SearchControlOptions {
  export let showSelectedButton = (sc: SearchControlHandler) => is_touch_device();
  export let showSystemTimeButton = (sc: SearchControlHandler) => true;
  export let showGroupButton = (sc: SearchControlHandler) => true;
  export let showFilterButton = (sc: SearchControlHandler) => true;
  export let allowChangeColumns = (sc: SearchControlHandler) => true;
  export let allowOrderColumns = (sc: SearchControlHandler) => true;
  export let showFooter = (sc: SearchControlHandler) => sc.searchControlLoaded?.props.showFooter;
}

const SearchControl = React.forwardRef(function SearchControl(p: SearchControlProps, ref: React.Ref<SearchControlHandler>) {

  const [state, setState] = useStateWithPromise<SearchControlState | undefined>(undefined);
  const searchControlLoaded = React.useRef<SearchControlLoaded>(null);
  const lastDeps = usePrevious(p.deps);
  //const lastFO = usePrevious(p.findOptions);
  //const lastFrame = usePrevious({ currentDate: p.ctx?.frame?.currentDate, previousDateda: p.ctx?.frame?.previousDate });

  const handler: SearchControlHandler = {
    findOptions: p.findOptions,
    get searchControlLoaded() {
      return searchControlLoaded.current;
    },
    state: state,
    doSearch: opts => searchControlLoaded.current && searchControlLoaded.current.doSearch(opts),
    doSearchPage1: force => searchControlLoaded.current && searchControlLoaded.current.doSearchPage1(force),
  };
  React.useImperativeHandle(ref, () => handler, [p.findOptions, state, searchControlLoaded.current]);

  React.useEffect(() => {
    if (state?.findOptionsParsed) {
      const fo = Finder.toFindOptions(state.findOptionsParsed, state.queryDescription, p.defaultIncludeDefaultFilters!);
      if (Finder.findOptionsPath(p.findOptions) == Finder.findOptionsPath(fo)) {
        if (lastDeps != null && p.deps != null && !areEqualDeps(lastDeps, p.deps))
          setState({ ...state, deps: p.deps });
        return;
      }
    }

    setState(undefined).then(() => {
      const fo = p.findOptions;
      if (!Finder.isFindable(fo.queryName, false)) {
        if (p.throwIfNotFindable)
          throw Error(`Query ${getQueryKey(fo.queryName)} not allowed`);

        return;
      }

      Finder.getQueryDescription(fo.queryName).then(async qd => {
        const message = Finder.validateNewEntities(fo);

        if (message)
          setState({ queryDescription: qd, message: message });
        else {
          const fop = await Finder.parseFindOptions(fo, qd, p.defaultIncludeDefaultFilters!);

          if (fop.systemTime == undefined && p.ctx?.frame?.currentDate && p.ctx.frame!.previousDate &&
            Finder.isSystemVersioned(qd.columns["Entity"].type)) {

            fop.systemTime = {
              mode: 'Between',
              joinMode: 'FirstCompatible',
              startDate: p.ctx.frame.previousDate,
              endDate: p.ctx.frame.currentDate
            };

            const cops = await Finder.parseColumnOptions([
              { token: QueryTokenString.entity().systemValidFrom(), hiddenColumn: true },
              { token: QueryTokenString.entity().systemValidTo(), hiddenColumn: true }
            ], fop.groupResults, qd);

            fop.columnOptions = [...cops, ...fop.columnOptions];

            fop.orderOptions = [...fop.orderOptions, { token: cops[0].token!, orderType: "Descending" }];
          }

          setState({ findOptionsParsed: fop, queryDescription: qd, deps: p.deps });
        }
      });
    });
  }, [Finder.findOptionsPath(p.findOptions), p.ctx?.frame?.currentDate, p.ctx?.frame?.previousDate, ...(p.deps ?? [])]);

  if (state?.message) {
    return (
      <div className="alert alert-danger" role="alert">
        <strong>Error in SearchControl ({getQueryKey(p.findOptions.queryName)}): </strong>
        {state.message}
      </div>
    );
  }

  if (!state || !state.findOptionsParsed)
    return null;

  const fop = state.findOptionsParsed;
  if (!Finder.isFindable(fop.queryKey, false))
    return null;

  const qs = Finder.getSettings(fop.queryKey);
  const qd = state!.queryDescription!;

  const tis = getTypeInfos(qd.columns["Entity"].type);

  return (
    <ErrorBoundary>
      {p.showTitle && <Title type={p.showTitle}>{getQueryNiceName(qd.queryKey)}</Title>}
      <SearchControlLoaded ref={searchControlLoaded}
        findOptions={fop}
        queryDescription={qd}
        querySettings={qs}

        formatters={p.formatters}
        rowAttributes={p.rowAttributes}
        entityFormatter={p.entityFormatter}
        extraButtons={p.extraButtons}
        getViewPromise={p.getViewPromise}
        maxResultsHeight={p.maxResultsHeight}
        tag={p.tag}

        defaultIncudeDefaultFilters={p.defaultIncludeDefaultFilters!}
        searchOnLoad={p.searchOnLoad != null ? p.searchOnLoad : true}
        showHeader={p.showHeader != null ? p.showHeader : true}
        pinnedFilterVisible={p.pinnedFilterVisible}
        showFilters={p.showFilters != null ? p.showFilters : false}
        showSimpleFilterBuilder={p.showSimpleFilterBuilder != null ? p.showSimpleFilterBuilder : true}
        showFilterButton={SearchControlOptions.showFilterButton(handler) && (p.showFilterButton ?? true)}
        showSystemTimeButton={SearchControlOptions.showSystemTimeButton(handler) && (p.showSystemTimeButton ?? false) && (qs?.allowSystemTime ?? tis.some(a => a.isSystemVersioned == true))}
        showGroupButton={SearchControlOptions.showGroupButton(handler) && (p.showGroupButton ?? false)}
        showSelectedButton={SearchControlOptions.showSelectedButton(handler) && (p.showSelectedButton ?? true)}
        showFooter={SearchControlOptions.showFooter(handler)}
        allowChangeColumns={SearchControlOptions.allowChangeColumns(handler) && (p.allowChangeColumns ?? true)}
        allowChangeOrder={SearchControlOptions.allowOrderColumns(handler) && (p.allowChangeOrder ?? true)}
        create={p.create != null ? p.create : (qs?.allowCreate ?? true) && tis.some(ti => Navigator.isCreable(ti, {isSearch: true }))}
        createButtonClass={p.createButtonClass}

        view={p.view != null ? p.view : tis.some(ti => Navigator.isViewable(ti, { isSearch: "main" }))}

        allowSelection={p.allowSelection != null ? p.allowSelection : qs && qs.allowSelection != null ? qs!.allowSelection : true}
        showContextMenu={p.showContextMenu ?? qs?.showContextMenu ?? ((fo) => fo.groupResults ? "Basic" : true)}
        hideButtonBar={p.hideButtonBar != null ? p.hideButtonBar : false}
        hideFullScreenButton={p.hideFullScreenButton != null ? p.hideFullScreenButton : false}
        showBarExtension={p.showBarExtension != null ? p.showBarExtension : true}
        showBarExtensionOption={p.showBarExtensionOption}
        largeToolbarButtons={p.largeToolbarButtons != null ? p.largeToolbarButtons : false}
        defaultRefreshMode={p.defaultRefreshMode}
        avoidChangeUrl={p.avoidChangeUrl != null ? p.avoidChangeUrl : false}
        deps={state.deps}
        extraOptions={p.extraOptions}

        enableAutoFocus={p.enableAutoFocus == null ? false : p.enableAutoFocus}
        simpleFilterBuilder={p.simpleFilterBuilder}

        onCreate={p.onCreate}
        onCreateFinished={p.onCreateFinished}
        onNavigated={p.onNavigated}
        onSearch={p.onSearch}
        onDoubleClick={p.onDoubleClick}
        onSelectionChanged={p.onSelectionChanged}
        onFiltersChanged={p.onFiltersChanged}
        onHeighChanged={p.onHeighChanged}
        onResult={p.onResult}

        ctx={p.ctx}
        customRequest={p.customRequest}
        onPageTitleChanged={p.onPageSubTitleChanged}

        selectionFormatter={p.selectionFromatter}

        mobileOptions={p.mobileOptions}
        onDrilldown={p.onDrilldown}
      />
    </ErrorBoundary>
  );
});

(SearchControl ).defaultProps = {
  allowSelection: true,
  maxResultsHeight: "400px",
  defaultIncludeDefaultFilters: false,
};

export default SearchControl;

export interface ISimpleFilterBuilder {
  getFilters(): FilterOption[];
  onDataChanged?(): void;
}


