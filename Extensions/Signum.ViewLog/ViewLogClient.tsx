import * as React from 'react'
import { RouteObject } from 'react-router'
import * as QuickLinks from '@framework/QuickLinks'
import * as Navigator from '@framework/Navigator'
import { getQueryKey } from '@framework/Reflection'
import { ViewLogEntity } from './Signum.ViewLog'

export function start(options: { routes: RouteObject[], showQuickLink?: (typeName: string) => boolean }) {

  QuickLinks.registerGlobalQuickLink(entityType =>
    new QuickLinks.QuickLinkExplore(entityType, ctx => ({ queryName: ViewLogEntity, filterOptions: [{ token: ViewLogEntity.token(e => e.target), value: ctx.lite }] }),
      {
        key: getQueryKey(ViewLogEntity),
        text: () => ViewLogEntity.nicePluralName(),
        isVisible: Navigator.isFindable(ViewLogEntity) && (options.showQuickLink == null || options.showQuickLink(entityType)),
        icon: "eye",
        iconColor: "#2E86C1",
      }
    ))
}


