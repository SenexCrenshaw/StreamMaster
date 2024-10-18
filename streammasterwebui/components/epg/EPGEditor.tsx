import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
import { SetSMChannelEPGId } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelEPGIdRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback } from 'react';
import EPGSelector from './EPGSelector';

interface EPGEditorProperties {
  readonly smChannelDto: SMChannelDto;
}

const EPGEditor = ({ smChannelDto }: EPGEditorProperties) => {
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'EPGId',
    Id: smChannelDto.Id.toString()
  });

  const onUpdateVideoStream = useCallback(
    async (epg: string) => {
      if (!smChannelDto.Id) {
        return;
      }
      setIsCellLoading(true);
      const request = {} as SetSMChannelEPGIdRequest;
      request.SMChannelId = smChannelDto.Id;
      request.EPGId = epg;

      await SetSMChannelEPGId(request)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => setIsCellLoading(false));
    },
    [setIsCellLoading, smChannelDto.Id]
  );

  return (
    <EPGSelector
      isLoading={isCellLoading}
      onChange={async (e: string) => {
        await onUpdateVideoStream(e);
      }}
      smChannel={smChannelDto}
    />
  );
};

export default memo(EPGEditor);
