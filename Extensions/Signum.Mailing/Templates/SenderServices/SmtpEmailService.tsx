import * as React from 'react'
import { AutoLine, EntityRepeater, EntityDetail, PasswordLine } from '@framework/Lines'
import { TypeContext } from '@framework/TypeContext'
import { SmtpNetworkDeliveryEmbedded, ClientCertificationFileEmbedded, SmtpEmailServiceEntity, } from '../../Signum.Mailing'
export default function SmtpEmailService(p: { ctx: TypeContext<SmtpEmailServiceEntity> }) {
  const sc = p.ctx;

  return (
    <div>
      <AutoLine ctx={sc.subCtx(s => s.deliveryFormat)} />
      <AutoLine ctx={sc.subCtx(s => s.deliveryMethod)} />
      <AutoLine ctx={sc.subCtx(s => s.pickupDirectoryLocation)} />
      <EntityDetail ctx={sc.subCtx(s => s.network)} getComponent={net =>
        <div>
          <AutoLine ctx={net.subCtx(s => s.port)} />
          <AutoLine ctx={net.subCtx(s => s.host)} />
          <AutoLine ctx={net.subCtx(s => s.useDefaultCredentials)} />
          <AutoLine ctx={net.subCtx(s => s.username)} />
          <PasswordLine ctx={net.subCtx(s => s.password)} />
		      <PasswordLine ctx={net.subCtx(s => s.newPassword)} />
          <AutoLine ctx={net.subCtx(s => s.enableSSL)} />

          <AutoLine ctx={net.subCtx(s => s.useOAuth)} />
          <AutoLine ctx={net.subCtx(s => s.oAuthClientID)} />
          <AutoLine ctx={net.subCtx(s => s.oAuthTenantID)} />
          <AutoLine ctx={net.subCtx(s => s.oAuthClientSecret)} />


          <PasswordLine ctx={net.subCtx(s => s.oAuthClientAccessToken)} />
		      <PasswordLine ctx={net.subCtx(s => s.oAuthClientAccessTokenNew)} />

          <PasswordLine ctx={net.subCtx(s => s.oAuthClientRefreshToken)} />
		      <PasswordLine ctx={net.subCtx(s => s.oAuthClientRefreshTokenNew)} />



          <EntityRepeater ctx={net.subCtx(s => s.clientCertificationFiles)} getComponent={cert =>
            <div>
              <AutoLine ctx={cert.subCtx(s => s.certFileType)} />
              <AutoLine ctx={cert.subCtx(s => s.fullFilePath)} />
            </div>
          } />
        </div>
      } />
    </div>
  );
}

