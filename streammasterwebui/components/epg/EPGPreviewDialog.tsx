import BookButton from '@components/buttons/BookButton';
import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { EpgFileDto, EpgFilePreviewDto, useEpgFilesGetEpgFilePreviewByIdQuery } from '@lib/iptvApi';
import { skipToken } from '@reduxjs/toolkit/query';
import { Dialog } from 'primereact/dialog';
import { memo, useMemo, useState } from 'react';

interface EPGPreviewDialogProperties {
  readonly selectedFile: EpgFileDto;
}

const EPGPreviewDialog = ({ selectedFile }: EPGPreviewDialogProperties) => {
  const epgFilesGetEpgFilePreviewByIdQuery = useEpgFilesGetEpgFilePreviewByIdQuery(selectedFile ? selectedFile.id : skipToken);
  const [showPreview, setShowPreview] = useState<boolean>(false);

  function imageBodyTemplate(data: EpgFilePreviewDto) {
    if (!data?.channelLogo || data.channelLogo === '') {
      return <div />;
    }

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img alt={data.channelLogo ?? 'Logo'} className="max-h-1rem max-w-full p-0" src={`${encodeURI(data.channelLogo ?? '')}`} />
      </div>
    );
  }

  const columns = useMemo(
    (): ColumnMeta[] => [
      { bodyTemplate: imageBodyTemplate, field: 'channelLogo', fieldType: 'image' },
      { field: 'channelNumber', filter: true, header: 'Station Id', sortable: true },
      { field: 'channelName', filter: true, sortable: true }
    ],
    []
  );

  return (
    <>
      <Dialog
        header={selectedFile ? selectedFile.name : ''}
        visible={showPreview}
        style={{ width: '50vw' }}
        onHide={() => {
          setShowPreview(false);
        }}
      >
        <div className="flex grid flex-col">
          <DataSelector
            columns={columns}
            dataSource={epgFilesGetEpgFilePreviewByIdQuery.data}
            defaultSortField="name"
            disableSelectAll
            emptyMessage="No Line Ups"
            enableState={false}
            headerName="Line Up Preview"
            id="queustatus"
            isLoading={epgFilesGetEpgFilePreviewByIdQuery.isLoading}
            selectedItemsKey="queustatus"
            style={{ height: 'calc(50vh)' }}
          />
        </div>
      </Dialog>
      <BookButton
        disabled={selectedFile === undefined}
        iconFilled={false}
        onClick={() => {
          setShowPreview(true);
        }}
        tooltip="Preview"
      />
    </>
  );
};

export default memo(EPGPreviewDialog);
