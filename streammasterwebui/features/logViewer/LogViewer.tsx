'use client';

import StandardHeader from '@components/StandardHeader';
import DownArrowButton from '@components/buttons/DownArrowButton';
import { ExportComponent, formatJSONDateString } from '@lib/common/common';
import { LogIcon } from '@lib/common/icons';
import useScrollAndKeyEvents from '@lib/hooks/useScrollAndKeyEvents';
import { LogEntry, LogEntryDto } from '@lib/iptvApi';
import { GetLog } from '@lib/smAPI/Logs/LogsGetAPI';

import { FilterMatchMode } from 'primereact/api';
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';

const LogViewer = () => {
  const itemSize: number = 30;
  const [lastLogId, setLastLogId] = useState<number>(0);
  const [dataSource, setDataSource] = useState<LogEntryDto[]>([] as LogEntryDto[]);
  const tableRef = useRef<DataTable<LogEntryDto[]>>(null);
  const [follow, setFollow] = useState<boolean>(true);
  const [lastScrollIndex, setLastScrollIndex] = useState<number>(0);

  const filters = {
    logLevelName: { matchMode: FilterMatchMode.CONTAINS, value: null },
    message: { matchMode: FilterMatchMode.CONTAINS, value: null },
    timeStamp: { matchMode: FilterMatchMode.CONTAINS, value: null },
  };

  const { type, direction, state } = useScrollAndKeyEvents();

  useEffect(() => {
    if (!follow) {
      if (direction === 'down' && state === 'blocked') {
        setFollow(true);
      }
      return;
    }

    if (state === 'moved') {
      setFollow(false);
    }

    console.log('direction2', type, direction, state);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [type, direction, state]);

  const tryScroll = useCallback(
    (scollTo: number) => {
      if (follow && scollTo !== lastScrollIndex && tableRef.current?.getVirtualScroller()) {
        const vs = tableRef.current.getVirtualScroller();
        console.log('scroll', scollTo, lastScrollIndex);
        vs.scrollToIndex(scollTo * itemSize);
      }
      setLastScrollIndex(scollTo);
    },
    [follow, lastScrollIndex],
  );

  const getLogData = useCallback(() => {
    console.log('getLogData', lastLogId, dataSource.length, lastScrollIndex);

    GetLog({ lastId: lastLogId, maxLines: 5000 })
      .then((data: LogEntryDto[] | null) => {
        if (!data) return;
        const uniqueData = data.filter((item) => !dataSource.some((existingItem) => existingItem.id === item.id));

        if (uniqueData.length > 0) {
          setDataSource((prevDataSource) => [...prevDataSource, ...uniqueData]);
          setLastLogId(uniqueData[uniqueData.length - 1].id);
        } else {
          tryScroll(dataSource.length - 1);
        }
      })
      .catch((error) => {
        console.log(error);
      });
  }, [lastLogId, dataSource, lastScrollIndex, tryScroll]);

  useEffect(() => {
    const intervalId = setInterval(() => {
      getLogData();
    }, 1000);

    return () => {
      clearInterval(intervalId);
    };
  }, [getLogData]);

  const timeStampTemplate = useCallback((rowData: LogEntry) => {
    return <div>{formatJSONDateString(rowData.timeStamp ?? '')}</div>;
  }, []);

  const levelTemplate = useCallback((rowData: LogEntry) => {
    switch (rowData.logLevel) {
      case 0:
        return <div className="text-gray-600">Trace</div>;
      case 1:
        return <div className="text-blue-600">Debug</div>;
      case 2:
        return <div className="text-green-600">Information</div>;
      case 3:
        return <div className="text-yellow-600">Warning</div>;
      case 4:
        return <div className="text-red-600">Error</div>;
      case 5:
        return <div className="text-purple-600">Critical</div>;
      case 6:
        return <div className="text-gray-400">None</div>;
      default:
        return <div className="text-gray-500">Unknown</div>;
    }
  }, []);

  const messageTemplate = useCallback((rowData: LogEntry) => {
    return <div>{rowData.message}</div>;
  }, []);

  const exportCSV = () => {
    tableRef.current?.exportCSV({ selectionOnly: false });
  };

  const renderHeader = useMemo(() => {
    return (
      <div className="flex grid flex-row w-full align-items-center justify-content-end col-12 h-full p-0 debug">
        <ExportComponent exportCSV={exportCSV} />
        <DownArrowButton
          onClick={() => {
            setFollow(true);
            tryScroll(dataSource.length);
          }}
          tooltip="Follow"
        />
      </div>
    );
  }, [dataSource.length, tryScroll]);

  return (
    <StandardHeader className="dataselector" displayName={follow ? 'LOGS - FOLLOWING' : 'LOGS'} icon={<LogIcon />}>
      <DataTable
        className="w-full text-sm"
        exportFilename={`StreamMaster_Logs_${new Date().toISOString()}`}
        filterDisplay="row"
        filters={filters}
        header={renderHeader}
        id="LogViewer"
        onScroll={(e) => {
          console.log('ssss', e);
          setFollow(false);
        }}
        ref={tableRef}
        scrollable
        scrollHeight="calc(100vh - 100px)"
        style={{
          height: 'calc(100vh - 200px)',
        }}
        value={dataSource}
        virtualScrollerOptions={{
          itemSize: itemSize,
        }}
      >
        <Column
          body={levelTemplate}
          field="logLevelName"
          filter
          header="Level"
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
          field="timeStamp"
          filter
          header="Time"
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
          field="message"
          filter
          header="Message"
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
    </StandardHeader>
  );
};

export default memo(LogViewer);
