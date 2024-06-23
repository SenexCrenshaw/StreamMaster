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
      headerName="Image Download Status"
      isLoading={downloadStatus.isLoading}
      columns={downloadColumns}
      dataSource={downloadStatus.data ? [downloadStatus.data] : []}
      enablePaginator={false}
      id="queustatus"
      defaultSortField="startTS"
      selectedItemsKey="queustatus"
    />
  );
};

DownloadStatusDataSelector.displayName = 'Queue Status';

export default React.memo(DownloadStatusDataSelector);
