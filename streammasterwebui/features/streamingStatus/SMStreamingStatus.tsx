import { memo } from 'react';
import SMChannelStatus from './SMChannelStatus';

export const StreamingStatus = (): JSX.Element => {
  return (
    <div className="flex flex-row gap-2 w-full px-1">
      <div className="sm-w-6">
        <SMChannelStatus />
      </div>
      {/* <div className="sm-w-6">
        <SMClientStatus />
      </div> */}
    </div>
  );
};

StreamingStatus.displayName = 'Streaming Status';

export default memo(StreamingStatus);
