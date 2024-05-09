import SGAddButton from '@components/buttons/SGAddButton';
import SGRemoveButton from '@components/buttons/SGRemoveButton';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AddSMChannelToStreamGroup, RemoveSMChannelFromStreamGroup } from '@lib/smAPI/StreamGroupSMChannelLinks/StreamGroupSMChannelLinksCommands';
import useGetStreamGroupSMChannels from '@lib/smAPI/StreamGroupSMChannelLinks/useGetStreamGroupSMChannels';
import { AddSMChannelToStreamGroupRequest, RemoveSMChannelFromStreamGroupRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';

interface EPGSGEditorProperties {
  readonly data: SMChannelDto;
  readonly enableEditMode?: boolean;
}

const EPGSGEditor = ({ data, enableEditMode }: EPGSGEditorProperties) => {
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const channelsQuery = useGetStreamGroupSMChannels({ StreamGroupId: selectedStreamGroup?.Id });

  // console.log('EPGSGEditor', channelsQuery.data);

  const addSMChannelToStreamGroup = useCallback(async () => {
    console.log('addSMChannelToStreamGroup', selectedStreamGroup);
    if (!selectedStreamGroup || !data) {
      return;
    }

    const request = {} as AddSMChannelToStreamGroupRequest;
    request.StreamGroupId = selectedStreamGroup.Id;
    request.SMChannelId = data.Id;

    await AddSMChannelToStreamGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      });
  }, [data, selectedStreamGroup]);

  const removeSMChannelFromStreamGroup = useCallback(async () => {
    console.log('removeSMChannelFromStreamGroup', selectedStreamGroup);
    if (!selectedStreamGroup || !data) {
      return;
    }

    const request = {} as RemoveSMChannelFromStreamGroupRequest;
    request.StreamGroupId = selectedStreamGroup.Id;
    request.SMChannelId = data.Id;

    await RemoveSMChannelFromStreamGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      });
  }, [data, selectedStreamGroup]);

  const isSMChannelInStreamGroup = useCallback(() => {
    if (!channelsQuery.data || !data) {
      return false;
    }

    return channelsQuery.data.some((channel) => channel.Id === data.Id);
  }, [channelsQuery.data, data]);

  const name = useMemo(() => {
    return selectedStreamGroup?.Name ?? '';
  }, [selectedStreamGroup?.Name]);

  if (isSMChannelInStreamGroup()) {
    return (
      <div className="flex justify-content-center align-items-center">
        <SGRemoveButton onClick={(e) => removeSMChannelFromStreamGroup()} tooltip={`Remove From ${name}`} />
      </div>
    );
  }

  return (
    <div className="flex justify-content-center align-items-center">
      <SGAddButton onClick={(e) => addSMChannelToStreamGroup()} tooltip={`Add to ${name}`} />
    </div>
  );
};

export default memo(EPGSGEditor);
