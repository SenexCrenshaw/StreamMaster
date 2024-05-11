import { SetSMChannelEPGId } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelEPGIdRequest } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';
import EPGSelector from './EPGSelector';

interface EPGEditorProperties {
  readonly smChannel: SMChannelDto;
  readonly enableEditMode?: boolean;
}

const EPGEditor = ({ smChannel, enableEditMode }: EPGEditorProperties) => {
  const onUpdateVideoStream = async (epg: string) => {
    if (!smChannel.Id) {
      return;
    }

    const request = {} as SetSMChannelEPGIdRequest;
    request.SMChannelId = smChannel.Id;
    request.EPGId = epg;

    await SetSMChannelEPGId(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      });
  };

  return (
    <EPGSelector
      enableEditMode={enableEditMode}
      onChange={async (e: string) => {
        await onUpdateVideoStream(e);
      }}
      smChannel={smChannel}
    />
  );
};

export default memo(EPGEditor);
