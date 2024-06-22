import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { useSMContext } from '@lib/signalr/SMProvider';
import { ChannelStreamingStatistics } from '@lib/smAPI/smapiTypes';
import { DataTableRowData, DataTableRowExpansionTemplate } from 'primereact/datatable';
import { useCallback, useMemo } from 'react';
import SMChannelStatusValue from './SMChannelStatusValue';

const SMChannelStatus = () => {
  const { channelStreamingStatistics } = useSMContext();

  const clientBitsPerSecondTemplate = (rowData: ChannelStreamingStatistics) => {
    if (rowData.BitsPerSecond === undefined) return <div />;

    const kbps = rowData.BitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const elapsedTSTemplate = useCallback((rowData: ChannelStreamingStatistics) => {
    return <div className="numeric-field">{getElapsedTimeString(rowData.StartTime, new Date().toString(), true)}</div>;
  }, []);

  const clientStartTimeTemplate = (rowData: ChannelStreamingStatistics) => <div>{formatJSONDateString(rowData.StartTime ?? '')}</div>;

  const logoTemplate = useCallback((rowData: ChannelStreamingStatistics) => {
    return (
      <div className="flex icon-button-template justify-content-center align-items-center w-full">
        <img alt="Icon logo" src={rowData.ChannelLogo} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { bodyTemplate: logoTemplate, field: 'Logo', fieldType: 'image', header: '' },
      { field: 'ChannelName', filter: true, sortable: true, width: 200 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'BitsPerSecond', header: 'Input Kbps', width: 80 },
      { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'Start', width: 180 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss)', width: 85 }
    ],
    [elapsedTSTemplate, logoTemplate]
  );

  const rowExpansionTemplate = useCallback((data: DataTableRowData<any>, options: DataTableRowExpansionTemplate) => {
    const ChannelStreamingStatistics = data as unknown as ChannelStreamingStatistics;

    return (
      <div className="ml-3 m-1">
        {
          <SMChannelStatusValue
            ChannelId={ChannelStreamingStatistics.Id}
            CurrentRank={ChannelStreamingStatistics.CurrentRank}
            StreamStreamingStatistics={ChannelStreamingStatistics.StreamStreamingStatistics}
          />
        }
      </div>
    );
  }, []);

  if (channelStreamingStatistics === undefined) return <div>Loading...</div>;

  return (
    <SMDataTable
      headerName="CHANNELS"
      columns={columns}
      id="channelStatus"
      dataSource={channelStreamingStatistics}
      showExpand
      rowExpansionTemplate={rowExpansionTemplate}
    />
  );
};

export default SMChannelStatus;
