
import { ExportComponent, formatJSONDateString } from '@/lib/common/common';
import { LogEntry, LogEntryDto, LogsGetLogApiArg, useLogsGetLogQuery } from '@/lib/iptvApi';

import { FilterMatchMode } from 'primereact/api';
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';

const LogViewer = () => {
  const [lastLogId, setLastLogId] = useState<number>(0);
  const [dataSource, setDataSource] = useState<LogEntryDto[]>([] as LogEntryDto[]);
  const tableRef = useRef<DataTable<LogEntryDto[]>>(null);

  const { isLoading, data, refetch } = useLogsGetLogQuery({ lastId: lastLogId, maxLines: 5000 } as LogsGetLogApiArg);


  const filters = ({
    logLevelName: { matchMode: FilterMatchMode.CONTAINS, value: null },
    message: { matchMode: FilterMatchMode.CONTAINS, value: null },
    timeStamp: { matchMode: FilterMatchMode.CONTAINS, value: null },
  });


  useEffect(() => {

    if (data && data.length > 0) {
      // Filter out duplicates based on ID
      const uniqueData = data.filter(item => !dataSource.some(existingItem => existingItem.id === item.id));

      if (uniqueData.length > 0) {
        // Add only the new unique items to the dataSource
        setDataSource(prevDataSource => [
          ...prevDataSource,
          ...uniqueData,
        ]);

        // Update the lastLogId to the ID of the last item in the new data
        setLastLogId(uniqueData[uniqueData.length - 1].id);

      }
    }

  }, [data, dataSource]);

  const getLogData = useCallback(() => {

    refetch();

  }, [refetch]);

  useEffect(() => {
    if (dataSource.length !== 0) {
      // tableRef.current?.getVirtualScroller()?.scrollToIndex(dataSource.length * 34);
      tableRef.current?.getVirtualScroller()?.scrollTo({ behavior: 'auto', left: 0, top: 400 });
    }
  }, [dataSource]);

  useEffect(() => {
    const intervalId = setInterval(() => {
      getLogData();
    }, 1000);

    return () => {
      clearInterval(intervalId);
    };
  }, [getLogData]);

  const timeStampTemplate = useCallback((rowData: LogEntry) => {
    return (<div>{formatJSONDateString(rowData.timeStamp ?? '')}</div>);
  }, []);

  const levelTemplate = useCallback((rowData: LogEntry) => {
    switch (rowData.logLevel) {
      case 0:
        return (<div className='text-gray-600'>Trace</div>);
      case 1:
        return (<div className='text-blue-600'>Debug</div>);
      case 2:
        return (<div className='text-green-600'>Information</div>);
      case 3:
        return (<div className='text-yellow-600'>Warning</div>);
      case 4:
        return (<div className='text-red-600'>Error</div>);
      case 5:
        return (<div className='text-purple-600'>Critical</div>);
      case 6:
        return (<div className='text-gray-400'>None</div>);
      default:
        return (<div className='text-gray-500'>Unknown</div>);
    }

  }, []);

  const messageTemplate = useCallback((rowData: LogEntry) => {
    return (<div>{rowData.message}</div>);
  }, []);

  const exportCSV = () => {
    tableRef.current?.exportCSV({ selectionOnly: false });
  };

  const renderHeader = useMemo(() => {
    return (
      <div className="flex grid flex-row w-full align-items-center justify-content-end col-12 h-full p-0 debug">
        <ExportComponent exportCSV={exportCSV} />
      </div>
    )
  }, []);

  return (
    <div className='dataselector flex w-full min-w-full flex-column col-12' >
      <DataTable
        exportFilename={`StreamMaster_Logs_${new Date().toISOString()}`}
        filterDisplay='row'
        filters={filters}
        header={renderHeader}
        id='LogViewer'
        ref={tableRef}
        scrollHeight='calc(100vh - 40px)'
        style={{
          height: 'calc(100vh - 40px)',
        }}
        value={dataSource}
        virtualScrollerOptions={{ itemSize: 28 }}
      >
        <Column
          body={levelTemplate}
          field='logLevelName'
          filter
          header='Level'
          sortable
          style={{
            flexGrow: 0,
            flexShrink: 1,
            maxWidth: '8rem',
            overflow: 'hidden',
            paddingLeft: '0.5rem !important',
            paddingRight: '0.5rem !important',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
            width: '10rem',
          }}
        />
        <Column
          body={timeStampTemplate}
          field='timeStamp'
          filter
          header='Time'
          sortable
          style={{
            flexGrow: 0,
            flexShrink: 1,
            maxWidth: '12rem',
            overflow: 'hidden',
            paddingLeft: '0.5rem !important',
            paddingRight: '0.5rem !important',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
            width: '12rem',
          }}
        />
        <Column
          body={messageTemplate}
          field='message'
          filter
          header='Message'
          sortable
          style={{
            flexGrow: 0,
            flexShrink: 1,
            overflow: 'hidden',
            paddingLeft: '0.5rem !important',
            paddingRight: '0.5rem !important',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap',
            width: '40rem',
          }}

        />
      </DataTable>
    </div >
  );
}

export default memo(LogViewer);


