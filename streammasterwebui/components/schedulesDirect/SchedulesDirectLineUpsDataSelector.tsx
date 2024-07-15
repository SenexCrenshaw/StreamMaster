import { memo, useCallback, useMemo } from 'react';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import useGetSubscribedLineups from '@lib/smAPI/SchedulesDirect/useGetSubscribedLineups';
import { HeadendDto } from '@lib/smAPI/smapiTypes';
import SchedulesDirectRemoveHeadendDialog from './SchedulesDirectRemoveHeadendDialog';
import { Logger } from '@lib/common/logger';
interface SchedulesDirectLineUpsDataSelectorProperties {
  id: string;
}
const SchedulesDirectLineUpsDataSelector = ({ id }: SchedulesDirectLineUpsDataSelectorProperties) => {
  const { data, isLoading } = useGetSubscribedLineups();

  const actionBodyTemplate = useCallback((data: HeadendDto) => {
    return (
      <div className="flex p-0 justify-content-center align-items-center">
        <SchedulesDirectRemoveHeadendDialog value={data} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Lineup', sortable: true, width: 80 },
      { field: 'Location', sortable: true, width: 80 },
      { field: 'Name', sortable: true, width: 100 },
      { field: 'Transport', sortable: true, width: 80 },
      {
        align: 'center',
        bodyTemplate: actionBodyTemplate,
        field: 'Remove',
        fieldType: 'actions',
        header: '',
        resizeable: false,
        sortable: false,
        width: 20
      }
    ],
    [actionBodyTemplate]
  );

  Logger.debug('SchedulesDirectLineUpsDataSelector', data);

  return (
    <SMDataTable
      arrayKey="Lineup"
      columns={columns}
      defaultSortField="Lineup"
      defaultSortOrder={-1}
      dataSource={data}
      emptyMessage="No Streams"
      enablePaginator
      headerName="Subscribed Line Ups"
      id={id}
      isLoading={isLoading}
      lazy
      style={{ height: 'calc(25vh)' }}
    />
  );
};

export default memo(SchedulesDirectLineUpsDataSelector);
