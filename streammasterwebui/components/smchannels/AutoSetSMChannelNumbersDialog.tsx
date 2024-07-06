import OKButton from '@components/buttons/OKButton';
import BooleanEditor from '@components/inputs/BooleanEditor';
import NumberEditor from '@components/inputs/NumberEditor';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AutoSetSMChannelNumbers, AutoSetSMChannelNumbersFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { AutoSetSMChannelNumbersFromParametersRequest, AutoSetSMChannelNumbersRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import React, { useMemo, useRef } from 'react';

interface AutoSetSMChannelNumbersDialogProperties {
  readonly selectedItemsKey: string;
  readonly disabled?: boolean;
}

const AutoSetSMChannelNumbersDialog = ({ disabled, selectedItemsKey }: AutoSetSMChannelNumbersDialogProperties) => {
  const popUpRef = useRef<SMPopUpRef>(null);
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');
  const [overwriteExisting, setOverwriteExisting] = React.useState(false);
  const [startingNumber, setStartingNumber] = React.useState(1);

  const ReturnToParent = React.useCallback(() => {
    popUpRef.current?.hide();
  }, []);

  const onAutoChannelsSave = React.useCallback(async () => {
    if (!selectedStreamGroup || !queryFilter) {
      return;
    }

    if (selectAll === true) {
      const qrequest = {} as AutoSetSMChannelNumbersFromParametersRequest;
      qrequest.Parameters = queryFilter;
      qrequest.StartingNumber = startingNumber;
      qrequest.OverwriteExisting = overwriteExisting;

      AutoSetSMChannelNumbersFromParameters(qrequest)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          ReturnToParent();
        });
      return;
    }

    const request = {} as AutoSetSMChannelNumbersRequest;
    request.SMChannelIds = selectedItems.map((item) => item.Id);
    request.StartingNumber = startingNumber;
    request.OverwriteExisting = overwriteExisting;

    AutoSetSMChannelNumbers(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [selectedStreamGroup, queryFilter, selectAll, selectedItems, startingNumber, overwriteExisting, ReturnToParent]);

  const tooltipText = useMemo(() => {
    if (selectedStreamGroup === undefined || selectedStreamGroup.Name === 'ALL') {
      return 'Select a Stream Group to set the channel numbers.';
    }
    return 'Set the channel numbers for ' + selectedStreamGroup.Name + '?';
  }, [selectedStreamGroup]);

  const getTotalCount = useMemo(() => selectedItems?.length ?? 0, [selectedItems]);

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      buttonDisabled={getTotalCount === 0}
      contentWidthSize="3"
      hasCloseButton={false}
      header={<OKButton onClick={async () => await onAutoChannelsSave()} />}
      icon="pi-sort-numeric-up-alt"
      iconFilled
      info=""
      label="Set #s"
      menu
      modal
      onCloseClick={() => ReturnToParent()}
      placement="bottom-end"
      ref={popUpRef}
      title="Set Channel #s"
      tooltip={tooltipText}
    >
      <div className="sm-center-stuff">
        <div className="text-container sm-center-stuff pr-2">{selectedStreamGroup?.Name} : </div>
        <div className="sm-center-stuff flex">
          <div className="w-4 pr-2">
            <BooleanEditor labelInline label="Overwrite" checked={overwriteExisting} onChange={setOverwriteExisting} />
          </div>
          <div className="w-6 sm-center-stuff">
            <NumberEditor autoFocus darkBackGround disableDebounce labelInline label="Starting #" value={startingNumber} onChange={setStartingNumber} />
          </div>
        </div>
      </div>
    </SMPopUp>
  );
};

AutoSetSMChannelNumbersDialog.displayName = 'Auto Set Channel Numbers';
export default React.memo(AutoSetSMChannelNumbersDialog);
