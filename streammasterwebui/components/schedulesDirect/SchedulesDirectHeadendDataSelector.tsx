import { memo, useCallback, useMemo } from 'react';

import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useSelectedCountry } from '@lib/redux/hooks/selectedCountry';
import { useSelectedPostalCode } from '@lib/redux/hooks/selectedPostalCode';
import useGetHeadends from '@lib/smAPI/SchedulesDirect/useGetHeadends';
import { GetHeadendsRequest, HeadendDto } from '@lib/smAPI/smapiTypes';
import SchedulesDirectAddHeadendDialog from './SchedulesDirectAddHeadendDialog';
import SchedulesDirectCountrySelector from './SchedulesDirectCountrySelector';
import SchedulesDirectLineupPreviewChannel from './SchedulesDirectLineupPreviewChannel';

const SchedulesDirectHeadendDataSelector = () => {
  const dataKey = 'SchedulesDirectHeadendDataSelector';
  const { selectedCountry } = useSelectedCountry('Country');
  const { selectedPostalCode } = useSelectedPostalCode('PostalCode');
  const { data } = useGetHeadends({ country: selectedCountry ?? 'USA', postalCode: selectedPostalCode ?? '00000' } as GetHeadendsRequest);

  const actionBodyTemplate = useCallback((headEndDto: HeadendDto) => {
    return (
      <div className="flex p-0 justify-content-center align-items-center">
        <SchedulesDirectLineupPreviewChannel lineup={headEndDto.Lineup} />
        <SchedulesDirectAddHeadendDialog value={headEndDto} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'HeadendId', filter: true, sortable: true, width: 80 },
      { field: 'Lineup', filter: true, sortable: true, width: 80 },
      { field: 'Location', filter: true, sortable: true, width: 80 },
      { field: 'Name', filter: true, sortable: true, width: 100 },
      { field: 'Transport', sortable: true, width: 60 },
      {
        bodyTemplate: actionBodyTemplate,
        field: 'actions',
        fieldType: 'actions',
        header: '',
        width: 24
      }
    ],
    [actionBodyTemplate]
  );

  const centerTemplate = useMemo(() => <SchedulesDirectCountrySelector />, []);

  return (
    <SMDataTable
      columns={columns}
      dataSource={data}
      defaultSortField="HeadendId"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      enablePaginator
      headerCenterTemplate={centerTemplate}
      headerName="LINEUPS"
      id={dataKey}
      selectRow
      selectedItemsKey="sdselectedItems"
      style={{ height: 'calc(100vh - 100px)' }}
    />
  );
};

SchedulesDirectHeadendDataSelector.displayName = 'SchedulesDirectHeadendDataSelector';

export default memo(SchedulesDirectHeadendDataSelector);
