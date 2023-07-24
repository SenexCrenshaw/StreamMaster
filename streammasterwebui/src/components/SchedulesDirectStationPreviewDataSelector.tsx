import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import DataSelector from "../features/dataSelector/DataSelector";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import { UpdateSetting } from "../store/signlar_functions_local";
import { Toast } from "primereact/toast";
import { arraysMatch } from "../common/common";
import SchedulesDirectSchedulesDataSelector from "./SchedulesDirectSchedulesDataSelector";

const SchedulesDirectStationPreviewDataSelector = () => {
  const toast = React.useRef<Toast>(null);

  const [selectedStationPreviews, setSelectedStationPreviews] = React.useState<StreamMasterApi.StationPreview[]>([] as StreamMasterApi.StationPreview[]);
  const [selectedStationPreviewsIds, setSelectedStationPreviewsIds] = React.useState<string[]>([] as string[]);
  const stationPreviews = StreamMasterApi.useSchedulesDirectGetStationPreviewsQuery();
  const settings = StreamMasterApi.useSettingsGetSettingQuery();

  React.useEffect(() => {
    if (selectedStationPreviews.length > 0 || settings.isLoading || !settings.data) {
      return;
    }

    if (selectedStationPreviews.length > 0 || !settings.data.sdStationIds || settings.data.sdStationIds.length === 0) {
      return;
    }

    const newStationPreviews = settings.data.sdStationIds.map((stationId) => {
      return stationPreviews.data?.find((stationPreview) => stationPreview.stationId === stationId);
    }).filter((stationPreview) => stationPreview !== undefined) as StreamMasterApi.StationPreview[];

    setSelectedStationPreviews(newStationPreviews);
    setSelectedStationPreviewsIds(newStationPreviews.map((stationPreview) => stationPreview.stationId).filter((id) => id !== undefined) as string[]);
  }, [stationPreviews.data, selectedStationPreviews.length, settings]);



  const onSave = React.useCallback((stationIds: string[]) => {
    const previousStationIds = selectedStationPreviews.map((stationPreview) => stationPreview.stationId).filter((stationId) => stationId !== undefined) as string[];

    if (arraysMatch(stationIds, previousStationIds)) {
      return;
    }


    const newData = {} as StreamMasterApi.UpdateSettingRequest;

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

  const onSelectedStationPreviews = React.useCallback((selectedData: StreamMasterApi.StationPreview | StreamMasterApi.StationPreview[]) => {

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

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
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
          enableState={false}
          id='SchedulesDirectStationPreviewDataSelector'
          isLoading={stationPreviews.isLoading}
          name="Line Up Preview"
          onSelectionChange={(e) => {
            onSelectedStationPreviews(e as StreamMasterApi.StationPreview[]);
          }
          }
          selection={selectedStationPreviews}
          selectionMode='multiple'
          showHeaders
          sortField='channel'
          style={{ height: 'calc(50vh - 40px)' }}
        />
        <SchedulesDirectSchedulesDataSelector stationIds={selectedStationPreviewsIds} />
      </div>
    </>
  );
}

SchedulesDirectStationPreviewDataSelector.displayName = 'SchedulesDirectStationPreviewDataSelector';

export default React.memo(SchedulesDirectStationPreviewDataSelector);
