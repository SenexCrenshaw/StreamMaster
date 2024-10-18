import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
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
  const { isTrue: smTableIsSimple } = useIsTrue('isSimple');

  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');

  const getTotalCount = useMemo(() => selectedItems?.length ?? 0, [selectedItems]);

  const addSMChannelsToStreamGroup = useCallback(async () => {
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

  const tooltipClassName = useMemo(() => `basebutton-${uuidv4()}`, []);

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
      buttonClassName="icon-green"
      buttonDisabled={selectedStreamGroup.Name === 'ALL' ? true : selectAll ? false : getTotalCount === 0}
      icon="pi-book"
      iconFilled
      info=""
      label={`Add (${selectAll ? 'All' : getTotalCount}) To SG : ` + selectedStreamGroup.Name}
      menu
      modal
      onOkClick={async () => addSMChannelsToStreamGroup()}
      placement={smTableIsSimple ? 'bottom-end' : 'bottom'}
      title={'Add to SG : ' + selectedStreamGroup.Name}
      tooltip={'Add to SG : ' + selectedStreamGroup.Name}
    >
      <div className="text-container sm-center-stuff">
        Add ({selectAll ? 'All' : getTotalCount}) channels to {selectedStreamGroup.Name}?
      </div>
    </SMPopUp>
  );
};

export default memo(AddSMChannelsToSGEditor);
