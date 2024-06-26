import OKButton from '@components/buttons/OKButton';
import NumberEditor from '@components/inputs/NumberEditor';
import { SMDialogRef } from '@components/sm/SMDialog';
import SMPopUp from '@components/sm/SMPopUp';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AutoSetSMChannelNumbers } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { AutoSetSMChannelNumbersRequest } from '@lib/smAPI/smapiTypes';
import { Checkbox, CheckboxChangeEvent } from 'primereact/checkbox';
import React, { useRef } from 'react';

interface AutoSetSMChannelNumbersDialogProperties {
  readonly disabled?: boolean;
}

const AutoSetSMChannelNumbersDialog = ({ disabled }: AutoSetSMChannelNumbersDialogProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);

  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const [overwriteNumbers, setOverwriteNumbers] = React.useState<boolean>(true);
  const [startNumber, setStartNumber] = React.useState<number>(1);
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');

  const onAutoChannelsSave = React.useCallback(async () => {
    if (!selectedStreamGroup || !queryFilter) {
      return;
    }

    const request = {} as AutoSetSMChannelNumbersRequest;
    request.Parameters = queryFilter;
    request.streamGroupId = selectedStreamGroup.Id;
    request.startingNumber = startNumber;
    request.overWriteExisting = overwriteNumbers;

    AutoSetSMChannelNumbers(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        smDialogRef.current?.hide();
      });
  }, [overwriteNumbers, queryFilter, selectedStreamGroup, startNumber]);

  const ReturnToParent = React.useCallback(() => {}, []);

  return (
    <SMPopUp
      header={<OKButton onClick={async () => await onAutoChannelsSave()} />}
      iconFilled
      label="AutoSet Channel #s for SG"
      title="AutoSet Channel #s for SG"
      menu
      hasCloseButton={false}
      onCloseClick={() => ReturnToParent()}
      buttonClassName="icon-yellow"
      icon="pi-sort-numeric-up-alt"
      placement="bottom-end"
      contentWidthSize="3"
    >
      <div className="surface-border flex grid flex-wrap justify-content-center p-0 m-0">
        <div className="flex flex-column mt-2 w-5 ">
          {`Set channel numbers ${overwriteNumbers ? 'and overwrite existing numbers ?' : '?'}`}
          <span className="scalein animation-duration-500 animation-iteration-2 text-bold text-red-500 font-italic mt-2">
            This is specific to stream groups
          </span>
        </div>

        <div className="flex mt-2 w-7 align-items-center justify-content-start p-0 m-0 ">
          <div className="w-6 pl-1 ">
            <NumberEditor darkBackGround max={99999} min={0} onChange={(e) => e !== undefined && setStartNumber(e)} showButtons value={startNumber} />
          </div>
          <div className="flex w-6 justify-content-center align-items-center pl-2">
            <div className="flex w-2 justify-content-center align-items-center p-0 m-0">
              <Checkbox checked={overwriteNumbers} id="overwriteNumbers" onChange={(e: CheckboxChangeEvent) => setOverwriteNumbers(e.checked ?? false)} />
            </div>
            <span className="flex w-9 text-xs pl-2">Overwrite Existing</span>
          </div>
        </div>
      </div>
    </SMPopUp>
  );
};

AutoSetSMChannelNumbersDialog.displayName = 'Auto Set Channel Numbers';
export default React.memo(AutoSetSMChannelNumbersDialog);
