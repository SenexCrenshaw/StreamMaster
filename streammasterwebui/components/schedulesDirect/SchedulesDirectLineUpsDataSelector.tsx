import { memo, useCallback, useMemo } from 'react';

import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import useGetLineups from '@lib/smAPI/SchedulesDirect/useGetLineups';
import { HeadendDto } from '@lib/smAPI/smapiTypes';
import SchedulesDirectRemoveHeadendDialog from './SchedulesDirectRemoveHeadendDialog';
interface SchedulesDirectLineUpsDataSelectorProperties {
  id: string;
}
const SchedulesDirectLineUpsDataSelector = ({ id }: SchedulesDirectLineUpsDataSelectorProperties) => {
  const { data, isLoading } = useGetLineups();

  const actionBodyTemplate = useCallback((data: HeadendDto) => {
    return (
      <div className="flex p-0 justify-content-center align-items-center">
        <SchedulesDirectRemoveHeadendDialog value={data} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Lineup', sortable: true },
      { field: 'Location', sortable: true },
      { field: 'Name', sortable: true },
      { field: 'Transport', sortable: true, width: '6rem' },
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
      <SMDataTable
        columns={columns}
        defaultSortField="name"
        dataSource={data}
        emptyMessage="No Streams"
        id={id}
        isLoading={isLoading}
        selectionMode="single"
        selectedItemsKey="sdEditorselectedItems"
        style={{ height: 'calc(100vh - 120px)' }}
      />
      {/* <SchedulesDirectLineUpPreviewDataSelector lineUps={getLineUpsQuery.data?.lineups} /> */}
    </div>
  );
};

export default memo(SchedulesDirectLineUpsDataSelector);
