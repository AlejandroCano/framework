import * as React from 'react'
import { ValueLine, EntityLine, EntityCombo, EntityList, EntityDetail, EntityStrip, EntityRepeater, TypeContext, RenderEntity, EntityTable } from '@framework/Lines'
import { SearchValueLine } from '@framework/Search';
import { JavascriptMessage } from '@framework/Signum.Entities';
import { useInDB } from '@framework/Finder';
import { useAPI, useAPIWithReload } from '../../../../Framework/Signum.React/Scripts/Hooks';
import { API, WhatsNewFull } from "../WhatsNewClient";
import "./AllNewsPage.css"
import * as AppContext from "@framework/AppContext"
import { FilePathEmbedded } from '../../../../Framework/Signum.React.Extensions/Files/Signum.Entities.Files';
import { downloadFile } from '../../../../Framework/Signum.React.Extensions/Files/FileDownloader';
import * as Services from '@framework/Services'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { WhatsNewEntity, WhatsNewMessage } from '../Signum.Entities.WhatsNew';

export default function AllNews() {
  const news: WhatsNewFull[] | undefined = useAPI(() => API.getAllNews().then(w => w), []);

  if (news == undefined)
    return <div>{JavascriptMessage.loading.niceToString()}</div>;

  return (
    <div>
      <h2>{WhatsNewMessage.YourNews.niceToString()} {news && <span className="notify-badge" style={{ marginTop: "6px", marginLeft: "3px", fontSize: "12px" }}>{news.length}</span>}
      </h2>
        <div className="mt-3">
            <div style={{ display: "flex", flexFlow: "wrap" }}>
                {news && news.map(wn =>
                  <WhatsNewPreviewPicture key={wn.whatsNew.id} news={wn} />
                )}
            </div>
          </div>
    </div>
  );
}

export function WhatsNewPreviewPicture(p: { news: WhatsNewFull}) {

  const whatsnew = p.news;

  function handleClickNew(news: WhatsNewFull) {
    AppContext.history.push("~/newspage/" + p.news.whatsNew.id);
  }

  if (whatsnew == undefined)
    return <div>{JavascriptMessage.loading.niceToString()}</div>;

  return (
    <div key={whatsnew.whatsNew.id} style={{ position: "relative", cursor: "pointer", margin: "10px", }}>
      <div className={"card news-shadow"} style={{ width: "500px" }} key={whatsnew.whatsNew.id}>
        {whatsnew.previewPicture != undefined && <img src={AppContext.toAbsoluteUrl("~/api/whatsnew/previewPicture/" + whatsnew.whatsNew.id)} style={{ width: "100%", height: "auto" }} /> }
        <div className={"card-body pt-2"}>
          <h5 className={"card-title"}>{whatsnew.title}</h5>
          <small className={"card-text"}>{whatsnew.description.substring(0, 100)}...</small>
          <br />
          <a href="" onClick={() => handleClickNew(whatsnew)}>{WhatsNewMessage.ReadFurther.niceToString()}</a>
          {(whatsnew.attachments > 0) && <Attachments news={whatsnew} />
          }
        </div>
      </div>
      <NewsBadge news={whatsnew} />
    </div>
  );
}

export function NewsBadge(p: { news: WhatsNewFull }) {
  if (!p.news.read)
    return (
      <span className="notify-badge" style={{ right: "0", top: "0" }}>NEW</span>
    );
  else {
    return (<div></div>);
  }
}

export function Attachments(p: { news: WhatsNewFull }) {
  return (
    <div>
      <hr />
      <h5>{WhatsNewMessage.Downloads.niceToString()} ({p.news.attachments.toString()})</h5>
    </div>
  );
}