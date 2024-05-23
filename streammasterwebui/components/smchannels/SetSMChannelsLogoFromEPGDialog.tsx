import SMButton from '@components/sm/SMButton';
import { SetSMChannelsLogoFromEPG } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SetSMChannelsLogoFromEPGRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';

import React from 'react';

interface SMChannelsLogoFromEPGsProperties {
  smChannel: SMChannelDto;
}

const SetSMChannelsLogoFromEPGDialog = ({ smChannel }: SMChannelsLogoFromEPGsProperties) => {
  const onOk = React.useCallback(async () => {
    if (!smChannel) {
      return;
    }

    const request = {} as SetSMChannelsLogoFromEPGRequest;
    request.Ids = [smChannel.Id];
    await SetSMChannelsLogoFromEPG(request);
  }, [smChannel]);

  return (
    <SMButton
      icon="pi-image"
      className="icon-blue"
      onClick={() => {
        onOk();
      }}
      tooltip="Set Logo From EPG"
    />
  );
};

SetSMChannelsLogoFromEPGDialog.displayName = 'SetSMChannelsLogoFromEPGDialog';
export default React.memo(SetSMChannelsLogoFromEPGDialog);
