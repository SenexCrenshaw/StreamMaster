import SGAddButton from '@components/buttons/SGAddButton';
import SGRemoveButton from '@components/buttons/SGRemoveButton';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AddSMChannelToStreamGroup, RemoveSMChannelFromStreamGroup } from '@lib/smAPI/StreamGroupSMChannelLinks/StreamGroupSMChannelLinksCommands';
import { AddSMChannelToStreamGroupRequest, RemoveSMChannelFromStreamGroupRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { Tooltip } from 'primereact/tooltip';
import { memo, useCallback, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

interface EPGSGEditorProperties {
  readonly smChannel: SMChannelDto;
  readonly enableEditMode?: boolean;
}

const EPGSGEditor = ({ smChannel, enableEditMode }: EPGSGEditorProperties) => {
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  const addSMChannelToStreamGroup = useCallback(async () => {
    console.log('addSMChannelToStreamGroup', selectedStreamGroup);
    if (!selectedStreamGroup || !smChannel) {
      return;
    }

    const request = {} as AddSMChannelToStreamGroupRequest;
    request.StreamGroupId = selectedStreamGroup.Id;
    request.SMChannelId = smChannel.Id;

    await AddSMChannelToStreamGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      });
  }, [smChannel, selectedStreamGroup]);

  const removeSMChannelFromStreamGroup = useCallback(async () => {
    console.log('removeSMChannelFromStreamGroup', selectedStreamGroup);
    if (!selectedStreamGroup || !smChannel) {
      return;
    }

    const request = {} as RemoveSMChannelFromStreamGroupRequest;
    request.StreamGroupId = selectedStreamGroup.Id;
    request.SMChannelId = smChannel.Id;

    await RemoveSMChannelFromStreamGroup(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      });
  }, [smChannel, selectedStreamGroup]);

  const isSMChannelInStreamGroup = useCallback(() => {
    if (!smChannel || smChannel.StreamGroupIds === undefined || selectedStreamGroup === undefined || smChannel.StreamGroupIds.length === 0) {
      return false;
    }

    return smChannel.StreamGroupIds.some((Id) => Id === selectedStreamGroup.Id);
  }, [selectedStreamGroup, smChannel]);

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
            data-pr-showdelay={400}
            data-pr-hidedelay={100}
            data-pr-autohide={true}
          >
            <i
              className="pi pi-circle p-disabled"
              data-pr-tooltip="Select SG"
              data-pr-position="left"
              data-pr-showdelay={400}
              data-pr-hidedelay={100}
              data-pr-autohide={true}
            />
          </span>
        </div>
      </>
    );
  }

  if (selectedStreamGroup.Name === 'ALL') {
    return (
      <div className="flex justify-content-center align-items-center">
        <SGRemoveButton disabled onClick={(e) => {}} tooltip="ALL SG" />
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
