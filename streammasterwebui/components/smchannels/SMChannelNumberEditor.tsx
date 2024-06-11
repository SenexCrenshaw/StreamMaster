import NumberEditor from '@components/inputs/NumberEditor';
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
      if (data.Id === 0 || data.ChannelNumber === channelNumber) {
        return;
      }

      const toSend = {} as SetSMChannelNumberRequest;

      toSend.SMChannelId = data.Id;
      toSend.ChannelNumber = channelNumber;

      await SetSMChannelNumber(toSend)
        .then(() => {})
        .catch((error) => {
          console.log(error);
        });
    },
    [data.ChannelNumber, data.Id]
  );

  return (
    <NumberEditor
      onSave={async (e) => {
        e !== undefined && (await onUpdateVideoStream(e));
      }}
      showSave={false}
      tooltip={isDev ? `id: ${data.Id}` : undefined}
      tooltipOptions={getTopToolOptions}
      value={data.ChannelNumber}
    />
  );
};

SMChannelNumberEditor.displayName = 'Channel Number Editor';

export default memo(SMChannelNumberEditor);
