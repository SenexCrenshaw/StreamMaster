import OKButton from '@components/buttons/OKButton';
import NumberEditor from '@components/inputs/NumberEditor';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AutoSetSMChannelNumbers } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { AutoSetSMChannelNumbersRequest } from '@lib/smAPI/smapiTypes';
import { Checkbox, CheckboxChangeEvent } from 'primereact/checkbox';

import React, { useRef } from 'react';
interface AutoSetSMChannelNumbersProperties {
  readonly onHide?: (didUpload: boolean) => void;
}

const AutoSetSMChannelNumbersDialog = ({ onHide }: AutoSetSMChannelNumbersProperties) => {
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
        smDialogRef.current?.close();
      });
  }, [overwriteNumbers, queryFilter, selectedStreamGroup, startNumber]);

  const ReturnToParent = React.useCallback(() => {}, []);

  return (
    <SMDialog
      header={<OKButton onClick={async () => await onAutoChannelsSave()} />}
      iconFilled
      ref={smDialogRef}
      label="Set Channel #"
      title="SET CHANNEL NUMBERS"
      onHide={() => ReturnToParent()}
      buttonClassName="icon-yellow-filled"
      icon="pi-sort-numeric-up-alt"
      info="General"
      widthSize={3}
    >
      <div className="surface-border flex grid flex-wrap justify-content-center p-0 m-0 w-12 min-h-4rem h-4rem">
        <div className="flex flex-column mt-2 w-5 ">
          {`Auto set channel numbers ${overwriteNumbers ? 'and overwrite existing numbers ?' : '?'}`}
          {/* <span className="scalein animation-duration-500 animation-iteration-2 text-bold text-red-500 font-italic mt-2">This will auto save</span> */}
        </div>

        <div className="flex mt-2 w-7 align-items-center justify-content-start p-0 m-0 ">
          <div className="w-6 pl-1 ">
            <NumberEditor darkBackGround max={99999} min={0} onChange={(e) => e && setStartNumber(e)} showButtons value={startNumber} />
          </div>
          <div className="flex w-6 justify-content-center align-items-center pl-2">
            <div className="flex w-2 justify-content-center align-items-center p-0 m-0">
              <Checkbox checked={overwriteNumbers} id="overwriteNumbers" onChange={(e: CheckboxChangeEvent) => setOverwriteNumbers(e.checked ?? false)} />
            </div>
            <span className="flex w-9 text-xs pl-2">Overwrite Existing</span>
          </div>
        </div>
      </div>
    </SMDialog>
  );
};

AutoSetSMChannelNumbersDialog.displayName = 'Auto Set Channel Numbers';
export default React.memo(AutoSetSMChannelNumbersDialog);
