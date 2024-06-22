import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { Logger } from '@lib/common/logger';
import { StreamStreamingStatistic } from '@lib/smAPI/smapiTypes';
import { useCallback, useMemo } from 'react';

interface SMChannelStatusProperties {
  readonly ChannelId: number;
  readonly CurrentRank: number;
  readonly StreamStreamingStatistics: StreamStreamingStatistic[];
}

const SMChannelStatusValue = ({ ChannelId, CurrentRank, StreamStreamingStatistics }: SMChannelStatusProperties) => {
  const clientBitsPerSecondTemplate = (rowData: StreamStreamingStatistic) => {
    if (rowData.BitsPerSecond === undefined) return <div />;

    const kbps = rowData.BitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const elapsedTSTemplate = useCallback((rowData: StreamStreamingStatistic) => {
    return <div className="numeric-field">{getElapsedTimeString(rowData.StartTime, new Date().toString(), true)}</div>;
  }, []);

  const clientStartTimeTemplate = (rowData: StreamStreamingStatistic) => <div>{formatJSONDateString(rowData.StartTime ?? '')}</div>;

  const logoTemplate = useCallback((rowData: StreamStreamingStatistic) => {
    return (
      <div className="flex icon-button-template justify-content-center align-items-center w-full">
        <img alt="Icon logo" src={rowData.StreamLogo} />
      </div>
    );
  }, []);

  const rowClass = useCallback(
    (data: any): string => {
      if (!data) {
        return '';
      }

      if (data.Rank === CurrentRank) {
        return 'channel-row-selected';
      }

      return '';
    },
    [CurrentRank]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { align: 'right', field: 'Rank', width: 50 },
      { bodyTemplate: logoTemplate, field: 'Logo', fieldType: 'image', header: '' },
      { field: 'StreamName', filter: true, width: 200 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'BitsPerSecond', header: 'Input Kbps', width: 80 },
      { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'Start', width: 180 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss)', width: 85 }
    ],
    [elapsedTSTemplate, logoTemplate]
  );

  // const dataSource = useMemo(() => {
  //   if (streamStreamingStatistics === undefined || channelId === undefined) return [];
  //   const data = streamStreamingStatistics.find((x) => x.Id === channelId);
  //   if (data === undefined) return [];
  //   return [data];
  // }, [channelId, clientStreamingStatistics]);

  // if (dataSource === undefined) return <div>Loading...</div>;

  Logger.debug('SMChannelStatusValue', ChannelId, 'dataSource', StreamStreamingStatistics);

  return (
    <SMDataTable
      defaultSortField="Rank"
      defaultSortOrder={1}
      headerName="Streams"
      rowClass={rowClass}
      columns={columns}
      id="channelStatus"
      dataSource={StreamStreamingStatistics}
    />
  );
};

export default SMChannelStatusValue;
