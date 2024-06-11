import SMOverlay from '@components/sm/SMOverlay';
import CreateSMChannelsFromSMStreamsDialog from '@components/smchannels/CreateSMChannelsFromSMStreamsDialog';
import CreateSMStreamDialog from '@components/smstreams/CreateSMStreamDialog';
import StreamMultiVisibleDialog from '@components/smstreams/StreamMultiVisibleDialog';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const SMStreamMenu = () => {
  return (
    <SMOverlay placement="bottom" icon="pi-bars" iconFilled buttonClassName="icon-orange" contentWidthSize="11rem">
      <div className="sm-channel-menu gap-2">
        <CreateSMChannelsFromSMStreamsDialog
          label="Stream to Channels"
          selectedItemsKey="selectSelectedSMStreamDtoItems"
          id="streameditor-SMStreamDataSelector"
        />
        <StreamMultiVisibleDialog label="Set Visibility" selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
        <CreateSMStreamDialog label="Create Stream" />
      </div>
    </SMOverlay>
  );
};

SMStreamMenu.displayName = 'SMStreamMenu';

export default memo(SMStreamMenu);
