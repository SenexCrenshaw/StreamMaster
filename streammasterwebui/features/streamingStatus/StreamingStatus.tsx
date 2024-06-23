import StandardHeader from '@components/StandardHeader';
import SMTasksDataSelector from '@components/smtasks/SMTasksDataSelector';
import { StreamingStatusIcon } from '@lib/common/icons';
import React from 'react';
import DownloadStatusDataSelector from './DownloadStatusDataSelector';
import SMStreamingStatus from './SMStreamingStatus';

export const StreamingStatus = (): JSX.Element => {
  return (
    <StandardHeader displayName="Streaming Status" icon={<StreamingStatusIcon />}>
      <div className="flex flex-column justify-content-between min-h-full max-h-full w-full">
        <SMStreamingStatus />
        <div className="layout-padding-bottom-lg" />
        <DownloadStatusDataSelector />
        <div className="layout-padding-bottom-lg" />
        <div className="absolute" style={{ bottom: '0%', width: '97%' }}>
          <SMTasksDataSelector width="100%" />
          <div className="layout-padding-bottom-lg" />
          <div className="layout-padding-bottom-lg" />
        </div>
      </div>
    </StandardHeader>
  );
};

StreamingStatus.displayName = 'Streaming Status';

export default React.memo(StreamingStatus);
