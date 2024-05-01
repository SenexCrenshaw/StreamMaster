import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';
import ChannelGroupSelector from '../../channelGroups/ChannelGroupSelector';

interface SMChannelGroupEditorProperties {
  readonly data: SMChannelDto;
}

const SMChannelGroupEditor = ({ data }: SMChannelGroupEditorProperties) => {
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

export default memo(SMChannelGroupEditor);
