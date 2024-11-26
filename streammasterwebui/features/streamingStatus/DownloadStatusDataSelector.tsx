import React, { useCallback, useMemo } from 'react';

import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import useGetDownloadServiceStatus from '@lib/smAPI/General/useGetDownloadServiceStatus';
import { ImageDownloadServiceStatus } from '@lib/smAPI/smapiTypes';

const DownloadStatusDataSelector = () => {
  const { data } = useGetDownloadServiceStatus();

  const headerTemplate = useMemo(() => {
    return <div className="sm-center-stuff">Queued/Processed/Downloaded/Exists/Errors</div>;
  }, []);

  const programmeHeaderTemplate = useMemo(() => {
    return <div className="sm-center-stuff">Program Queued/Program Processed/Art Downloaded/Art Exists/Errors</div>;
  }, []);

  const programMetadataTemplate = useCallback((rowData: ImageDownloadServiceStatus) => {
    return (
      <div className="numeric-field">
        {rowData.TotalProgramMetadata}/{rowData.TotalProgramMetadataDownloadAttempts}/{rowData.TotalProgramMetadataDownloaded}/
        {rowData.TotalProgramMetadataAlreadyExists}/{rowData.TotalProgramMetadataErrors}
      </div>
    );
  }, []);

  const logoTemplate = useCallback((rowData: ImageDownloadServiceStatus) => {
    return (
      <div className="numeric-field">
        {rowData.TotallogoInfo}/{rowData.TotallogoInfoDownloadAttempts}/{rowData.TotallogoInfoSuccessful}/{rowData.TotallogoInfoAlreadyExists}/
        {rowData.TotallogoInfoErrors}
      </div>
    );
  }, []);

  const downloadColumns = useMemo(
    (): ColumnMeta[] => [
      {
        align: 'center',
        bodyTemplate: programMetadataTemplate,
        field: 'TotalProgramMetadata',
        headerTemplate: programmeHeaderTemplate,
        width: '10rem'
      },
      {
        align: 'center',
        bodyTemplate: logoTemplate,
        field: 'TotallogoInfo',
        headerTemplate: headerTemplate,
        width: '10rem'
      },
      // {
      //   align: 'center',
      //   field: 'TotalProgramMetadataDownloadAttempts'
      // },
      // {
      //   align: 'center',
      //   field: 'TotallogoInfoDownloadAttempts',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalProgramMetadataSuccessful',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotallogoInfoSuccessful',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalProgramMetadataAlreadyExists',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotallogoInfoAlreadyExists',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalProgramMetadataErrors',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotallogoInfoErrors',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalAlreadyExists',
      //   width: '10rem'
      // },
      {
        align: 'center',
        field: 'TotalProgramMetadataNoArt',
        width: '10rem'
      }
    ],
    [headerTemplate, logoTemplate, programMetadataTemplate, programmeHeaderTemplate]
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
