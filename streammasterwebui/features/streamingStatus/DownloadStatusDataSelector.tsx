import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import useGetDownloadServiceStatus from '@lib/smAPI/General/useGetDownloadServiceStatus';
import { ImageDownloadServiceStatus } from '@lib/smAPI/smapiTypes';

import React, { useMemo, useState, useEffect } from 'react';

const DownloadStatusDataSelector = () => {
  const downloadStatus = useGetDownloadServiceStatus();

  // State to store a copy of downloadStatus.data
  const [savedData, setSavedData] = useState<ImageDownloadServiceStatus>({} as ImageDownloadServiceStatus);

  // Update savedData whenever downloadStatus.data changes and is not empty
  useEffect(() => {
    if (downloadStatus.data && Object.keys(downloadStatus.data).length > 0) {
      setSavedData(downloadStatus.data);
    }
  }, [downloadStatus.data]);

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

  // Determine the data source using savedData when downloadStatus.data is empty
  const dataSource = useMemo(() => {
    if (downloadStatus.data && Object.keys(downloadStatus.data).length > 0) {
      return [downloadStatus.data];
    } else if (savedData && Object.keys(savedData).length > 0) {
      return [savedData];
    } else {
      return [];
    }
  }, [downloadStatus.data, savedData]);

  return (
    <SMDataTable
      columns={downloadColumns}
      dataSource={downloadStatus.data ? dataSource : dataSource}
      defaultSortField="startTS"
      enablePaginator={false}
      headerName="Image Download Status"
      id="queustatus"
      // isLoading={downloadStatus.isLoading}
      lazy
      selectedItemsKey="queustatus"
    />
  );
};

DownloadStatusDataSelector.displayName = 'Queue Status';

export default React.memo(DownloadStatusDataSelector);
