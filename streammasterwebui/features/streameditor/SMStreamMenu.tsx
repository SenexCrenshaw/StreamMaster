import SMPopUp from '@components/sm/SMPopUp';
import CreateSMChannelsFromSMStreamsDialog from '@components/smchannels/CreateSMChannelsFromSMStreamsDialog';
import CreateSMStreamDialog from '@components/smstreams/CreateSMStreamDialog';
import StreamMultiVisibleDialog from '@components/smstreams/StreamMultiVisibleDialog';
import { memo } from 'react';

export interface SChannelMenuProperties {}

const SMStreamMenu = () => {
  return (
    <SMPopUp placement="bottom" icon="pi-bars" iconFilled buttonClassName="icon-orange" contentWidthSize="11rem">
      <div className="sm-channel-menu gap-2">
        <CreateSMChannelsFromSMStreamsDialog selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
        <CreateSMStreamDialog label="Add Custom" />
        <StreamMultiVisibleDialog label="Hide Selected" selectedItemsKey="selectSelectedSMStreamDtoItems" id="streameditor-SMStreamDataSelector" />
      </div>
    </SMPopUp>
  );
};

SMStreamMenu.displayName = 'SMStreamMenu';

export default memo(SMStreamMenu);
