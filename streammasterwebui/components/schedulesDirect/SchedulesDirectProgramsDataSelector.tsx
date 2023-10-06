import { useSchedulesDirectGetProgramsQuery } from '@/lib/iptvApi';
import { Toast } from 'primereact/toast';
import { memo, useMemo, useRef } from 'react';

import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

type SchedulesDirectProgramsDataSelectorProps = {
  readonly id: string;
};

const SchedulesDirectProgramsDataSelector = ({ id }: SchedulesDirectProgramsDataSelectorProps) => {
  const toast = useRef<Toast>(null);

  const schedulesDirectGetProgramsQuery = useSchedulesDirectGetProgramsQuery();

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [{ field: 'resourceID' }, { field: 'programID' }];
  }, []);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="m3uFilesEditor flex flex-column border-2 border-round surface-border">
        <DataSelector
          columns={sourceColumns}
          dataSource={schedulesDirectGetProgramsQuery.data}
          emptyMessage="No Line Ups"
          headerName="Schedules"
          id={id}
          isLoading={schedulesDirectGetProgramsQuery.isLoading}
          key="callsign"
          selectedItemsKey="sdEditorSelectSelectedItems"
          selectionMode="multiple"
          style={{ height: 'calc(50vh - 40px)' }}
        />
      </div>
    </>
  );
};

export default memo(SchedulesDirectProgramsDataSelector);
