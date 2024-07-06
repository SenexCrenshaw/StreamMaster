import SGAddButton from '@components/buttons/SGAddButton';
import SGRemoveButton from '@components/buttons/SGRemoveButton';
import { Logger } from '@lib/common/logger';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AddSMChannelToStreamGroup, RemoveSMChannelFromStreamGroup } from '@lib/smAPI/StreamGroupSMChannelLinks/StreamGroupSMChannelLinksCommands';
import { AddSMChannelToStreamGroupRequest, RemoveSMChannelFromStreamGroupRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { Tooltip } from 'primereact/tooltip';
import { memo, useCallback, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

interface SMChannelSGEditorProperties {
  readonly smChannel: SMChannelDto;
}

const SMChannelSGEditor = ({ smChannel }: SMChannelSGEditorProperties) => {
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  const addSMChannelToStreamGroup = useCallback(async () => {
    Logger.debug('Attempting to add SMChannel to StreamGroup', { selectedStreamGroup, smChannel });
    if (!selectedStreamGroup || !smChannel) {
      Logger.warn('Missing selectedStreamGroup or smChannel', { selectedStreamGroup, smChannel });
      return;
    }

    const request: AddSMChannelToStreamGroupRequest = {
      SMChannelId: smChannel.Id,
      StreamGroupId: selectedStreamGroup.Id
    };

    try {
      await AddSMChannelToStreamGroup(request);
      Logger.info('SMChannel added to StreamGroup successfully', { request });
    } catch (error) {
      Logger.error('Error adding SMChannel to StreamGroup', error);
    }
  }, [smChannel, selectedStreamGroup]);

  const removeSMChannelFromStreamGroup = useCallback(async () => {
    Logger.debug('Attempting to remove SMChannel from StreamGroup', { selectedStreamGroup, smChannel });
    if (!selectedStreamGroup || !smChannel) {
      Logger.warn('Missing selectedStreamGroup or smChannel', { selectedStreamGroup, smChannel });
      return;
    }

    const request: RemoveSMChannelFromStreamGroupRequest = {
      SMChannelId: smChannel.Id,
      StreamGroupId: selectedStreamGroup.Id
    };

    try {
      await RemoveSMChannelFromStreamGroup(request);
      Logger.info('SMChannel removed from StreamGroup successfully', { request });
    } catch (error) {
      Logger.error('Error removing SMChannel from StreamGroup', error);
    }
  }, [smChannel, selectedStreamGroup]);

  const isSMChannelInStreamGroup = useCallback(() => {
    if (!smChannel || !selectedStreamGroup || !smChannel.StreamGroupIds) {
      return false;
    }

    return smChannel.StreamGroupIds.includes(selectedStreamGroup.Id);
  }, [selectedStreamGroup, smChannel]);

  const name = useMemo(() => selectedStreamGroup?.Name ?? '', [selectedStreamGroup?.Name]);

  const tooltipClassName = useMemo(() => `basebutton-${uuidv4()}`, []);

  if (!selectedStreamGroup || selectedStreamGroup.Name === 'ALL') {
    return (
      <>
        <Tooltip target={`.${tooltipClassName}`} />
        <div className="flex justify-content-center align-items-center">
          <span
            className={tooltipClassName}
            data-pr-tooltip="Please Select a SG"
            data-pr-position="left"
            data-pr-showdelay={400}
            data-pr-hidedelay={100}
            data-pr-autohide={true}
          >
            <i className="pi pi-circle p-disabled" />
          </span>
        </div>
      </>
    );
  }

  if (isSMChannelInStreamGroup()) {
    return (
      <div className="flex justify-content-center align-items-center">
        <SGRemoveButton onClick={removeSMChannelFromStreamGroup} tooltip={`Remove From ${name}`} />
      </div>
    );
  }

  if (selectedStreamGroup.Name === 'ALL') {
    return (
      <div className="flex justify-content-center align-items-center">
        <SGRemoveButton buttonDisabled tooltip="ALL SG" />
      </div>
    );
  }

  return (
    <div className="flex justify-content-center align-items-center">
      <SGAddButton onClick={addSMChannelToStreamGroup} tooltip={`Add to ${name}`} />
    </div>
  );
};

export default memo(SMChannelSGEditor);
