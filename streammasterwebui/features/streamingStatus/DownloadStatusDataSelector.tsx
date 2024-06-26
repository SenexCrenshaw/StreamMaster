import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import useGetDownloadServiceStatus from '@lib/smAPI/General/useGetDownloadServiceStatus';

import React, { useMemo } from 'react';

const DownloadStatusDataSelector = () => {
  const downloadStatus = useGetDownloadServiceStatus();

  const downloadColumns = useMemo(
    (): ColumnMeta[] => [
      {
        align: 'center',
        field: 'TotalDownloadAttempts'
      },
      {
        align: 'center',
        field: 'TotalInQueue',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalSuccessful',
        width: '16rem'
      },
      {
        align: 'center',
        field: 'TotalAlreadyExists',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalNoArt',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalErrors'
      }
    ],
    []
  );

  return (
    <SMDataTable
      columns={downloadColumns}
      dataSource={downloadStatus.data ? [downloadStatus.data] : []}
      defaultSortField="startTS"
      enablePaginator={false}
      headerName="Image Download Status"
      id="queustatus"
      isLoading={downloadStatus.isLoading}
      selectedItemsKey="queustatus"
    />
  );
};

DownloadStatusDataSelector.displayName = 'Queue Status';

export default React.memo(DownloadStatusDataSelector);
