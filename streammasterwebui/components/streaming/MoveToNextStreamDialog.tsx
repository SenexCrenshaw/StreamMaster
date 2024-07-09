import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { MoveToNextStreamRequest } from '@lib/smAPI/smapiTypes';
import { MoveToNextStream } from '@lib/smAPI/Streaming/StreamingCommands';

import { memo } from 'react';

const MoveToNextStreamDialog = (props: { channelId: number }) => {
  const simulateChannel = () => {
    // if (props.streamUrl === null || props.streamUrl === undefined || props.streamUrl === '') return;

    const request = { SMChannelId: props.channelId } as MoveToNextStreamRequest;
    MoveToNextStream(request)
      .then(() => {})
      .catch(() => {
        Logger.error('Failed to simulate channel');
      })
      .finally(() => {});
  };

  return (
    <SMPopUp
      icon="pi-angle-double-right"
      buttonDisabled={props.channelId === null || props.channelId === undefined || props.channelId === 0}
      buttonClassName="icon-orange"
      onOkClick={simulateChannel}
      title="Move to next stream"
      info=""
      tooltip="Move to next stream"
      showRemember
      rememberKey="simulateChannel"
    />
  );
};

export default memo(MoveToNextStreamDialog);
