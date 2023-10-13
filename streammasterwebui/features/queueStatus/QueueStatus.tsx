'use client';
import StandardHeader from '@components/StandardHeader';
import { formatJSONDateString } from '@lib/common/common';
import { QueueStatisIcon } from '@lib/common/icons';
import { TaskQueueStatusDto, useSettingsGetQueueStatusQuery } from '@lib/iptvApi';
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import React from 'react';

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
        const diff = new Date(rowData.stopTS).getTime() - new Date(rowData.startTS).getTime();
        const seconds = diff / 1000;

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
    <StandardHeader displayName="QUEUE STATUS" icon={<QueueStatisIcon />}>
      <DataTable
        className="w-full text-sm"
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
            maxWidth: '10rem',
            width: '10rem',
          }}
        />

        <Column field="command" header="Command" key="command" />

        <Column
          body={queuedDateTimeTemplate}
          field="queueTS"
          header="Queued Time"
          key="queueTS"
          style={{
            maxWidth: '16rem',
            width: '16rem',
          }}
        />

        <Column
          body={startDateTimeTemplate}
          field="startTS"
          header="Start Time"
          key="startTS"
          style={{
            maxWidth: '16rem',
            width: '16rem',
          }}
        />
        <Column
          body={stopDateTimeTemplate}
          field="stopTS"
          header="Stop Time"
          key="stopTS"
          style={{
            maxWidth: '16rem',
            width: '16rem',
          }}
        />

        <Column
          body={elaspsedTemplate}
          header="Elapsed Seconds"
          style={{
            maxWidth: '10rem',
            width: '10rem',
          }}
        />
      </DataTable>
    </StandardHeader>
  );
};

QueueStatus.displayName = 'Queue Status';

export default React.memo(QueueStatus);
