import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { AutoSetSMChannelNumbers } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { AutoSetSMChannelNumbersRequest } from '@lib/smAPI/smapiTypes';
import { Checkbox, CheckboxChangeEvent } from 'primereact/checkbox';
import { InputNumber } from 'primereact/inputnumber';
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
      iconFilled
      ref={smDialogRef}
      label="Auto Set"
      title="AUTOSET CHANNEL NUMBERS"
      onHide={() => ReturnToParent()}
      buttonClassName="icon-primary-filled"
      icon="pi-sort-numeric-up-alt"
      info="General"
    >
      <div className="border-1 surface-border flex grid flex-wrap justify-content-center p-0 m-0">
        <div className="flex flex-column mt-2 col-6">
          {`Auto set channel numbers ${overwriteNumbers ? 'and overwrite existing numbers ?' : '?'}`}
          <span className="scalein animation-duration-500 animation-iteration-2 text-bold text-red-500 font-italic mt-2">This will auto save</span>
        </div>

        <div className=" flex mt-2 col-6 align-items-center justify-content-start p-0 m-0">
          <span>
            <div className="flex col-12 justify-content-center align-items-center p-0 m-0  w-full ">
              <div className="flex col-2 justify-content-center align-items-center p-0 m-0">
                <Checkbox checked={overwriteNumbers} id="overwriteNumbers" onChange={(e: CheckboxChangeEvent) => setOverwriteNumbers(e.checked ?? false)} />
              </div>
              <span className="flex col-10 text-xs">Overwrite Existing</span>
            </div>

            <div className="flex col-12 justify-content-center align-items-center p-0 m-0">
              <div className="flex col-6 justify-content-end align-items-center p-0 m-0">
                <span className="text-xs pl-4">Ch. #</span>
              </div>
              <div className="flex col-6 pl-1 justify-content-start align-items-center p-0 m-0 w-full">
                <InputNumber
                  className="numbereditorbody"
                  id="startNumber"
                  max={999_999}
                  min={0}
                  onChange={(e) => e.value && setStartNumber(e.value)}
                  showButtons
                  size={3}
                  value={startNumber}
                />
              </div>
            </div>
          </span>
        </div>
        <div className="flex col-12 gap-1 mt-4 justify-content-center ">
          <OKButton onClick={async () => await onAutoChannelsSave()} />
        </div>
      </div>
    </SMDialog>
  );
};

AutoSetSMChannelNumbersDialog.displayName = 'Auto Set Channel Numbers';
export default React.memo(AutoSetSMChannelNumbersDialog);
