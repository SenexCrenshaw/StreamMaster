import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { CancelChannelRequest } from '@lib/smAPI/smapiTypes';
import { CancelChannel } from '@lib/smAPI/Streaming/StreamingCommands';

import { memo } from 'react';

const CancelChannelDialog = (props: { channelId: number | string }) => {
  const cancelChannel = () => {
    // if (props.streamUrl === null || props.streamUrl === undefined || props.streamUrl === '') return;
    const channelId = typeof props.channelId === 'string' ? parseInt(props.channelId, 10) : props.channelId;

    if (isNaN(channelId)) {
      Logger.error('Invalid channelId: Unable to convert to a number');
      return;
    }

    const request = { SMChannelId: channelId } as CancelChannelRequest;
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
