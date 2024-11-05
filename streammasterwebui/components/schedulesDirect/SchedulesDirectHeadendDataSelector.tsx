import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useSMContext } from '@lib/context/SMProvider';
import useGetHeadendsByCountryPostal from '@lib/smAPI/SchedulesDirect/useGetHeadendsByCountryPostal';
import useGetSubscribedLineups from '@lib/smAPI/SchedulesDirect/useGetSubscribedLineups';
import { GetHeadendsByCountryPostalRequest, HeadendDto } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo, useState } from 'react';
import SchedulesDirectAddHeadendDialog from './SchedulesDirectAddHeadendDialog';
import SchedulesDirectCountrySelector from './SchedulesDirectCountrySelector';
import SchedulesDirectLineupPreviewChannel from './SchedulesDirectLineupPreviewChannel';
import SchedulesDirectRemoveHeadendDialog from './SchedulesDirectRemoveHeadendDialog';

const SchedulesDirectHeadendDataSelector = () => {
  const dataKey = 'SchedulesDirectHeadendDataSelector';
  const [country, setCountry] = useState<string | null>(null);
  const [postalCode, setPostalCode] = useState<string | null>(null);

  const { data } = useGetHeadendsByCountryPostal({
    Country: country || 'USA',
    PostalCode: postalCode || '00000'
  } as GetHeadendsByCountryPostalRequest);

  const { data: subscribedLineups } = useGetSubscribedLineups();
  const { settings } = useSMContext();

  const actionBodyTemplate = useCallback(
    (headEndDto: HeadendDto) => {
      let found = subscribedLineups?.some((item) => item.Lineup === headEndDto.Lineup) ?? false;

      return (
        <div className="flex p-0 justify-content-center align-items-center">
          <SchedulesDirectLineupPreviewChannel lineup={headEndDto.Lineup} />
          {found ? (
            <SchedulesDirectRemoveHeadendDialog value={headEndDto} />
          ) : (
            <SchedulesDirectAddHeadendDialog
              buttonDisabled={(subscribedLineups !== undefined && subscribedLineups.length >= settings?.SDSettings?.MaxSubscribedLineups) ?? 4}
              value={headEndDto}
            />
          )}
        </div>
      );
    },
    [settings.SDSettings, subscribedLineups]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      // { bodyTemplate: isViewedTemplate, field: 'blah', header: 'In Lineups', width: 24 },
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
        width: 28
      }
    ],
    [actionBodyTemplate]
  );

  const centerTemplate = useMemo(
    () => (
      <SchedulesDirectCountrySelector
        onChange={(e) => {
          setCountry(e.Country);
          setPostalCode(e.PostalCode);
        }}
      />
    ),
    []
  );

  const rowClass = useCallback(
    (headEndDto: any): string => {
      let found = subscribedLineups?.some((item) => item.Lineup === headEndDto.Lineup) ?? false;

      if (found) {
        return 'channel-row-selected';
      }

      return '';
    },
    [subscribedLineups]
  );

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
      lazy
      selectRow
      rowClass={rowClass}
      style={{ height: 'calc(55vh)' }}
    />
  );
};

SchedulesDirectHeadendDataSelector.displayName = 'SchedulesDirectHeadendDataSelector';

export default memo(SchedulesDirectHeadendDataSelector);
