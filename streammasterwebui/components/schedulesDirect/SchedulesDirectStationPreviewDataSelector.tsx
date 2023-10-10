import { findDifferenceStationIdLineUps } from '@/lib/common/common';
import {
  StationIdLineUp,
  UpdateSettingRequest,
  useSchedulesDirectGetSelectedStationIdsQuery,
  useSchedulesDirectGetStationPreviewsQuery,
  type StationPreview,
} from '@/lib/iptvApi';
import { useSelectedItems } from '@/lib/redux/slices/useSelectedItemsSlice';
import { UpdateSetting } from '@/lib/smAPI/Settings/SettingsMutateAPI';
import { Toast } from 'primereact/toast';
import { memo, useCallback, useEffect, useMemo, useRef } from 'react';
import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
const SchedulesDirectStationPreviewDataSelector = () => {
  const toast = useRef<Toast>(null);

  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<StationPreview>('SchedulesDirectSchedulesDataSelector');

  const schedulesDirectGetSelectedStationIdsQuery = useSchedulesDirectGetSelectedStationIdsQuery();
  const stationPreviews = useSchedulesDirectGetStationPreviewsQuery();

  useEffect(() => {
    if (
      schedulesDirectGetSelectedStationIdsQuery.isLoading ||
      schedulesDirectGetSelectedStationIdsQuery.data === undefined ||
      stationPreviews.data === undefined
    ) {
      return;
    }

    const sp = schedulesDirectGetSelectedStationIdsQuery.data
      .map((stationIdLineUp) => {
        return stationPreviews.data?.find(
          (stationPreview) => stationPreview.stationId === stationIdLineUp.stationId && stationPreview.lineUp === stationIdLineUp.lineUp,
        );
      })
      .filter((station) => station !== undefined) as StationPreview[];

    if (findDifferenceStationIdLineUps(sp, schedulesDirectGetSelectedStationIdsQuery.data).length !== 0) {
      setSelectSelectedItems(sp as StationPreview[]);
    }
  }, [schedulesDirectGetSelectedStationIdsQuery.data, schedulesDirectGetSelectedStationIdsQuery.isLoading, setSelectSelectedItems, stationPreviews.data]);

  const onSave = useCallback(
    (stationIdLineUps: StationIdLineUp[]) => {
      if (stationIdLineUps === undefined || schedulesDirectGetSelectedStationIdsQuery.data === undefined) {
        return;
      }
      const test = findDifferenceStationIdLineUps(schedulesDirectGetSelectedStationIdsQuery.data, stationIdLineUps);
      if (test.length === 0) {
        return;
      }

      const newData = {} as UpdateSettingRequest;

      newData.sdStationIds = stationIdLineUps;

      UpdateSetting(newData)
        .then(() => {
          if (toast.current) {
            toast.current.show({
              detail: `Update Station Ids Successful`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
          }
        })
        .catch(() => {
          if (toast.current) {
            toast.current.show({
              detail: `Update Station Ids Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }
        });
    },
    [schedulesDirectGetSelectedStationIdsQuery.data],
  );

  useEffect(() => {
    console.log('selectSelectedItems', selectSelectedItems);
    const lineUps = selectSelectedItems.map((stationPreview) => {
      return {
        lineUp: stationPreview.lineUp,
        stationId: stationPreview.stationId,
      };
    });
    onSave(lineUps);
  }, [onSave, selectSelectedItems]);

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'stationId', filter: true, header: 'Station Id', sortable: true },
      { field: 'lineUp', header: 'Line Up', sortable: true },
      { field: 'name', filter: true, header: 'Name', sortable: true },
      { field: 'callsign', filter: true, header: 'Call Sign', sortable: true },
      { field: 'affiliate', filter: true, header: 'Affiliate', sortable: true },
    ];
  }, []);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="m3uFilesEditor flex flex-column border-2 border-round surface-border">
        <DataSelector
          columns={sourceColumns}
          dataSource={stationPreviews.data}
          disableSelectAll={true}
          emptyMessage="No Line Ups"
          headerName="Line Up Preview"
          id="SchedulesDirectStationPreviewDataSelector"
          isLoading={stationPreviews.isLoading}
          onRowClick={(e) => {
            console.log(e);
          }}
          selectedItemsKey="SchedulesDirectSchedulesDataSelector"
          selectionMode="multiple"
          showSelections
          style={{ height: 'calc(100vh - 40px)' }}
        />
        {/* <SchedulesDirectProgramsDataSelector id="SchedulesDirectStationPreviewDataSelector2" /> */}
      </div>
    </>
  );
};

SchedulesDirectStationPreviewDataSelector.displayName = 'SchedulesDirectStationPreviewDataSelector';

export default memo(SchedulesDirectStationPreviewDataSelector);
