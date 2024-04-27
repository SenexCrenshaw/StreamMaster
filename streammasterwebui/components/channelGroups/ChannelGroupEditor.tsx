import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';
import ChannelGroupSelector from './ChannelGroupSelector';

interface ChannelGroupEditorProperties {
  readonly data: SMChannelDto;
}

const ChannelGroupEditor = ({ data }: ChannelGroupEditorProperties) => {
  const onUpdateVideoStream = async (group: string) => {
    if (!data.Id) {
      return;
    }

    const request = {} as SetSMChannelGroupRequest;
    request.SMChannelId = data.Id;
    request.Group = group;

    await SetSMChannelGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      });
  };

  return (
    <ChannelGroupSelector
      onChange={async (e: string) => {
        await onUpdateVideoStream(e);
      }}
      value={data.Group}
    />
  );
};

export default memo(ChannelGroupEditor);
