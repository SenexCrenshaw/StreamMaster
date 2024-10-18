import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { CancelChannelRequest } from '@lib/smAPI/smapiTypes';
import { CancelChannel } from '@lib/smAPI/Streaming/StreamingCommands';

import { memo } from 'react';

const CancelChannelDialog = (props: { channelId: number }) => {
  const cancelChannel = () => {
    // if (props.streamUrl === null || props.streamUrl === undefined || props.streamUrl === '') return;

    const request = { SMChannelId: props.channelId } as CancelChannelRequest;
    CancelChannel(request)
      .then(() => {})
      .catch(() => {
        Logger.error('Failed to cancel channel');
      })
      .finally(() => {});
  };

  return (
    <SMPopUp
      icon="pi-times"
      buttonDisabled={props.channelId === null || props.channelId === undefined || props.channelId === 0}
      buttonClassName="icon-red"
      onOkClick={cancelChannel}
      title="Cancel Channel?"
      info=""
      tooltip="Cancel Channel"
      // showRemember
      // rememberKey="cancelChannel"
    />
  );
};

export default memo(CancelChannelDialog);
