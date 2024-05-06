import React from 'react';
import SMChannelSMStreamNewDataSelector from './SMChannelSMStreamNewDataSelector';
import SMStreamDataForSMChannelSelector from './SMStreamDataForSMChannelSelector';

interface SMChannelSMStreamDialogProperties {
  readonly name?: string;
}

const SMChannelSMStreamDialog = ({ name }: SMChannelSMStreamDialogProperties) => {
  const id = 'SMChannelSMStreamDialog';

  return (
    <div className="flex w-12 gap-2 input-border">
      <div className="w-6">
        <SMChannelSMStreamNewDataSelector height="350px" id={id} name={name} />
      </div>
      <div className="w-6">
        <SMStreamDataForSMChannelSelector height="350px" id={id} name={name} />
      </div>
    </div>
  );
};

SMChannelSMStreamDialog.displayName = 'SMChannelSMStreamDialog';

export default React.memo(SMChannelSMStreamDialog);
