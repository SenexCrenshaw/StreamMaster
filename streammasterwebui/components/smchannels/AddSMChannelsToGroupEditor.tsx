import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AddSMChannelsToStreamGroup, AddSMChannelsToStreamGroupByParameters } from '@lib/smAPI/StreamGroupSMChannelLinks/StreamGroupSMChannelLinksCommands';
import {
  AddSMChannelsToStreamGroupByParametersRequest,
  AddSMChannelsToStreamGroupRequest,
  SetSMChannelsGroupFromParametersRequest,
  SetSMChannelsGroupRequest,
  SMChannelDto
} from '@lib/smAPI/smapiTypes';
import { Tooltip } from 'primereact/tooltip';
import { memo, useCallback, useMemo, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';
import SMChannelGroupEditor from './columns/SMChannelGroupEditor';
import { SetSMChannelsGroup, SetSMChannelsGroupFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';

const AddSMChannelsToGroupEditor = () => {
  const selectedItemsKey = 'selectSelectedSMChannelDtoItems';
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');
  const { isTrue: smTableIsSimple } = useIsTrue('isSimple');
  const [group, setGroup] = useState<string>('Dummy');

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
      const request: SetSMChannelsGroupFromParametersRequest = {
        Group: group,
        Parameters: queryFilter
      };

      try {
        await SetSMChannelsGroupFromParameters(request);
      } catch (error) {
        Logger.error('Error changing SMChannels to group', error);
      } finally {
      }
      return;
    }
    const ids = selectedItems.map((item) => item.Id);
    const request: SetSMChannelsGroupRequest = {
      Group: group,
      SMChannelIds: ids
    };

    try {
      await SetSMChannelsGroup(request);
      Logger.info('SMChannel added to StreamGroup successfully', { request });
    } catch (error) {
      Logger.error('Error adding SMChannel to StreamGroup', error);
    }
  }, [selectedStreamGroup, selectAll, getTotalCount, selectedItems, group, queryFilter]);

  const tooltipClassName = useMemo(() => `basebutton-${uuidv4()}`, []);

  // if (!selectedStreamGroup) {
  //   return (
  //     <>
  //       <Tooltip target={`.${tooltipClassName}`} />
  //       <div className="flex justify-content-center align-items-center">
  //         <span
  //           className={tooltipClassName}
  //           data-pr-tooltip="Please Select a SsG"
  //           data-pr-position="left"
  //           data-pr-showdelay={400}
  //           data-pr-hidedelay={100}
  //           data-pr-autohide={true}
  //         >
  //           <i className="pi pi-circle p-disabled" />
  //         </span>
  //       </div>
  //     </>
  //   );
  // }
  return (
    <SMPopUp
      buttonClassName="icon-green"
      buttonDisabled={selectAll === true ? false : getTotalCount === 0}
      icon="pi-book"
      iconFilled
      info=""
      label="Add to Group"
      menu
      modal
      onOkClick={async () => addSMChannelsToStreamGroup()}
      placement={smTableIsSimple ? 'bottom-end' : 'bottom'}
      title="Add to Group"
      tooltip={'Add to Group'}
    >
      <div className="px-2">
        <SMChannelGroupEditor
          darkBackGround
          onChange={(e) => {
            setGroup(e);
          }}
        />
        <div className="text-container sm-center-stuff">
          Set ({selectAll ? 'All' : getTotalCount}) channels to {group}?
        </div>
      </div>
    </SMPopUp>
  );
};

export default memo(AddSMChannelsToGroupEditor);
