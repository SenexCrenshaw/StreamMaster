import { useLineUpColumnConfig } from '@components/columns/useLineUpColumnConfig';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { compareStationPreviews, findDifferenceStationIdLineUps } from '@lib/common/common';
import { Logger } from '@lib/common/logger';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { AddStation, RemoveStation } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import useGetSelectedStationIds from '@lib/smAPI/SchedulesDirect/useGetSelectedStationIds';
import useGetStationPreviews from '@lib/smAPI/SchedulesDirect/useGetStationPreviews';
import { AddStationRequest, RemoveStationRequest, StationPreview, StationRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useEffect, useMemo } from 'react';

const SchedulesDirectStationDataSelector = () => {
  const dataKey = 'SchedulesDirectSchedulesDataSelector';
  const { selectedItems, setSelectedItems } = useSelectedItems<StationPreview>(dataKey);
  const schedulesDirectGetSelectedStationIdsQuery = useGetSelectedStationIds();
  const stationPreviews = useGetStationPreviews();
  const { columnConfig: lineUpColumnConfig } = useLineUpColumnConfig();

  // Logger.debug('SchedulesDirectStationDataSelector', { stationPreviews: stationPreviews.data?.length });

  useEffect(() => {
    if (
      schedulesDirectGetSelectedStationIdsQuery.isLoading ||
      schedulesDirectGetSelectedStationIdsQuery.data === undefined ||
      stationPreviews.data === undefined
    ) {
      return;
    }

    const station19614 = stationPreviews.data.find((station) => station.StationId === '19614');
    const station19614Selected = schedulesDirectGetSelectedStationIdsQuery.data.some((station) => station.StationId === '19614');

    const sp = schedulesDirectGetSelectedStationIdsQuery.data
      .map((stationIdLineUp) =>
        stationPreviews.data?.find(
          (stationPreview) => stationPreview.StationId === stationIdLineUp.StationId && stationPreview.Lineup === stationIdLineUp.Lineup
        )
      )
      .filter((station) => station !== undefined) as StationPreview[];

    //   setSelectedItems(sp as StationPreview[]);
    if (findDifferenceStationIdLineUps(sp, selectedItems).length > 0) {
      setSelectedItems(sp as StationPreview[]);
    }

    Logger.debug('SchedulesDirectStationDataSelector', { sp, station19614, station19614Selected });
  }, [
    schedulesDirectGetSelectedStationIdsQuery.data,
    schedulesDirectGetSelectedStationIdsQuery.isLoading,
    selectedItems,
    setSelectedItems,
    stationPreviews.data
  ]);

  const onSave = useCallback(
    (stationIdLineUps: StationPreview[]) => {
      if (stationIdLineUps === undefined || schedulesDirectGetSelectedStationIdsQuery.data === undefined) {
        return;
      }

      const { added, removed } = compareStationPreviews(selectedItems, stationIdLineUps);

      if (added.length === 0 && removed.length === 0) {
        return;
      }
      // setIsLoading(true);

      if (added !== undefined && added.length > 0) {
        const toSend = {} as AddStationRequest;

        toSend.Requests = added.map((station) => {
          const request: StationRequest = { Lineup: station.Lineup, StationId: station.StationId };
          return request;
        });

        AddStation(toSend)
          .then(() => {})
          .catch(() => {
            Logger.error('error');
          })
          .finally(() => {
            setSelectedItems([] as StationPreview[]);
            // setIsLoading(false);
          });
      }

      if (removed !== undefined && removed.length > 0) {
        const toSend = {} as RemoveStationRequest;

        toSend.Requests = removed.map((station) => {
          const request: StationRequest = { Lineup: station.Lineup, StationId: station.StationId };
          return request;
        });

        RemoveStation(toSend)
          .then(() => {})
          .catch(() => {
            Logger.error('error');
          })
          .finally(() => {
            setSelectedItems([] as StationPreview[]);
            // setIsLoading(false);
          });
      }
    },
    [schedulesDirectGetSelectedStationIdsQuery.data, selectedItems, setSelectedItems]
  );

  function imageBodyTemplate(data: StationPreview) {
    if (!data?.Logo || data.Logo.URL === '') {
      return <div />;
    }

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img loading="lazy" alt={data.Logo.URL ?? 'Logo'} className="max-h-1rem max-w-full p-0" src={`${encodeURI(data.Logo.URL ?? '')}`} />
      </div>
    );
  }

  const columns = useMemo((): ColumnMeta[] => {
    const columnConfigs: ColumnMeta[] = [
      { field: 'StationId', filter: true, header: 'Station Id', sortable: true, width: 40 },
      { bodyTemplate: imageBodyTemplate, field: 'image', fieldType: 'image', width: 24 }
    ];
    // // columnConfigs.push(channelGroupConfig);
    columnConfigs.push(lineUpColumnConfig);
    columnConfigs.push({ field: 'Name', filter: true, sortable: true, width: 100 });
    columnConfigs.push({ field: 'Callsign', filter: true, header: 'Call Sign', sortable: true, width: 50 });
    columnConfigs.push({ field: 'Affiliate', filter: true, sortable: true, width: 80 });

    return columnConfigs;
  }, [lineUpColumnConfig]);

  Logger.debug('SchedulesDirectStationDataSelector', { stationPreviews: stationPreviews.data?.length });
  return (
    <div className="w-full">
      <SMDataTable
        columns={columns}
        dataSource={stationPreviews.data}
        defaultSortField="name"
        emptyMessage="No Line Ups"
        enablePaginator
        headerName="SD Channels"
        id="SchedulesDirectStationDataSelector"
        lazy
        noIsLoading={true}
        onSelectionChange={(e) => {
          onSave(e);
        }}
        selectedItemsKey={dataKey}
        selectionMode="multiple"
        showSelected
        showSelectAll={false}
        style={{ height: 'calc(100vh - 100px)' }}
      />
    </div>
  );
};

SchedulesDirectStationDataSelector.displayName = 'SchedulesDirectStationDataSelector';

export default memo(SchedulesDirectStationDataSelector);
