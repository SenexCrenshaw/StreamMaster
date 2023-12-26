import { HeadendDto, useSchedulesDirectGetLineupsQuery } from '@lib/iptvApi';
import { memo, useCallback, useMemo } from 'react';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

import DataSelector from '../dataSelector/DataSelector';
import SchedulesDirectRemoveHeadendDialog from './SchedulesDirectRemoveHeadendDialog';
interface SchedulesDirectLineUpsDataSelectorProperties {
  id: string;
}
const SchedulesDirectLineUpsDataSelector = ({ id }: SchedulesDirectLineUpsDataSelectorProperties) => {
  const getLineUpsQuery = useSchedulesDirectGetLineupsQuery();

  const actionBodyTemplate = useCallback((data: HeadendDto) => {
    return (
      <div className="flex p-0 justify-content-center align-items-center">
        <SchedulesDirectRemoveHeadendDialog value={data} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'lineup', sortable: true },
      { field: 'location', sortable: true },
      { field: 'name', sortable: true },
      { field: 'transport', sortable: true, width: '6rem' },
      {
        align: 'center',
        bodyTemplate: actionBodyTemplate,
        field: 'Remove',
        header: '',
        resizeable: false,
        sortable: false,
        width: '3rem'
      }
    ],
    [actionBodyTemplate]
  );

  return (
    <div className="flex flex-column">
      <DataSelector
        columns={columns}
        defaultSortField="name"
        dataSource={getLineUpsQuery.data}
        emptyMessage="No Streams"
        id={id}
        isLoading={getLineUpsQuery.isLoading}
        selectionMode="single"
        selectedItemsKey="sdEditorSelectSelectedItems"
        style={{ height: 'calc(100vh - 120px)' }}
      />
      {/* <SchedulesDirectLineUpPreviewDataSelector lineUps={getLineUpsQuery.data?.lineups} /> */}
    </div>
  );
};

export default memo(SchedulesDirectLineUpsDataSelector);
