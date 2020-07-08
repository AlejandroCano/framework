import * as React from 'react'
import * as moment from 'moment'
import { ExceptionEntity } from '../Signum.Entities.Basics'
import { ValueLine, EntityLine, TypeContext } from '../Lines'
import { Tab, Tabs } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { classes } from '../Globals';

export default function Exception(p: { ctx: TypeContext<ExceptionEntity> }) {
  const ctx = p.ctx;
  const sc = p.ctx.subCtx({ labelColumns: { sm: 4 } });
  return (
    <div>
      <div className="row">
        <div className="col-sm-6">
          <ValueLine ctx={sc.subCtx(f => f.environment)} />
          <ValueLine ctx={sc.subCtx(f => f.creationDate)} unitText={moment(sc.value.creationDate!).toUserInterface().fromNow()} />
          <EntityLine ctx={sc.subCtx(f => f.user)} />
          <ValueLine ctx={sc.subCtx(f => f.version)} />
          <ValueLine ctx={sc.subCtx(f => f.threadId)} />
          <ValueLine ctx={sc.subCtx(f => f.machineName)} />
          <ValueLine ctx={sc.subCtx(f => f.applicationName)} />
        </div>
        <div className="col-sm-6">
          <ValueLine ctx={sc.subCtx(f => f.actionName)} />
          <ValueLine ctx={sc.subCtx(f => f.controllerName)} />
          <ValueLine ctx={sc.subCtx(f => f.userHostAddress)} />
          <ValueLine ctx={sc.subCtx(f => f.userHostName)} />
          <ValueLine ctx={sc.subCtx(f => f.userAgent)} valueLineType="TextArea" />
        </div>
      </div>
      <ValueLine ctx={ctx.subCtx(f => f.requestUrl)} />
      <ValueLine ctx={ctx.subCtx(f => f.urlReferer)} />
      <h3 style={{ color: "rgb(139, 0, 0)" }}>{ctx.value.exceptionType} <small>(HResult = {ctx.value.hResult})</small></h3>
      <pre><code>{ctx.value.exceptionMessage}</code></pre>
      <Tabs id="exceptionTabs">
        {codeTab(0, a => a.stackTrace)}
        {codeTab(1, a => a.data)}
        {codeTab(2, a => a.queryString)}
        {codeTab(3, a => a.form, true)}
        {codeTab(4, a => a.session)}
      </Tabs>
    </div>
  );

  function codeTab(tabId: number, property: (ex: ExceptionEntity) => any, formatJson?: boolean) {
    const tc = p.ctx.subCtx(property);

    if (tc.propertyRoute == null || !tc.value || tc.value == "")
      return undefined;

    return (
      <Tab title={tc.propertyRoute.member!.niceName} eventKey={tabId}>
        {formatJson ?
          <FormatJson code={tc.value} /> :
          <pre>
            <code>{tc.value}</code>
          </pre>
        }
      </Tab>
    );
  }
}

function FormatJson(p: { code: string | undefined }) {

  const [formatJson, setFormatJson] = React.useState<boolean>(false);

  const formattedJson = React.useMemo(() => {
    if (formatJson == false || p.code == undefined)
      return null;

    try {
      return JSON.stringify(JSON.parse(p.code), undefined, 2);
    } catch{
      return "Invalid Json"
    }
  }, [formatJson, p.code])

  return (
    <div>
      <button className={classes("btn btn-sm btn-light", formatJson && "active")} onClick={() => setFormatJson(!formatJson)}>
        <FontAwesomeIcon icon="code" /> Format JSON 
      </button>
      <pre>
        <code>{formatJson ? formattedJson : p.code}</code>
      </pre>
    </div>
  );
}
