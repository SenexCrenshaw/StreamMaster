import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { CancelClientRequest } from '@lib/smAPI/smapiTypes';
import { CancelClient } from '@lib/smAPI/Streaming/StreamingCommands';

import { memo } from 'react';

const CancelClientDialog = (props: { clientId: string }) => {
  const cancelClient = () => {
    const request = { ClientId: props.clientId } as CancelClientRequest;
    CancelClient(request)
      .then(() => {})
      .catch(() => {
        Logger.error('Failed to cancel client');
      })
      .finally(() => {});
  };

  return (
    <SMPopUp
      icon="pi-times"
      buttonClassName="icon-red"
      onOkClick={cancelClient}
      title="Cancel Client?"
      info=""
      tooltip="Cancel Client"
      showRemember
      rememberKey="cancelClient"
    />
  );
};

export default memo(CancelClientDialog);
