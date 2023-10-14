import { useSchedulesDirectGetSchedulesQuery } from '@lib/iptvApi';
import { Toast } from 'primereact/toast';
import { memo, useMemo, useRef } from 'react';

import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

type SchedulesDirectSchedulesDataSelectorProps = {
  readonly id: string;
};

const SchedulesDirectSchedulesDataSelector = ({ id }: SchedulesDirectSchedulesDataSelectorProps) => {
  const toast = useRef<Toast>(null);

  // const [dataSource, setDataSource] = useState<Schedule[]>([] as Schedule[]);
  // const [isLoading, setIsLoading] = useState<boolean>(false);
  // const { selectSelectedItems } = useSelectedItems<Lineup>('SchedulesDirectSchedulesDataSelector');

  const schedulesDirectGetSchedulesQuery = useSchedulesDirectGetSchedulesQuery();

  // const schedulesDirectGetStationsQuery = useSchedulesDirectGetStationsQuery()

  // useEffect(() => {
  //   if (selectSelectedItems.length === 0) {
  //     return;
  //   }

  //   console.log('SchedulesDirectSchedulesDataSelector', selectSelectedItems);

  //   setIsLoading(true);

  //   GetSchedules()
  //     .then((data) => {
  //       if (data) {
  //         setDataSource(data);
  //       }
  //       setIsLoading(false);
  //     })
  //     .catch(() => {
  //       setIsLoading(false);

  //       if (toast.current) {
  //         toast.current.show({
  //           detail: `Get Schedules Failed`,
  //           life: 3000,
  //           severity: 'error',
  //           summary: 'Failed',
  //         });
  //       }
  //     });
  // }, [selectSelectedItems]);

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'stationID', header: 'Station Id' },
      { field: 'metadata.startDate', header: 'metadata Id' },
    ];
  }, []);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="m3uFilesEditor flex flex-column border-2 border-round surface-border">
        <DataSelector
          columns={sourceColumns}
          defaultSortField="stationID"
          dataSource={schedulesDirectGetSchedulesQuery.data}
          emptyMessage="No Line Ups"
          headerName="Schedules"
          id={id}
          isLoading={schedulesDirectGetSchedulesQuery.isLoading}
          key="callsign"
          selectedItemsKey="sdEditorSelectSelectedItems"
          selectionMode="multiple"
          style={{ height: 'calc(50vh - 40px)' }}
        />
      </div>
    </>
  );
};

export default memo(SchedulesDirectSchedulesDataSelector);
