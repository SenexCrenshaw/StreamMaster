import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';

import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { CopySMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CopySMChannelRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { Checkbox, CheckboxChangeEvent } from 'primereact/checkbox';
import { InputNumber } from 'primereact/inputnumber';
import React, { useRef } from 'react';

interface CopySMChannelProperties {
  label: string;
  readonly onHide?: () => void;
  smChannel: SMChannelDto;
}

const CopySMChannelDialog = ({ label, onHide, smChannel }: CopySMChannelProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);
  const [overwriteNumbers, setOverwriteNumbers] = React.useState<boolean>(true);
  const [startNumber, setStartNumber] = React.useState<number>(1);

  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');

  const ReturnToParent = React.useCallback(() => {
    onHide?.();
  }, [onHide]);

  const onSave = React.useCallback(async () => {
    if (!smChannel || !queryFilter) {
      return;
    }

    const request = {} as CopySMChannelRequest;
    request.SMChannelId = smChannel.Id;
    request.NewName = smChannel.Name + '-Copy';

    CopySMChannel(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        smDialogRef.current?.close();
      });
  }, [queryFilter, smChannel]);

  return (
    <SMDialog
      ref={smDialogRef}
      iconFilled={false}
      title="COPY CHANNEL"
      onHide={() => ReturnToParent()}
      buttonClassName="icon-orange"
      icon="pi-clone"
      info="General"
    >
      <div className="w-12">
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
          <div className="flex col-12 gap-2 mt-4 justify-content-center ">
            <OKButton onClick={async () => await onSave()} />
          </div>
        </div>
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMDialog>
  );
};

CopySMChannelDialog.displayName = 'COPYSMCHANNELDIALOG';

export default React.memo(CopySMChannelDialog);
