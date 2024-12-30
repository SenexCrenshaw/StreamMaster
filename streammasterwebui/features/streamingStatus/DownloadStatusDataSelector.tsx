import React from 'react';

import useGetDownloadServiceStatus from '@lib/smAPI/Statistics/useGetDownloadServiceStatus';
import DownloadStatusView from './DownloadStatusView';

const DownloadStatusDataSelector = () => {
  const { data } = useGetDownloadServiceStatus();

  if (!data) {
    return null;
  }

  return (
    <div className="flex flex-row sm-border sm-start-stuff">
      <div className="sm-w-3">
        <DownloadStatusView stats={data?.Logos} title="Logos" />
      </div>
      <div className="sm-w-3">
        <DownloadStatusView stats={data?.ProgramLogos} title="Program Logos" />
      </div>
    </div>
  );
};

DownloadStatusDataSelector.displayName = 'Queue Status';

export default React.memo(DownloadStatusDataSelector);
