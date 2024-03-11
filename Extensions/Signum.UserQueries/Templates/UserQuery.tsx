import * as React from 'react'
import { SystemTimeEmbedded, UserQueryEntity, UserQueryMessage } from '../Signum.UserQueries'
import { FormGroup, AutoLine, EntityLine, EntityTable, EntityStrip, CheckboxLine, TextBoxLine, EntityDetail } from '@framework/Lines'
import { Finder } from '@framework/Finder'
import { FilterConditionOption, FindOptions, SubTokensOptions } from '@framework/FindOptions'
import { getQueryNiceName, getTypeInfos } from '@framework/Reflection'
import { TypeContext } from '@framework/TypeContext'
import QueryTokenEmbeddedBuilder from '../../Signum.UserAssets/Templates/QueryTokenEmbeddedBuilder'
import FilterBuilderEmbedded from '../../Signum.UserAssets/Templates/FilterBuilderEmbedded';
import { useAPI, useForceUpdate } from '@framework/Hooks'
import { SearchMessage, getToString } from '@framework/Signum.Entities'
import { QueryColumnEmbedded, QueryOrderEmbedded, QueryTokenEmbedded } from '../../Signum.UserAssets/Signum.UserAssets.Queries'

const CurrentEntityKey = "[CurrentEntity]";

export default function UserQuery(p: { ctx: TypeContext<UserQueryEntity> }) {

  const forceUpdate = useForceUpdate();

  const query = p.ctx.value.query;
  const ctx = p.ctx;
  const ctx4 = ctx.subCtx({ labelColumns: 4 });
  const ctxxs = ctx.subCtx({ formSize: "xs" });

  const canAggregate = ctx.value.groupResults ? SubTokensOptions.CanAggregate : 0;

  const qd = useAPI(() => Finder.getQueryDescription(query.key), [query.key]);
  if (!qd)
    return null;

  var qs = Finder.querySettings[query.key];

  var hasSystemTime = qs?.allowSystemTime ?? getTypeInfos(qd.columns["Entity"].type);

  return (
    <div>
      <EntityLine ctx={ctx.subCtx(e => e.owner)} />
      <AutoLine ctx={ctx.subCtx(e => e.displayName)} />
      <FormGroup ctx={ctx.subCtx(e => e.query)}>
        {() => query && (
          Finder.isFindable(query.key, true) ?
            <a className="form-control-static" href={Finder.findOptionsPath({ queryName: query.key })}>{getQueryNiceName(query.key)}</a> :
            <span>{getQueryNiceName(query.key)}</span>)
        }
      </FormGroup>

      {query &&
        (<div>
          <EntityLine ctx={ctx.subCtx(e => e.entityType)} readOnly={ctx.value.appendFilters} onChange={() => forceUpdate()}
            helpText={
              <div>
                {UserQueryMessage.MakesThe0AvailableAsAQuickLinkOf1.niceToString(UserQueryEntity.niceName(), ctx.value.entityType ? getToString(ctx.value.entityType) : UserQueryMessage.TheSelected0.niceToString(ctx.niceName(a => a.entityType)))}
                {p.ctx.value.entityType && <br />}
                {p.ctx.value.entityType && UserQueryMessage.Use0ToFilterCurrentEntity.niceToString().formatHtml(<code style={{ display: "inline" }}><strong>{CurrentEntityKey}</strong></code>)}
                {p.ctx.value.entityType && <br />}
                {p.ctx.value.entityType && <CheckboxLine ctx={ctx.subCtx(e => e.hideQuickLink)} inlineCheckbox />}
              </div>
            } />



          <div className="row">
            <div className="col-sm-6">
              <AutoLine ctx={ctx4.subCtx(e => e.groupResults)} onChange={handleOnGroupResultsChange} />
              <AutoLine ctx={ctx4.subCtx(e => e.appendFilters)} readOnly={ctx.value.entityType != null} onChange={() => forceUpdate()}
                helpText={UserQueryMessage.MakesThe0AvailableForCustomDrilldownsAndInContextualMenuWhenGrouping0.niceToString(UserQueryEntity.niceName(), query?.key)} />

            </div>
            <div className="col-sm-6">
              <AutoLine ctx={ctx4.subCtx(e => e.refreshMode)} />
              <AutoLine ctx={ctx4.subCtx(e => e.includeDefaultFilters)} />
              <EntityStrip ctx={ctx4.subCtx(e => e.customDrilldowns)}
                findOptions={getCustomDrilldownsFindOptions()}
                avoidDuplicates={true}
                vertical={true}
                iconStart={true} />
            </div>
          </div>

          <div>
            <FilterBuilderEmbedded ctx={ctxxs.subCtx(e => e.filters)}
              avoidFieldSet="h5"
              subTokenOptions={SubTokensOptions.CanAnyAll | SubTokensOptions.CanElement | canAggregate}
              queryKey={ctxxs.value.query!.key}
              showPinnedFilterOptions={true} />
            <EntityTable ctx={ctxxs.subCtx(e => e.columns)} avoidFieldSet="h5" columns={[
              {
                property: a => a.token,
                template: (ctx, row) =>
                  <div>
                    <QueryTokenEmbeddedBuilder
                      ctx={ctx.subCtx(a => a.token, { formGroupStyle: "SrOnly" })}
                      queryKey={p.ctx.value.query!.key}
                      onTokenChanged={() => { ctx.value.summaryToken = null; ctx.value.modified = true; row.forceUpdate(); }}
                      subTokenOptions={SubTokensOptions.CanElement | SubTokensOptions.CanToArray | SubTokensOptions.CanSnippet | (canAggregate ? canAggregate : SubTokensOptions.CanOperation | SubTokensOptions.CanManual)} />

                    <div className="d-flex">
                      <label className="col-form-label col-form-label-xs me-2" style={{ minWidth: "140px" }}>
                        <input type="checkbox" className="form-check-input" disabled={ctx.value.token == null} checked={ctx.value.summaryToken != null} onChange={() => {
                          ctx.value.summaryToken = ctx.value.summaryToken == null ? QueryTokenEmbedded.New(ctx.value.token) : null;
                          ctx.value.modified = true;
                          row.forceUpdate();
                        }} /> {SearchMessage.SummaryHeader.niceToString()}
                      </label>
                      <div className="flex-grow-1">
                        {ctx.value.summaryToken &&
                          <QueryTokenEmbeddedBuilder
                            ctx={ctx.subCtx(a => a.summaryToken, { formGroupStyle: "SrOnly" })}
                            queryKey={p.ctx.value.query!.key}
                            subTokenOptions={SubTokensOptions.CanElement | SubTokensOptions.CanAggregate} />
                        }
                      </div>
                    </div>
                  </div>
              },
              {
                property: a => a.displayName,
                template: (ctx, row) => <TextBoxLine ctx={ctx.subCtx(a => a.displayName)} readOnly={ctx.value.hiddenColumn} valueHtmlAttributes={{ placeholder: ctx.value.token?.token?.niceName }}
                  helpText={
                    <div>
                      <AutoLine ctx={ctx.subCtx(a => a.combineRows)} readOnly={ctx.value.hiddenColumn} />
                      <CheckboxLine ctx={ctx.subCtx(a => a.hiddenColumn)} inlineCheckbox="block" onChange={() => { ctx.value.summaryToken = null; ctx.value.displayName = null; ctx.value.combineRows = null; row.forceUpdate(); }} />
                    </div>
                  }
                />
              },
            ]} />
            <AutoLine ctx={ctxxs.subCtx(e => e.columnsMode)} valueColumns={4} />

            <EntityTable ctx={ctxxs.subCtx(e => e.orders)} avoidFieldSet="h5" columns={[
              {
                property: a => a.token,
                template: ctx => <QueryTokenEmbeddedBuilder
                  ctx={ctx.subCtx(a => a.token, { formGroupStyle: "SrOnly" })}
                  queryKey={p.ctx.value.query!.key}
                  subTokenOptions={SubTokensOptions.CanElement | SubTokensOptions.CanSnippet | canAggregate} />
              },
              { property: a => a.orderType }
            ]} />
          </div>
          <h5 className="mt-2">{UserQueryMessage.Pagination.niceToString()}</h5>
          <div className="row">
            <div className="col-sm-6">
              <AutoLine ctx={ctxxs.subCtx(e => e.paginationMode, { labelColumns: { sm: 4 } })} formGroupStyle="Basic" />
            </div>
            <div className="col-sm-6">
              <AutoLine ctx={ctxxs.subCtx(e => e.elementsPerPage, { labelColumns: { sm: 4 } })} formGroupStyle="Basic" />
            </div>
          </div>

          {(hasSystemTime || ctx.value.systemTime) && <EntityDetail ctx={ctx.subCtx(a => a.systemTime)} avoidFieldSet="h5"
            getComponent={st => <SystemTime ctx={st} />} />}
        </div>)
      }
    </div>
  );

  function handleOnGroupResultsChange() {
    ctx.value.customDrilldowns = [];
    ctx.value.modified = true;
    forceUpdate();
  }

  function getCustomDrilldownsFindOptions() {
    var fos: FilterConditionOption[] = [];

    if (ctx.value.groupResults)
      fos.push(...[
        { token: UserQueryEntity.token(e => e.query.key), value: query.key },
        { token: UserQueryEntity.token(e => e.entity.appendFilters), value: true }
      ]);
    else
      fos.push({ token: UserQueryEntity.token(e => e.entityType?.entity?.cleanName), value: qd!.columns["Entity"].type.name });

    if (!ctx.value.isNew)
      fos.push({ token: UserQueryEntity.token(e => e.entity), operation: "DistinctTo", value: ctx.value });

    const result = {
      queryName: UserQueryEntity,
      filterOptions: fos.map(fo => { fo.frozen = true; return fo; }),
    } as FindOptions;

    return result;
  }
}

