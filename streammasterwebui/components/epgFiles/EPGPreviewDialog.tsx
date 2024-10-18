import SMPopUp from '@components/sm/SMPopUp';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import useGetEPGFilePreviewById from '@lib/smAPI/EPGFiles/useGetEPGFilePreviewById';
import { EPGFileDto, EPGFilePreviewDto } from '@lib/smAPI/smapiTypes';
import { skipToken } from '@reduxjs/toolkit/query';
import { memo, useMemo } from 'react';

interface EPGPreviewDialogProperties {
  readonly selectedFile: EPGFileDto;
}

const EPGPreviewDialog = ({ selectedFile }: EPGPreviewDialogProperties) => {
  const epgFilesGetEpgFilePreviewByIdQuery = useGetEPGFilePreviewById(selectedFile ? { Id: selectedFile.Id } : skipToken);

  function imageBodyTemplate(data: EPGFilePreviewDto) {
    if (!data?.ChannelLogo || data.ChannelLogo === '') {
      return <div />;
    }

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img loading="lazy" alt={data.ChannelLogo ?? 'Logo'} className="max-h-1rem max-w-full p-0" src={`${encodeURI(data.ChannelLogo ?? '')}`} />
      </div>
    );
  }

  const columns = useMemo(
    (): ColumnMeta[] => [
      { bodyTemplate: imageBodyTemplate, field: 'ChannelLogo', fieldType: 'image', width: 8 },
      { field: 'ChannelNumber', filter: true, header: 'Station Id', sortable: true },
      { field: 'ChannelName', filter: true, sortable: true }
    ],
    []
  );

  return (
    <SMPopUp
      buttonClassName="icon-orange"
      contentWidthSize="5"
      icon="pi-book"
      info=""
      modal
      modalCentered
      title={selectedFile ? 'EPG Preview : ' + selectedFile.Name : 'EPG Preview'}
    >
      <SMDataTable
        columns={columns}
        dataSource={epgFilesGetEpgFilePreviewByIdQuery.data}
        defaultSortField="ChannelName"
        enablePaginator
        id="epgPreviewTable"
        isLoading={epgFilesGetEpgFilePreviewByIdQuery.isLoading}
        noSourceHeader
        lazy
        style={{ height: 'calc(50vh)' }}
      />
    </SMPopUp>
  );
};

export default memo(EPGPreviewDialog);
