
import React from 'react';

import { type TaskQueueStatusDto } from '../../store/iptvApi';
import { useSettingsGetQueueStatusQuery } from '../../store/iptvApi';
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import { formatJSONDateString } from '../../common/common';
import { QueueStatisIcon } from '../../common/icons';

const QueueStatus = () => {

  const status = useSettingsGetQueueStatusQuery();

  const startDateTimeTemplate = (rowData: TaskQueueStatusDto) => {

    if (rowData.startTS === '0001-01-01T00:00:00') {
      return 'Queued';
    }

    return formatJSONDateString(rowData.startTS ?? '');
  };

  const queuedDateTimeTemplate = (rowData: TaskQueueStatusDto) => {


    return formatJSONDateString(rowData.queueTS ?? '');

  };

  const stopDateTimeTemplate = (rowData: TaskQueueStatusDto) => {
    if (rowData.startTS && rowData.stopTS) {
      if (rowData.startTS < rowData.stopTS) {
        return formatJSONDateString(rowData.stopTS ?? '');
      }
    }

    return '';
  };

  const elaspsedTemplate = (rowData: TaskQueueStatusDto) => {
    if (rowData.startTS && rowData.stopTS) {
      if (rowData.startTS <= rowData.stopTS) {
        const diff = (new Date(rowData.stopTS).getTime() - new Date(rowData.startTS).getTime());
        const seconds = (diff / 1000);

        return seconds.toString();
      }
    }

    return '';
  };


  const isRunningTemplate = (rowData: TaskQueueStatusDto) => {
    if (rowData.isRunning === true) {
      return <span className="pi pi-spin pi-spinner" />;
    }

    if (rowData.stopTS === '0001-01-01T00:00:00') {
      return <span className="p-0">Waiting To Start</span>;
    }

    return <span className="pi pi-check" />;
  };


  return (
    <div className="playListEditor">
      <div className="grid grid-nogutter flex justify-content-between align-items-center">
        <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
          <QueueStatisIcon className='p-0 mr-1' />
          {QueueStatus.displayName?.toUpperCase()}
        </div >
        <div className="flex col-12 mt-1 m-0 p-0 border-1 border-rounded surface-border" >
          <DataTable
            className='w-full text-sm'
            emptyMessage="No Status Found"
            key="id"
            loading={status.isLoading}
            showGridlines
            size="small"
            sortField="startTS"
            sortMode="single"
            sortOrder={1}
            stripedRows
            value={status.data}

          >

            <Column
              body={isRunningTemplate}
              field="isRunning"
              header="Running"
              key="isRunning"
              style={{
                maxWidth: '20rem',
                width: '20rem'
              }}
            />

            <Column
              field="command"
              header="Command"
              key="command"

            />

            <Column
              body={queuedDateTimeTemplate}
              field="queueTS"
              header="Queued Time"
              key="queueTS"
              style={{
                maxWidth: '14rem',
                width: '14rem'
              }}

            />

            <Column
              body={startDateTimeTemplate}
              field="startTS"
              header="Start Time"
              key="startTS"
              style={{
                maxWidth: '14rem',
                width: '14rem'
              }}

            />
            <Column
              body={stopDateTimeTemplate}
              field="stopTS"
              header="Stop Time"
              key="stopTS"
              style={{
                maxWidth: '14rem',
                width: '14rem'
              }}
            />

            <Column
              body={elaspsedTemplate}
              header="Elapsed Seconds"
              style={{
                maxWidth: '8rem',
                width: '8rem'
              }}
            />
          </DataTable>
        </div>
      </div>
    </div >
  );
};

QueueStatus.displayName = 'Queue Status';
QueueStatus.defaultProps = {};

export default React.memo(QueueStatus);
