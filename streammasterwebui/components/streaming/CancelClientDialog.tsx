import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { CancelClientRequest } from '@lib/smAPI/smapiTypes';
import { CancelClient } from '@lib/smAPI/Streaming/StreamingCommands';

import { memo } from 'react';

const CancelClientDialog = (props: { clientId: string | undefined }) => {
  const cancelClient = () => {
    const request = { UniqueRequestId: props.clientId } as CancelClientRequest;
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
      disabled={props.clientId === undefined}
      buttonClassName="icon-red"
      onOkClick={cancelClient}
      title="Cancel Client?"
      info=""
      tooltip="Cancel Client"
    />
  );
};

export default memo(CancelClientDialog);
