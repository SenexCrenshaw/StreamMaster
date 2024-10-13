import React, { useMemo } from 'react';

import useGetDownloadServiceStatus from '@lib/smAPI/General/useGetDownloadServiceStatus';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

const DownloadStatusDataSelector = () => {
  const { data } = useGetDownloadServiceStatus();

  const downloadColumns = useMemo(
    (): ColumnMeta[] => [
      {
        align: 'center',
        field: 'TotalProgramMetadata'
      },
      {
        align: 'center',
        field: 'TotalNameLogo',
        width: '4rem'
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
      arrayKey="TotalProgramMetadata"
      columns={downloadColumns}
      dataSource={data ? [data] : undefined}
      defaultSortField="TotalProgramMetadata"
      enablePaginator={false}
      headerName="Image Download Status"
      id="queustatus"
      selectedItemsKey="queustatus"
    />
  );
};

DownloadStatusDataSelector.displayName = 'Queue Status';

export default React.memo(DownloadStatusDataSelector);
