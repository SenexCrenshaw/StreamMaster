import BooleanEditor from '@components/inputs/BooleanEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { useSelectedCountry } from '@lib/redux/hooks/selectedCountry';
import { useSelectedPostalCode } from '@lib/redux/hooks/selectedPostalCode';
import { useSMContext } from '@lib/signalr/SMProvider';
import { AddHeadendToView, RemoveHeadendToView } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import useGetHeadendsByCountryPostal from '@lib/smAPI/SchedulesDirect/useGetHeadendsByCountryPostal';
import useGetHeadendsToView from '@lib/smAPI/SchedulesDirect/useGetHeadendsToView';
import useGetSubscribedLineups from '@lib/smAPI/SchedulesDirect/useGetSubscribedLineups';
import { AddHeadendToViewRequest, GetHeadendsByCountryPostalRequest, HeadendDto, RemoveHeadendToViewRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';
import SchedulesDirectAddHeadendDialog from './SchedulesDirectAddHeadendDialog';
import SchedulesDirectCountrySelector from './SchedulesDirectCountrySelector';
import SchedulesDirectLineupPreviewChannel from './SchedulesDirectLineupPreviewChannel';
import SchedulesDirectRemoveHeadendDialog from './SchedulesDirectRemoveHeadendDialog';

const SchedulesDirectHeadendDataSelector = () => {
  const dataKey = 'SchedulesDirectHeadendDataSelector';
  const { selectedCountry } = useSelectedCountry('Country');
  const { selectedPostalCode } = useSelectedPostalCode('PostalCode');
  const { data } = useGetHeadendsByCountryPostal({
    Country: selectedCountry ?? 'USA',
    PostalCode: selectedPostalCode ?? '00000'
  } as GetHeadendsByCountryPostalRequest);
  const { data: subscribedLineups } = useGetSubscribedLineups();
  const { data: headendsToView } = useGetHeadendsToView();
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

  const checkBoxChanged = useCallback(
    (headEndDto: HeadendDto, isChecked: boolean) => {
      Logger.debug('checkBoxChanged', headEndDto, isChecked);
      if (isChecked === true) {
        AddHeadendToView({ Country: selectedCountry, HeadendId: headEndDto.HeadendId, Postal: selectedPostalCode } as AddHeadendToViewRequest);
      } else {
        RemoveHeadendToView({ Country: selectedCountry, HeadendId: headEndDto.HeadendId, Postal: selectedPostalCode } as RemoveHeadendToViewRequest);
      }
    },
    [selectedCountry, selectedPostalCode]
  );

  const isViewedTemplate = useCallback(
    (rowData: HeadendDto) => {
      const found = headendsToView?.some((item) => item.Id === rowData.HeadendId && item.Lineup === rowData.Lineup) ?? false;

      return (
        <BooleanEditor
          checked={found}
          onChange={(e) => {
            checkBoxChanged(rowData, e);
          }}
        />
      );
    },
    [headendsToView]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { bodyTemplate: isViewedTemplate, field: 'blah', header: 'In Lineups', width: 24 },
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

  const centerTemplate = useMemo(() => <SchedulesDirectCountrySelector />, []);

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
