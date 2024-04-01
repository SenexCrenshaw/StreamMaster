import NumberEditorBodyTemplate from '@components/inputs/NumberEditorBodyTemplate';
import { getTopToolOptions } from '@lib/common/common';
import { isDev } from '@lib/settings';
import { SetSMChannelNumber } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelNumberRequest } from '@lib/smAPI/smapiTypes';
import { CSSProperties, memo, useCallback } from 'react';

interface SMChannelNumberEditorProperties {
  readonly data: SMChannelDto;
  readonly style?: CSSProperties;
}

const SMChannelNumberEditor = ({ data, style }: SMChannelNumberEditorProperties) => {
  const onUpdateVideoStream = useCallback(
    async (channelNumber: number) => {
      if (data.id === 0 || data.channelNumber === channelNumber) {
        return;
      }

      const toSend = {} as SetSMChannelNumberRequest;

      toSend.smChannelId = data.id;
      toSend.channelNumber = channelNumber;

      await SetSMChannelNumber(toSend)
        .then(() => {})
        .catch((error) => {
          console.log(error);
        });
    },
    [data.channelNumber, data.id]
  );

  return (
    <NumberEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateVideoStream(e);
      }}
      showSave={false}
      tooltip={isDev ? `id: ${data.id}` : undefined}
      tooltipOptions={getTopToolOptions}
      value={data.channelNumber}
    />
  );
};

SMChannelNumberEditor.displayName = 'Channel Number Editor';

export default memo(SMChannelNumberEditor);
