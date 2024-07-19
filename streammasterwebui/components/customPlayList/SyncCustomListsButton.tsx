import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { ScanForCustomPlayLists } from '@lib/smAPI/CustomPlayLists/CustomPlayListsCommands';
import React, { useRef } from 'react';

const SyncCustomListsButton = () => {
  const smPopUpRef = useRef<SMPopUpRef>(null);

  const onSave = React.useCallback(() => {
    ScanForCustomPlayLists()
      .then(() => {})
      .catch((e: any) => {
        console.error(e);
      })
      .finally(() => {
        smPopUpRef.current?.hide();
      });
  }, []);

  return (
    <SMPopUp
      buttonClassName="icon-green"
      buttonLabel="Custom Playlist"
      contentWidthSize="2"
      icon="pi-sync"
      iconFilled
      modal
      onOkClick={() => onSave()}
      placement="top"
      ref={smPopUpRef}
      showRemember={false}
      title="Sync Custom Play Lists"
    >
      <div className="sm-center-stuff">Sync Custom Lists</div>
    </SMPopUp>
  );
};

SyncCustomListsButton.displayName = 'SyncCustomListsButton';

export default React.memo(SyncCustomListsButton);
