/* eslint-disable @typescript-eslint/no-unused-vars */
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import DataSelector from "../features/dataSelector/DataSelector";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import { UpdateSetting } from "../store/signlar_functions_local";
import { Toast } from "primereact/toast";
import { arraysMatch } from "../common/common";
import { GetSchedules } from "../store/signlar_functions";

const SchedulesDirectSchedulesDataSelector = (props: SchedulesDirectSchedulesDataSelectorProps) => {
  const toast = React.useRef<Toast>(null);

  // const [selectedLineUpPreviews, setSelectedLineUpPreviews] = React.useState<StreamMasterApi.LineUpPreview[]>([] as StreamMasterApi.LineUpPreview[]);
  // const lineupPreviews = StreamMasterApi.useSchedulesDirectGetLineupPreviewsQuery();
  const settings = StreamMasterApi.useSettingsGetSettingQuery();
  const [dataSource, setDataSource] = React.useState<StreamMasterApi.Schedule[]>([] as StreamMasterApi.Schedule[]);
  const [isLoading, setIsLoading] = React.useState<boolean>(false);

  React.useEffect(() => {
    if (props.stationIds.length === 0) {
      return;
    }

    setIsLoading(true);
    GetSchedules().
      then((data) => {
        setDataSource(data);
        setIsLoading(false);
      }).catch(() => {
        setIsLoading(false);
        if (toast.current) {
          toast.current.show({
            detail: `Get Schedules Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Failed',
          });
        }
      });


  }, [props.stationIds]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      { field: 'stationID', header: 'Station Id' },
      { field: 'metadata.startDate', header: 'metadata Id' }
    ]
  }, []);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className='m3uFilesEditor flex flex-column border-2 border-round surface-border'>
        <DataSelector
          columns={sourceColumns}
          dataSource={dataSource}
          emptyMessage="No Line Ups"
          enableState={false}
          id='SchedulesDirectSchedulesDataSelector-ds'
          isLoading={isLoading}
          key="callsign"
          name="Schedules"
          selectionMode='multiple'
          showHeaders
          sortField='channel'
          style={{ height: 'calc(50vh - 40px)' }}
        />
      </div>
    </>
  );
}

SchedulesDirectSchedulesDataSelector.displayName = 'SchedulesDirectSchedulesDataSelector';
SchedulesDirectSchedulesDataSelector.defaultProps = {

};

export type SchedulesDirectSchedulesDataSelectorProps = {
  stationIds: string[];
};
export default React.memo(SchedulesDirectSchedulesDataSelector);