function SystemTime(p: { ctx: TypeContext<SystemTimeEmbedded> }) {
  const forceUpdate = useForceUpdate();
  const ctx = p.ctx.subCtx({ formSize: "xs", formGroupStyle: "Basic" });
  return (
    <div className="row">
      <div className="col-sm-3">
        <AutoLine ctx={ctx.subCtx(e => e.mode)} onChange={() => {
          ctx.value.startDate = ctx.value.mode == "All" ? null : ctx.value.startDate;
          ctx.value.endDate = ctx.value.mode == "All" || ctx.value.mode == "AsOf" ? null : ctx.value.endDate;
          ctx.value.joinMode = ctx.value.mode == "AsOf" ? null : (ctx.value.joinMode ?? "FirstCompatible");
          forceUpdate();
        }} />
      </div>
      <div className="col-sm-3">
        {ctx.value.mode == "All" ? null : <AutoLine ctx={ctx.subCtx(e => e.startDate)} label={ctx.value.mode == "AsOf" ? UserQueryMessage.Date.niceToString() : undefined} mandatory />}
      </div>
      <div className="col-sm-3">
        {ctx.value.mode == "All" || ctx.value.mode == "AsOf" ? null : <AutoLine ctx={ctx.subCtx(e => e.endDate)} mandatory />}
      </div>
      <div className="col-sm-3">
        {ctx.value.mode == "AsOf" ? null : <AutoLine ctx={ctx.subCtx(e => e.joinMode)} mandatory />}
      </div>
    </div>
  );
}
