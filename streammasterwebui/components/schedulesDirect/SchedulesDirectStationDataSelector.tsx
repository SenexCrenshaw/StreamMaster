import ResetButton from '@components/buttons/ResetButton';
import { useLineUpColumnConfig } from '@components/columns/useLineUpColumnConfig';
import SMButton from '@components/sm/SMButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { arraysEqualByKey } from '@components/smDataTable/helpers/arraysEqual';

import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { SetStations } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import useGetSelectedStationIds from '@lib/smAPI/SchedulesDirect/useGetSelectedStationIds';
import useGetStationPreviews from '@lib/smAPI/SchedulesDirect/useGetStationPreviews';
import { SetStationsRequest, StationPreview, StationRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';

const SchedulesDirectStationDataSelector = () => {
  const dataKey = 'SchedulesDirectSchedulesDataSelector';
  const schedulesDirectGetSelectedStationIdsQuery = useGetSelectedStationIds();
  const stationPreviews = useGetStationPreviews();
  const { columnConfig: lineUpColumnConfig } = useLineUpColumnConfig();
  const { selectedItems, setSelectedItems } = useSelectedItems<StationPreview>(dataKey);
  const [originalSelectedItems, setOriginalSelectedItems] = useState<StationPreview[] | undefined>(undefined);
  const isInitialLoad = useRef(true);

  useEffect(() => {
    if (
      (originalSelectedItems !== undefined && originalSelectedItems.length > 0) ||
      schedulesDirectGetSelectedStationIdsQuery.isLoading ||
      schedulesDirectGetSelectedStationIdsQuery.data === undefined ||
      schedulesDirectGetSelectedStationIdsQuery.data.length === 0 ||
      stationPreviews.isLoading ||
      stationPreviews.data === undefined ||
      stationPreviews.data.length === 0
    ) {
      return;
    }

    const sp = schedulesDirectGetSelectedStationIdsQuery.data
      .map((stationIdLineUp) =>
        stationPreviews.data?.find(
          (stationPreview) => stationPreview.StationId === stationIdLineUp.StationId && stationPreview.Lineup === stationIdLineUp.Lineup
        )
      )
      .filter((station) => station !== undefined) as StationPreview[];

    if (isInitialLoad.current) {
      if (originalSelectedItems === undefined || originalSelectedItems.length === 0) {
        setOriginalSelectedItems(sp);
        setSelectedItems(sp);
      }
      isInitialLoad.current = false;
    }
  }, [
    originalSelectedItems,
    schedulesDirectGetSelectedStationIdsQuery.isLoading,
    schedulesDirectGetSelectedStationIdsQuery.data,
    stationPreviews.data,
    setOriginalSelectedItems,
    setSelectedItems,
    stationPreviews.isLoading
  ]);

  const isSaveEnabled = useMemo(() => {
    if (originalSelectedItems === undefined) {
      if (selectedItems === undefined) {
        return false;
      }
      if (selectedItems.length > 0) {
        return true;
      }

      return false;
    }

    if (originalSelectedItems) {
      const test = arraysEqualByKey(originalSelectedItems, selectedItems, 'StationId');
      return !test;
    }

    return false;
  }, [originalSelectedItems, selectedItems]);

  const dataSource = useMemo(() => {
    if (stationPreviews.data === undefined) {
      return [];
    }
    return stationPreviews.data;
  }, [stationPreviews.data]);

  const onSave = useCallback(
    (stationIdLineUps: StationPreview[]) => {
      if (stationIdLineUps === undefined) {
        return;
      }

      if (originalSelectedItems !== undefined) {
        if (arraysEqualByKey(originalSelectedItems, selectedItems, 'StationId')) {
          return;
        }
      }

      const request = {} as SetStationsRequest;

      request.Requests = selectedItems.map((station) => {
        const request: StationRequest = { Lineup: station.Lineup, StationId: station.StationId };
        return request;
      });

      SetStations(request)
        .then(() => {})
        .catch(() => {
          Logger.error('error');
        })
        .finally(() => {
          setOriginalSelectedItems(selectedItems);
        });
    },
    [originalSelectedItems, selectedItems]
  );

  function imageBodyTemplate(data: StationPreview) {
    if (!data?.Logo || data.Logo.Url === '') {
      return <div />;
    }

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img loading="lazy" alt={data.Logo.Url ?? 'Logo'} className="max-h-1rem max-w-full p-0" src={`${encodeURI(data.Logo.Url ?? '')}`} />
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

  const headerRight = useMemo((): React.ReactNode => {
    return (
      <div className="flex w-12 gap-1 justify-content-end align-content-center">
        <ResetButton
          buttonDisabled={!isSaveEnabled}
          onClick={() => {
            setSelectedItems(originalSelectedItems);
          }}
        />
        <SMButton
          buttonDisabled={!isSaveEnabled}
          icon="pi-save"
          buttonClassName="pr-3 icon-green"
          iconFilled
          label="Save"
          onClick={() => onSave(selectedItems)}
        />
      </div>
    );
  }, [isSaveEnabled, onSave, originalSelectedItems, selectedItems, setSelectedItems]);

  return (
    <div className="w-full">
      <SMDataTable
        arrayKey="StationId"
        columns={columns}
        dataSource={dataSource}
        defaultSortField="name"
        emptyMessage="No Line Ups"
        enablePaginator
        headerName="SD Channels"
        headerRightTemplate={headerRight}
        id="SchedulesDirectStationDataSelector"
        lazy
        noIsLoading={true}
        selectedItemsKey={dataKey}
        selectionMode="multiple"
        showSortSelected
        showSelectAll={false}
        style={{ height: 'calc(100vh - 100px)' }}
      />
    </div>
  );
};

SchedulesDirectStationDataSelector.displayName = 'SchedulesDirectStationDataSelector';

export default memo(SchedulesDirectStationDataSelector);
