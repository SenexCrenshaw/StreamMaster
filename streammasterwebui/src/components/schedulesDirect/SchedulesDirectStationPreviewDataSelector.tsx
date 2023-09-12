
import { Toast } from "primereact/toast";
import { memo, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { arraysMatch } from "../../common/common";
import { UpdateSetting } from "../../smAPI/Settings/SettingsMutateAPI";
import { useSchedulesDirectGetStationPreviewsQuery, useSettingsGetSettingQuery, type StationPreview, type UpdateSettingRequest } from "../../store/iptvApi";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";
import SchedulesDirectSchedulesDataSelector from "./SchedulesDirectSchedulesDataSelector";

const SchedulesDirectStationPreviewDataSelector = () => {
  const toast = useRef<Toast>(null);

  const [selectedStationPreviews, setSelectedStationPreviews] = useState<StationPreview[]>([] as StationPreview[]);
  const [selectedStationPreviewsIds, setSelectedStationPreviewsIds] = useState<string[]>([] as string[]);
  const stationPreviews = useSchedulesDirectGetStationPreviewsQuery();
  const settings = useSettingsGetSettingQuery();

  useEffect(() => {
    if (selectedStationPreviews.length > 0 || settings.isLoading || !settings.data) {
      return;
    }

    if (selectedStationPreviews.length > 0 || !settings.data.sdStationIds || settings.data.sdStationIds.length === 0) {
      return;
    }

    const newStationPreviews = settings.data.sdStationIds.map((stationId) => {
      return stationPreviews.data?.find((stationPreview) => stationPreview.stationId === stationId);
    }).filter((stationPreview) => stationPreview !== undefined) as StationPreview[];

    setSelectedStationPreviews(newStationPreviews);
    setSelectedStationPreviewsIds(newStationPreviews.map((stationPreview) => stationPreview.stationId).filter((id) => id !== undefined) as string[]);
  }, [stationPreviews.data, selectedStationPreviews.length, settings]);



  const onSave = useCallback((stationIds: string[]) => {
    const previousStationIds = selectedStationPreviews.map((stationPreview) => stationPreview.stationId).filter((stationId) => stationId !== undefined) as string[];

    if (arraysMatch(stationIds, previousStationIds)) {
      return;
    }


    const newData = {} as UpdateSettingRequest;

    newData.sdStationIds = stationIds;

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
      }).catch(() => {
        if (toast.current) {
          toast.current.show({
            detail: `Update Station Ids Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error'
          });
        }
      });

  }, [selectedStationPreviews]);

  const onSelectedStationPreviews = useCallback((selectedData: StationPreview | StationPreview[]) => {

    if (Array.isArray(selectedData)) {

      setSelectedStationPreviews(selectedData);
      setSelectedStationPreviewsIds(selectedData.map((stationPreview) => stationPreview.stationId).filter((id) => id !== undefined) as string[]);
      onSave(selectedData.map((stationPreview) => stationPreview.stationId).filter((stationId) => stationId !== undefined) as string[]);
    } else {
      setSelectedStationPreviews([selectedData]);

      if (selectedData.stationId !== undefined) {
        setSelectedStationPreviewsIds([selectedData.stationId]);
        onSave([selectedData.stationId]);

      }
    }

  }, [onSave]);

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'stationId', header: 'Station Id' },
      { field: 'lineUp', header: 'Line Up', sortable: true },
      { field: 'name', header: 'Name', sortable: true },
      { field: 'callsign', header: 'Call Sign', sortable: true },
      { field: 'affiliate', header: 'Affiliate' },
    ]
  }, []);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className='m3uFilesEditor flex flex-column border-2 border-round surface-border'>
        <DataSelector
          columns={sourceColumns}
          dataSource={stationPreviews.data}
          emptyMessage="No Line Ups"

          headerName="Line Up Preview"
          id='SchedulesDirectStationPreviewDataSelector'
          isLoading={stationPreviews.isLoading}
          onSelectionChange={(e) => {
            onSelectedStationPreviews(e as StationPreview[]);
          }
          }
          selectionMode='multiple'

          style={{ height: 'calc(50vh - 40px)' }}
        />
        <SchedulesDirectSchedulesDataSelector stationIds={selectedStationPreviewsIds} />
      </div>
    </>
  );
}

SchedulesDirectStationPreviewDataSelector.displayName = 'SchedulesDirectStationPreviewDataSelector';

export default memo(SchedulesDirectStationPreviewDataSelector);
