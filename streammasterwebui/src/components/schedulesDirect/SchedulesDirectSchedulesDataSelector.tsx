

import { Toast } from "primereact/toast";
import { useRef, useState, useEffect, useMemo, memo } from "react";
import { type Schedule } from "../../store/iptvApi";
import { GetSchedules } from "../../store/signlar_functions";
import DataSelector from "../dataSelector/DataSelector";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";


const SchedulesDirectSchedulesDataSelector = (props: SchedulesDirectSchedulesDataSelectorProps) => {
  const toast = useRef<Toast>(null);

  const [dataSource, setDataSource] = useState<Schedule[]>([] as Schedule[]);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  useEffect(() => {
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

  const sourceColumns = useMemo((): ColumnMeta[] => {
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

          headerName="Schedules"
          id='SchedulesDirectSchedulesDataSelector-ds'
          isLoading={isLoading}
          key="callsign"
          selectionMode='multiple'
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
  readonly stationIds: string[];
};
export default memo(SchedulesDirectSchedulesDataSelector);
