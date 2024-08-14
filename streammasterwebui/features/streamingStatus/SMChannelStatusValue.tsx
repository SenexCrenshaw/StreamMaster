import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { formatJSONDateString, getElapsedTimeString } from '@lib/common/dateTime';
import { Logger } from '@lib/common/logger';
import { ClientChannelDto } from '@lib/smAPI/smapiTypes';
import { useCallback, useMemo } from 'react';

interface SMChannelStatusProperties {
  readonly clientMetrics: ClientChannelDto[];
}

const SMChannelStatusValue = ({ clientMetrics }: SMChannelStatusProperties) => {
  Logger.debug('SMChannelStatusValue:', clientMetrics);

  const clientBitsPerSecondTemplate = (rowData: ClientChannelDto) => {
    if (rowData.BitsPerSecond === undefined) return <div />;

    const kbps = rowData.BitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);

    return <div>{roundedKbps.toLocaleString('en-US')}</div>;
  };

  const elapsedTSTemplate = useCallback((rowData: ClientChannelDto) => {
    return <div className="numeric-field">{getElapsedTimeString(rowData.StartTime, new Date().toString(), true)}</div>;
  }, []);

  const clientStartTimeTemplate = (rowData: ClientChannelDto) => <div>{formatJSONDateString(rowData.StartTime ?? '')}</div>;

  const logoTemplate = useCallback(
    (rowData: ClientChannelDto) => {
      if (rowData === undefined || rowData.Logo === null || rowData.Logo === undefined || rowData.Logo === '') return <div />;
      const found = clientMetrics.find((predicate) => predicate.SMChannelId === rowData.SMChannelId);
      if (found === undefined || found.Logo === null || found.Logo === undefined || found.Logo === '') return <div />;

      return (
        <div className="flex icon-button-template justify-content-center align-items-center w-full">
          <img alt="Icon logo" src={found.Logo} />
        </div>
      );
    },
    [clientMetrics]
  );

  const rowClass = useCallback((data: any): string => {
    if (!data) {
      return '';
    }

    // if (data.Rank === CurrentRank) {
    //   return 'channel-row-selected';
    // }

    return '';
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      // { align: 'right', field: 'Rank', width: 50 },
      { bodyTemplate: logoTemplate, field: 'Logo', fieldType: 'image', header: '' },
      { field: 'Name', filter: true, width: 200 },
      { align: 'right', bodyTemplate: clientBitsPerSecondTemplate, field: 'BitsPerSecond', header: 'Input Kbps', width: 80 },
      { align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'StartTime', header: 'Start', width: 180 },
      { align: 'right', bodyTemplate: elapsedTSTemplate, field: 'ElapsedTime', header: '(d hh:mm:ss)', width: 85 }
    ],
    [elapsedTSTemplate, logoTemplate]
  );

  return <SMDataTable id="SMChannelId" noSourceHeader rowClass={rowClass} columns={columns} dataSource={clientMetrics} />;
};

export default SMChannelStatusValue;
