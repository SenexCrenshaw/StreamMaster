import SMButton from '@components/sm/SMButton';
import { AutoSetEPG } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { AutoSetEPGRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import React from 'react';

interface AutoSetEPGSMChannelDialogProperties {
  smChannel: SMChannelDto;
}

const AutoSetEPGSMChannelDialog = ({ smChannel }: AutoSetEPGSMChannelDialogProperties) => {
  const onOk = React.useCallback(async () => {
    if (!smChannel) {
      return;
    }

    const request = {} as AutoSetEPGRequest;
    request.Ids = [smChannel.Id];
    await AutoSetEPG(request);
    //   .then(() => {})
    //   .catch((error) => {
    //     console.error(error);
    //   })
    //   .finally(() => {
    //     smDialogRef.current?.close();
    //   });
    // CopySMChannel(request)
    //   .then(() => {})
    //   .catch((error) => {
    //     console.error(error);
    //   })
    //   .finally(() => {
    //     smDialogRef.current?.close();
    //   });
  }, [smChannel]);

  return (
    <SMButton
      icon="pi-book"
      onClick={() => {
        onOk();
      }}
      tooltip="Auto Set EPG"
    />
  );
};

AutoSetEPGSMChannelDialog.displayName = 'COPYSMCHANNELDIALOG';

export default React.memo(AutoSetEPGSMChannelDialog);
