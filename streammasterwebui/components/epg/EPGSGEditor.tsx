import SGAddButton from '@components/buttons/SGAddButton';
import SGRemoveButton from '@components/buttons/SGRemoveButton';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AddSMChannelToStreamGroup, RemoveSMChannelFromStreamGroup } from '@lib/smAPI/StreamGroupSMChannelLinks/StreamGroupSMChannelLinksCommands';
import useGetStreamGroupSMChannels from '@lib/smAPI/StreamGroupSMChannelLinks/useGetStreamGroupSMChannels';
import { AddSMChannelToStreamGroupRequest, RemoveSMChannelFromStreamGroupRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { Tooltip } from 'primereact/tooltip';
import { memo, useCallback, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

interface EPGSGEditorProperties {
  readonly data: SMChannelDto;
  readonly enableEditMode?: boolean;
}

const EPGSGEditor = ({ data, enableEditMode }: EPGSGEditorProperties) => {
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const streamGroupSMChannelsQuery = useGetStreamGroupSMChannels({ StreamGroupId: selectedStreamGroup?.Id });

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
    if (!streamGroupSMChannelsQuery.data || !data) {
      return false;
    }

    return streamGroupSMChannelsQuery.data.some((channel) => channel.Id === data.Id);
  }, [streamGroupSMChannelsQuery.data, data]);

  const name = useMemo(() => {
    return selectedStreamGroup?.Name ?? '';
  }, [selectedStreamGroup?.Name]);

  const tooltipClassName = useMemo(() => {
    const ret = `basebutton-${uuidv4()}`;

    return ret;
  }, []);

  if (isSMChannelInStreamGroup()) {
    return (
      <div className="flex justify-content-center align-items-center">
        <SGRemoveButton onClick={(e) => removeSMChannelFromStreamGroup()} tooltip={`Remove From ${name}`} />
      </div>
    );
  }

  if (selectedStreamGroup === undefined) {
    return (
      <>
        <Tooltip target={`.${tooltipClassName}`} />
        <div className="flex justify-content-center align-items-center">
          <span
            className={tooltipClassName}
            data-pr-tooltip="Select SG"
            data-pr-position="left"
            data-pr-showDelay="400"
            data-pr-hideDelay="100"
            data-pr-autoHide="true"
          >
            <i
              className="pi pi-circle p-disabled"
              data-pr-tooltip="Select SG"
              data-pr-position="left"
              data-pr-showDelay="400"
              data-pr-hideDelay="100"
              data-pr-autoHide="true"
            />
          </span>
        </div>
      </>
    );
  }

  return (
    <div className="flex justify-content-center align-items-center">
      <SGAddButton disabled={selectedStreamGroup === undefined} onClick={(e) => addSMChannelToStreamGroup()} tooltip={`Add to ${name}`} />
    </div>
  );
};

export default memo(EPGSGEditor);
