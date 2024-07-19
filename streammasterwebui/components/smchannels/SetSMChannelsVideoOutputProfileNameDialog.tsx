import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { SetSMChannelsVideoOutputProfileNameFromParametersRequest, SetSMChannelsVideoOutputProfileNameRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { SetSMChannelsVideoOutputProfileName, SetSMChannelsVideoOutputProfileNameFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { Logger } from '@lib/common/logger';
import React, { useMemo, useRef } from 'react';
import VideoOutputProfileNameSelector from './VideoOutputProfileNameSelector';

interface SetSMChannelsVideoOutputProfileNamDialogProperties {
  readonly selectedItemsKey: string;
  readonly disabled?: boolean;
}

const SetSMChannelsVideoOutputProfileNameDialog = ({ disabled, selectedItemsKey }: SetSMChannelsVideoOutputProfileNamDialogProperties) => {
  const popUpRef = useRef<SMPopUpRef>(null);
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');

  const [videoOutputProfileName, setVideoOutputProfileName] = React.useState('StreamMaster');

  // const [smChannelDto, setSmChannelDto] = React.useState<string>();

  const ReturnToParent = React.useCallback(() => {
    // popUpRef.current?.hide();
  }, []);

  const onAutoChannelsSave = React.useCallback(async () => {
    if (!queryFilter) {
      return;
    }

    if (selectAll === true) {
      const qrequest = {} as SetSMChannelsVideoOutputProfileNameFromParametersRequest;
      qrequest.Parameters = queryFilter;
      qrequest.VideoOutputProfileName = videoOutputProfileName;

      SetSMChannelsVideoOutputProfileNameFromParameters(qrequest)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          ReturnToParent();
        });
      return;
    }

    const request = {} as SetSMChannelsVideoOutputProfileNameRequest;
    request.SMChannelIds = selectedItems.map((item) => item.Id);
    request.VideoOutputProfileName = videoOutputProfileName;

    SetSMChannelsVideoOutputProfileName(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [queryFilter, selectAll, selectedItems, videoOutputProfileName, ReturnToParent]);

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
          <VideoOutputProfileNameSelector
            darkBackGround
            value={videoOutputProfileName}
            onChange={(e) => {
              setVideoOutputProfileName(e);
              Logger.debug('SetSMChannelsVideoOutputProfileNameDialog', 'onChange', e);
            }}
          />
        </div>
      </div>
    </SMPopUp>
  );
};

SetSMChannelsVideoOutputProfileNameDialog.displayName = 'Auto Set Channel Numbers';
export default React.memo(SetSMChannelsVideoOutputProfileNameDialog);
