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
        field: 'TotalProgramMetadata'
      },
      {
        align: 'center',
        field: 'TotalNameLogo',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalProgramMetadataDownloadAttempts'
      },
      {
        align: 'center',
        field: 'TotalNameLogoDownloadAttempts',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalProgramMetadataSuccessful',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalNameLogoSuccessful',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalProgramMetadataAlreadyExists',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalNameLogoAlreadyExists',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalProgramMetadataErrors',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'TotalNameLogoErrors',
        width: '10rem'
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
