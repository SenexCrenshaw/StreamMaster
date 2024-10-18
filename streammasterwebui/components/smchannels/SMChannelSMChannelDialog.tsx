import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import React from 'react';

import SMChannelSMChannelDataSelector from './SMChannelSMChannelDataSelector';
import SMChannelDataForSMChannelSelector from './SMChannelDataForSMChannelSelector';

interface SMChannelSMChannelDialogProperties {
  readonly name?: string;
  readonly selectionKey: string;
  readonly smChannel: SMChannelDto;
}

const SMChannelSMChannelDialog = ({ name, selectionKey, smChannel }: SMChannelSMChannelDialogProperties) => {
  return (
    <div className="flex w-12 gap-1">
      <div className="w-6">
        <SMChannelSMChannelDataSelector height="250px" selectionKey={selectionKey} smChannel={smChannel} />
      </div>
      <div className="w-6">
        <SMChannelDataForSMChannelSelector height="250px" selectionKey={selectionKey} name={name} smChannel={smChannel} />
      </div>
    </div>
  );
};

SMChannelSMChannelDialog.displayName = 'SMChannelSMChannelDialog';

export default React.memo(SMChannelSMChannelDialog);
