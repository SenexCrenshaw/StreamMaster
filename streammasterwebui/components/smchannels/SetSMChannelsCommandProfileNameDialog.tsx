import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { SetSMChannelsCommandProfileNameFromParametersRequest, SetSMChannelsCommandProfileNameRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { SetSMChannelsCommandProfileName, SetSMChannelsCommandProfileNameFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import React, { useMemo, useRef } from 'react';
import CommandProfileNameSelector from './CommandProfileNameSelector';

interface SetSMChannelsVideoOutputProfileNamDialogProperties {
  readonly selectedItemsKey: string;
  readonly disabled?: boolean;
}

const SetSMChannelsCommandProfileNameDialog = ({ disabled, selectedItemsKey }: SetSMChannelsVideoOutputProfileNamDialogProperties) => {
  const popUpRef = useRef<SMPopUpRef>(null);
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');

  const [CommandProfileName, setCommandProfileName] = React.useState('StreamMaster');

  // const [smChannelDto, setSmChannelDto] = React.useState<string>();

  const ReturnToParent = React.useCallback(() => {
    // popUpRef.current?.hide();
  }, []);

  const onAutoChannelsSave = React.useCallback(async () => {
    if (!queryFilter) {
      return;
    }

    if (selectAll === true) {
      const qrequest = {} as SetSMChannelsCommandProfileNameFromParametersRequest;
      qrequest.Parameters = queryFilter;
      qrequest.CommandProfileName = CommandProfileName;

      SetSMChannelsCommandProfileNameFromParameters(qrequest)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          ReturnToParent();
        });
      return;
    }

    const request = {} as SetSMChannelsCommandProfileNameRequest;
    request.SMChannelIds = selectedItems.map((item) => item.Id);
    request.CommandProfileName = CommandProfileName;

    SetSMChannelsCommandProfileName(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [queryFilter, selectAll, selectedItems, CommandProfileName, ReturnToParent]);

  const tooltipText = useMemo(() => {
    if (selectedStreamGroup === undefined || selectedStreamGroup.Name === 'ALL') {
      return 'Select a Stream Group to set the channel numbers.';
    }
    return 'Set the channel numbers for ' + selectedStreamGroup.Name + '?';
  }, [selectedStreamGroup]);

  const getTotalCount = useMemo(() => selectedItems?.length ?? 0, [selectedItems]);

  return (
    <SMPopUp
      buttonClassName="icon-orange"
      buttonDisabled={getTotalCount === 0 && !selectAll}
      contentWidthSize="3"
      icon="pi-briefcase"
      iconFilled
      info=""
      label={`Set (${selectAll ? 'All' : getTotalCount}) Profiles`}
      menu
      modal
      onOkClick={async () => {
        await onAutoChannelsSave();
      }}
      onCloseClick={() => ReturnToParent()}
      placement="bottom-end"
      ref={popUpRef}
      title="Set Profiles"
      tooltip={tooltipText}
    >
      <div className="sm-center-stuff">
        {/* <div className="text-container sm-center-stuff pr-2">{selectedStreamGroup?.Name} : </div> */}

        <div className="w-8">
          <CommandProfileNameSelector
            darkBackGround
            value={CommandProfileName}
            onChange={(e) => {
              setCommandProfileName(e);
              Logger.debug('SetSMChannelsCommandProfileNameDialog', 'onChange', e);
            }}
          />
        </div>
      </div>
    </SMPopUp>
  );
};

SetSMChannelsCommandProfileNameDialog.displayName = 'Auto Set Channel Numbers';
export default React.memo(SetSMChannelsCommandProfileNameDialog);
