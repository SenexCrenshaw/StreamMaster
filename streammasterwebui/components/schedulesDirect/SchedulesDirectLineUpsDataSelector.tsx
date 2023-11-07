import { useSchedulesDirectGetLineupsQuery } from '@lib/iptvApi';
import { memo, useMemo } from 'react';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

import DataSelector from '../dataSelector/DataSelector';
interface SchedulesDirectLineUpsDataSelectorProperties {
  id: string;
}
const SchedulesDirectLineUpsDataSelector = ({ id }: SchedulesDirectLineUpsDataSelectorProperties) => {
  const getLineUpsQuery = useSchedulesDirectGetLineupsQuery();

  const sourceColumns = useMemo((): ColumnMeta[] => [{ field: 'lineup' }, { field: 'location' }, { field: 'name' }, { field: 'transport' }], []);
  // console.log(getLineUpsQuery.data);
  return (
    <div className="m3uFilesEditor flex flex-column border-2 border-round surface-border">
      {/* <h3><span className='text-bold'>LineUps | </span><span className='text-bold text-blue-500'>{props.country}</span> - <span className='text-bold text-500'>{props.postalCode}</span></h3> */}
      <DataSelector
        columns={sourceColumns}
        defaultSortField="name"
        dataSource={getLineUpsQuery.data}
        emptyMessage="No Streams"
        id={id}
        isLoading={getLineUpsQuery.isLoading}
        selectionMode="single"
        selectedItemsKey="sdEditorSelectSelectedItems"
        style={{ height: 'calc(50vh - 40px)' }}
      />
      {/* <SchedulesDirectLineUpPreviewDataSelector lineUps={getLineUpsQuery.data?.lineups} /> */}
    </div>
  );
};

export default memo(SchedulesDirectLineUpsDataSelector);
