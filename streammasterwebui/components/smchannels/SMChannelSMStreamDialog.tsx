import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import React, { useEffect, useState } from 'react';
import SMChannelSMStreamNewDataSelector from './SMChannelSMStreamDataSelector';
import SMStreamDataForSMChannelSelector from './SMStreamDataForSMChannelSelector';

interface SMChannelSMStreamDialogProperties {
  readonly name?: string;
  readonly smChannel?: SMChannelDto;
}

const SMChannelSMStreamDialog = ({ name, smChannel }: SMChannelSMStreamDialogProperties) => {
  const id = 'SMChannelSMStreamDialog';
  const [internalSMChannel, setInternalSMChannel] = useState<SMChannelDto>({} as SMChannelDto);

  useEffect(() => {
    if (smChannel && smChannel.Id !== internalSMChannel?.Id) {
      setInternalSMChannel(smChannel);
    }
  }, [internalSMChannel?.Id, smChannel]);

  return (
    <div className="flex w-12 gap-1">
      <div className="w-6">
        <SMChannelSMStreamNewDataSelector height="250px" id={id} name={name} smChannel={smChannel} />
      </div>
      <div className="w-6">
        <SMStreamDataForSMChannelSelector height="250px" id={id} name={name} smChannel={smChannel} />
      </div>
    </div>
  );
};

SMChannelSMStreamDialog.displayName = 'SMChannelSMStreamDialog';

export default React.memo(SMChannelSMStreamDialog);
