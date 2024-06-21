import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AddSMChannelsToStreamGroup, AddSMChannelsToStreamGroupByParameters } from '@lib/smAPI/StreamGroupSMChannelLinks/StreamGroupSMChannelLinksCommands';
import { AddSMChannelsToStreamGroupByParametersRequest, AddSMChannelsToStreamGroupRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { Tooltip } from 'primereact/tooltip';
import { memo, useCallback, useMemo } from 'react';
import { v4 as uuidv4 } from 'uuid';

const AddSMChannelsToSGEditor = () => {
  const selectedItemsKey = 'selectSelectedSMChannelDtoItems';
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');

  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  const getTotalCount = useMemo(() => selectedItems?.length ?? 0, [selectedItems]);

  Logger.debug('AddSMChannelsToSGEditor', selectedStreamGroup, getTotalCount);
  const addSMChannelsToStreamGroup = useCallback(async () => {
    Logger.debug('Attempting to add SMChannel to StreamGroup', { getTotalCount, selectAll, selectedStreamGroup });

    if (!selectedStreamGroup || (selectAll !== true && getTotalCount === 0)) {
      return;
    }

    if (selectAll) {
      if (!queryFilter) {
        return;
      }
      const request: AddSMChannelsToStreamGroupByParametersRequest = {
        Parameters: queryFilter,
        StreamGroupId: selectedStreamGroup.Id
      };

      try {
        await AddSMChannelsToStreamGroupByParameters(request);
        Logger.info('SMChannels added to StreamGroup successfully', { request });
      } catch (error) {
        Logger.error('Error adding SMChannel to StreamGroup', error);
      } finally {
      }
      return;
    }
    const ids = selectedItems.map((item) => item.Id);
    const request: AddSMChannelsToStreamGroupRequest = {
      SMChannelIds: ids,
      StreamGroupId: selectedStreamGroup.Id
    };

    try {
      await AddSMChannelsToStreamGroup(request);
      Logger.info('SMChannel added to StreamGroup successfully', { request });
    } catch (error) {
      Logger.error('Error adding SMChannel to StreamGroup', error);
    }
  }, [queryFilter, selectAll, selectedItems, selectedStreamGroup, getTotalCount]);

  // const isSMChannelInStreamGroup = useCallback(() => {
  //   if (!smChannel || !selectedStreamGroup || !smChannel.StreamGroupIds) {
  //     return false;
  //   }

  //   return smChannel.StreamGroupIds.includes(selectedStreamGroup.Id);
  // }, [selectedStreamGroup, smChannel]);

  // const name = useMemo(() => selectedStreamGroup?.Name ?? '', [selectedStreamGroup?.Name]);

  const tooltipClassName = useMemo(() => `basebutton-${uuidv4()}`, []);

  // if (isSMChannelInStreamGroup()) {
  //   return (
  //     <div className="flex justify-content-center align-items-center">
  //       <SGRemoveButton onClick={removeSMChannelFromStreamGroup} tooltip={`Remove From ${name}`} />
  //     </div>
  //   );
  // }
  if (!selectedStreamGroup) {
    return (
      <>
        <Tooltip target={`.${tooltipClassName}`} />
        <div className="flex justify-content-center align-items-center">
          <span
            className={tooltipClassName}
            data-pr-tooltip="Please Select a SsG"
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
  return (
    <SMPopUp
      icon="pi-book"
      label="Add to SG"
      menu
      iconFilled
      title="Add to SG"
      buttonClassName="icon-green"
      buttonDisabled={selectAll ? false : getTotalCount === 0}
      onOkClick={async () => addSMChannelsToStreamGroup()}
      tooltip="Auto Set EPG"
    >
      <div className="text-container px-1">
        Add ({selectAll ? 'All' : getTotalCount}) channels to {selectedStreamGroup.Name}?
      </div>
    </SMPopUp>
  );

  // if (selectedStreamGroup.Name === 'ALL') {
  //   return (
  //     <div className="flex justify-content-center align-items-center">
  //       <SGRemoveButton buttonDisabled tooltip="ALL SG" />
  //     </div>
  //   );
  // }

  // return (
  //   <div className="flex justify-content-center align-items-center">
  //     <SGAddButton onClick={addSMChannelToStreamGroup} tooltip={`Add to ${name}`} />
  //   </div>
  // );
};

export default memo(AddSMChannelsToSGEditor);
