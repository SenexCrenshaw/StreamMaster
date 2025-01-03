import StringEditor from '@components/inputs/StringEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { AddCustomLogo } from '@lib/smAPI/Logos/LogosCommands';
import { AddCustomLogoRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useRef } from 'react';

export function CustomLogosAddDialog(): React.ReactElement {
  const dialogRef = useRef<SMPopUpRef>(null);
  const [name, setName] = React.useState<string>('');
  const [source, setSource] = React.useState<string>('');
  const ReturnToParent = useCallback((didUpload?: boolean) => {}, []);

  const onSave = useCallback(() => {
    // Save the new logo
    if (name && source) {
      const toSend = { Name: name, Source: source } as AddCustomLogoRequest;

      AddCustomLogo(toSend)
        .then(() => {
          ReturnToParent(true);
        })
        .catch(() => {
          ReturnToParent(false);
        });
    }
  }, [ReturnToParent, name, source]);

  return (
    <SMPopUp
      buttonClassName="icon-green"
      contentWidthSize="3"
      icon="pi-plus"
      info=""
      onOkClick={() => {
        onSave();
      }}
      onCloseClick={() => {
        ReturnToParent();
      }}
      iconFilled
      modal
      modalCentered
      placement="bottom-end"
      title="Add Custom Logo"
      zIndex={11}
      ref={dialogRef}
    >
      <div className="layout-padding-bottom-lg" />
      <div className="sm-w-12">
        <StringEditor
          disableDebounce
          darkBackGround
          labelInline
          label="Name"
          value={name}
          onChange={(e) => {
            e && setName(e);
          }}
        />
        <StringEditor darkBackGround disableDebounce labelInline label="Source" value={source} onChange={(e) => e && setSource(e)} />
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMPopUp>
  );
}
