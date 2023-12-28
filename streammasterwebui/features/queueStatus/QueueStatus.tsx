import StandardHeader from '@components/StandardHeader';
import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { formatJSONDateString } from '@lib/common/common';
import { QueueStatisIcon } from '@lib/common/icons';
import { TaskQueueStatus, useMiscGetDownloadServiceStatusQuery, useSettingsGetQueueStatusQuery } from '@lib/iptvApi';
import React, { useMemo } from 'react';

const QueueStatus = () => {
  const status = useSettingsGetQueueStatusQuery();
  const downloadStatus = useMiscGetDownloadServiceStatusQuery();

  const startDateTimeTemplate = (rowData: TaskQueueStatus) => {
    if (rowData.startTS === '0001-01-01T00:00:00') {
      return <div>Queued</div>;
    }
    return <div>{formatJSONDateString(rowData.startTS ?? '')}</div>;
  };

  const queuedDateTimeTemplate = (rowData: TaskQueueStatus) => {
    return <div>{formatJSONDateString(rowData.queueTS ?? '')}</div>;
  };

  const stopDateTimeTemplate = (rowData: TaskQueueStatus) => {
    if (rowData.startTS && rowData.stopTS && rowData.startTS < rowData.stopTS) {
      return <div>{formatJSONDateString(rowData.stopTS ?? '')}</div>;
    }

    return <div></div>;
  };

  const isRunningTemplate = (rowData: TaskQueueStatus) => {
    if (rowData.isRunning === true) {
      return <span className="pi pi-spin pi-spinner" />;
    }

    if (rowData.stopTS === '0001-01-01T00:00:00') {
      return <span className="p-0">Waiting To Start</span>;
    }

    return <span className="pi pi-check" />;
  };

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: isRunningTemplate,
        field: 'isRunning',
        header: 'Running',
        width: '10rem'
      },
      {
        field: 'command',
        header: 'Command'
      },
      {
        bodyTemplate: queuedDateTimeTemplate,
        field: 'queueTS',
        header: 'Queued Time',
        width: '16rem'
      },

      {
        align: 'center',
        bodyTemplate: startDateTimeTemplate,
        field: 'startTS',
        header: 'Start Time',
        width: '16rem'
      },
      {
        align: 'center',
        bodyTemplate: stopDateTimeTemplate,
        field: 'stopTS',
        header: 'Stop Time',
        width: '16rem'
      },
      {
        align: 'center',
        field: 'elapsedTS',
        header: 'Elapsed',
        width: '10rem'
      }
    ],
    []
  );

  const downloadColumns = useMemo(
    (): ColumnMeta[] => [
      {
        align: 'center',
        field: 'totalDownloadAttempts'
      },
      {
        align: 'center',
        field: 'totalInQueue',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'totalSuccessful',
        width: '16rem'
      },
      {
        align: 'center',
        field: 'totalAlreadyExists',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'totalNoArt',
        width: '10rem'
      },
      {
        align: 'center',
        field: 'totalErrors'
      }
    ],
    []
  );

  return (
    <StandardHeader displayName="QUEUE STATUS" icon={<QueueStatisIcon />}>
      <div className="flex grid flex-col">
        <DataSelector
          isLoading={status.isLoading}
          columns={columns}
          dataSource={status.data}
          enablePaginator={false}
          id="queustatus"
          defaultSortField="startTS"
          selectedItemsKey="queustatus"
          style={{ height: 'calc(50vh)' }}
        />

        <span className="ml-2 mt-4">Image Download Service</span>
        <DataSelector
          className="mt-4"
          isLoading={downloadStatus.isLoading}
          columns={downloadColumns}
          dataSource={downloadStatus.data ? [downloadStatus.data] : []}
          enablePaginator={false}
          id="queustatus"
          defaultSortField="startTS"
          selectedItemsKey="queustatus"
        />
      </div>
    </StandardHeader>
  );
};

QueueStatus.displayName = 'Queue Status';

export default React.memo(QueueStatus);
