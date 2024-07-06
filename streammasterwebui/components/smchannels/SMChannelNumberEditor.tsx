import NumberEditor from '@components/inputs/NumberEditor';
import { getTopToolOptions } from '@lib/common/common';
import { Logger } from '@lib/common/logger';
import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
import { isDev } from '@lib/settings';
import { SetSMChannelNumber } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelNumberRequest } from '@lib/smAPI/smapiTypes';
import { CSSProperties, memo, useCallback } from 'react';

interface SMChannelNumberEditorProperties {
  readonly data: SMChannelDto;
  readonly style?: CSSProperties;
}

const SMChannelNumberEditor = ({ data, style }: SMChannelNumberEditorProperties) => {
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'ChannelNumber',
    Id: data.Id.toString()
  });

  const onUpdateVideoStream = useCallback(
    async (channelNumber: number) => {
      if (data.Id === 0 || data.ChannelNumber === channelNumber) {
        return;
      }
      setIsCellLoading(true);
      const toSend = {} as SetSMChannelNumberRequest;

      toSend.SMChannelId = data.Id;
      toSend.ChannelNumber = channelNumber ?? 0;

      await SetSMChannelNumber(toSend)
        .then(() => {})
        .catch((error) => {
          console.log(error);
        })
        .finally(() => {
          setIsCellLoading(false);
        });
    },
    [data.ChannelNumber, data.Id, setIsCellLoading]
  );

  return (
    <NumberEditor
      isLoading={isCellLoading}
      onSave={async (e) => {
        Logger.debug('SMChannelNumberEditor', e);
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
