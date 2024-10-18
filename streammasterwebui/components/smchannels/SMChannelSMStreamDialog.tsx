import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import React from 'react';
import SMChannelSMStreamDataSelector from './SMChannelSMStreamDataSelector';
import SMStreamDataForSMChannelSelector from './SMStreamDataForSMChannelSelector';

interface SMChannelSMStreamDialogProperties {
  readonly name?: string;
  readonly selectionKey: string;
  readonly smChannel: SMChannelDto;
}

const SMChannelSMStreamDialog = ({ name, selectionKey, smChannel }: SMChannelSMStreamDialogProperties) => {
  return (
    <div className="flex w-12 gap-1">
      <div className="w-6">
        <SMChannelSMStreamDataSelector height="250px" selectionKey={selectionKey} smChannel={smChannel} />
      </div>
      <div className="w-6">
        <SMStreamDataForSMChannelSelector height="250px" selectionKey={selectionKey} name={name} smChannel={smChannel} />
      </div>
    </div>
  );
};

SMChannelSMStreamDialog.displayName = 'SMChannelSMStreamDialog';

export default React.memo(SMChannelSMStreamDialog);
