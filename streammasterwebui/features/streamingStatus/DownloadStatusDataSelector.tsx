import React, { useCallback, useMemo } from 'react';

import useGetDownloadServiceStatus from '@lib/smAPI/General/useGetDownloadServiceStatus';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { ImageDownloadServiceStatus } from '@lib/smAPI/smapiTypes';

const DownloadStatusDataSelector = () => {
  const { data } = useGetDownloadServiceStatus();

  const headerTemplate = useMemo(() => {
    return <div className="sm-center-stuff">Queued/Processed/Downloaded/AlreadyExists/Errors</div>;
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
        {rowData.TotalNameLogo}/{rowData.TotalNameLogoDownloadAttempts}/{rowData.TotalNameLogoSuccessful}/{rowData.TotalNameLogoAlreadyExists}/
        {rowData.TotalNameLogoErrors}
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
        field: 'TotalNameLogo',
        headerTemplate: headerTemplate,
        width: '10rem'
      },
      // {
      //   align: 'center',
      //   field: 'TotalProgramMetadataDownloadAttempts'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalNameLogoDownloadAttempts',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalProgramMetadataSuccessful',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalNameLogoSuccessful',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalProgramMetadataAlreadyExists',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalNameLogoAlreadyExists',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalProgramMetadataErrors',
      //   width: '10rem'
      // },
      // {
      //   align: 'center',
      //   field: 'TotalNameLogoErrors',
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
    [headerTemplate, logoTemplate, programMetadataTemplate]
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
