import { memo, useRef } from 'react';

import SMButton from '@components/sm/SMButton';

import CreateSMChannelsFromSMStreamsDialog from '@components/smchannels/CreateSMChannelsFromSMStreamsDialog';
import CreateSMStreamDialog from '@components/smstreams/CreateSMStreamDialog';
import StreamMultiVisibleDialog from '@components/smstreams/StreamMultiVisibleDialog';
import { OverlayPanel } from 'primereact/overlaypanel';

export interface SChannelMenuProperties {}

const SMStreamMenu = () => {
  const op = useRef<OverlayPanel>(null);

  return (
    <>
      <OverlayPanel className="sm-overlay" ref={op}>
        <div className="sm-channel-menu gap-1">
          <div className="pt-1"></div>
          <CreateSMChannelsFromSMStreamsDialog
            label="Stream to Channels"
            selectedItemsKey="selectSelectedSMStreamDtoItems"
            id="streameditor-SMStreamDataSelector"
          />
          <StreamMultiVisibleDialog label="Set Visibility" selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
          <CreateSMStreamDialog label="Create Stream" />
          <div className="pt-1"></div>
        </div>
      </OverlayPanel>
      <SMButton
        className="icon-orange"
        iconFilled
        icon="pi pi-bars"
        rounded
        onClick={(event) => {
          op.current?.toggle(event);
        }}
        aria-controls="popup_menu_right"
        aria-haspopup
      />
    </>
  );
};

SMStreamMenu.displayName = 'SMStreamMenu';

export default memo(SMStreamMenu);
