import StandardHeader from '@components/StandardHeader';
import { StreamingStatusIcon } from '@lib/common/icons';
import React, { lazy, Suspense } from 'react';

const DownloadStatusDataSelector = lazy(() => import('./DownloadStatusDataSelector'));

const SMStreamingStatus = lazy(() => import('./SMStreamingStatus'));
const SMTasksDataSelector = lazy(() => import('@components/smtasks/SMTasksDataSelector'));

export const StreamingStatus = (): JSX.Element => {
  return (
    <StandardHeader displayName="Streaming Status" icon={<StreamingStatusIcon />}>
      <Suspense>
        <div className="layout-padding-bottom" />
        <SMStreamingStatus />
        <div className="layout-padding-bottom-lg" />
        <div className="absolute" style={{ bottom: '0%', width: '97%' }}>
          <DownloadStatusDataSelector />
          <div className="layout-padding-bottom-lg" />
          <SMTasksDataSelector width="100%" height="30vh" />
          <div className="layout-padding-bottom" />
        </div>
      </Suspense>
    </StandardHeader>
  );
};

StreamingStatus.displayName = 'Streaming Status';

export default React.memo(StreamingStatus);
