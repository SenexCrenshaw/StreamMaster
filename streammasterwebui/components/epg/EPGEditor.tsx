import { SetSMChannelEPGId } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelEPGIdRequest } from '@lib/smAPI/smapiTypes';
import { memo, useState } from 'react';
import EPGSelector from './EPGSelector';

interface EPGEditorProperties {
  readonly data: SMChannelDto;
}

const EPGEditor = ({ data }: EPGEditorProperties) => {
  const [isSaving, setIsSaving] = useState<boolean>(false);

  const onUpdateVideoStream = async (epg: string) => {
    if (!data.Id) {
      return;
    }
    setIsSaving(true);
    const request = {} as SetSMChannelEPGIdRequest;
    request.SMChannelId = data.Id;
    request.EPGId = epg;

    await SetSMChannelEPGId(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => setIsSaving(false));
  };

  return (
    <EPGSelector
      isLoading={isSaving}
      onChange={async (e: string) => {
        await onUpdateVideoStream(e);
      }}
      smChannel={data}
    />
  );
};

export default memo(EPGEditor);
