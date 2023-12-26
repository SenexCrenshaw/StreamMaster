import { HeadendDto, SchedulesDirectGetHeadendsApiArg } from '@lib/iptvApi';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

import { useSelectedCountry } from '@lib/redux/slices/selectedCountrySlice';
import { useSelectedPostalCode } from '@lib/redux/slices/selectedPostalCodeSlice';
import { GetHeadends } from '@lib/smAPI/SchedulesDirect/SchedulesDirectGetAPI';
import DataSelector from '../dataSelector/DataSelector';
import SchedulesDirectAddHeadendDialog from './SchedulesDirectAddHeadendDialog';
import SchedulesDirectCountrySelector from './SchedulesDirectCountrySelector';
import SchedulesDirectLineupPreviewChannel from './SchedulesDirectLineupPreviewChannel';

const SchedulesDirectHeadendDataSelector = () => {
  const { selectedCountry } = useSelectedCountry('Country');
  const { selectedPostalCode } = useSelectedPostalCode('PostalCode');
  const [dataSource, setDataSource] = useState<HeadendDto[]>([]);

  const [lineupToPreview, setLineupToPreview] = useState<string | undefined>(undefined);

  // const getHeadendsQuery = useSchedulesDirectGetHeadendsQuery(
  //   ({ country: selectedCountry ?? 'USA', postalCode: selectedPostalCode ?? '0000' } as SchedulesDirectGetHeadendsApiArg) ?? skipToken
  // );
  useEffect(() => {
    GetHeadends({ country: selectedCountry ?? 'USA', postalCode: selectedPostalCode ?? '00000' } as SchedulesDirectGetHeadendsApiArg)
      .then((data) => {
        setDataSource(data ?? []);
      })
      .catch((error) => {
        setDataSource([]);
      });
  }, [selectedCountry, selectedPostalCode]);

  const actionBodyTemplate = useCallback((data: HeadendDto) => {
    return (
      <div className="flex p-0 justify-content-end align-items-center">
        <SchedulesDirectAddHeadendDialog value={data} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'headendId', filter: true, sortable: true, width: '6rem' },
      { field: 'lineup', filter: true, sortable: true },
      { field: 'location', filter: true, sortable: true },
      { field: 'name', filter: true, sortable: true },
      { field: 'transport', sortable: true, width: '6rem' },
      {
        bodyTemplate: actionBodyTemplate,
        field: 'Add',
        header: '',
        resizeable: false,
        sortable: false,
        width: '3rem'
      }
    ],
    [actionBodyTemplate]
  );

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <SchedulesDirectCountrySelector />
      </div>
    ),
    []
  );

  return (
    <div className="m3uFilesEditor flex flex-column border-2 border-round surface-border">
      <h3>
        <span className="text-bold">TV Headends | </span>
        <span className="text-bold text-blue-500">{selectedCountry}</span> -<span className="text-bold text-500">{selectedPostalCode}</span>
      </h3>

      <SchedulesDirectLineupPreviewChannel lineup={lineupToPreview} onHide={() => setLineupToPreview(undefined)} />
      <DataSelector
        columns={columns}
        defaultSortField="name"
        dataSource={dataSource}
        emptyMessage="No Streams"
        id="StreamingServerStatusPanel"
        headerRightTemplate={rightHeaderTemplate}
        onRowClick={(e) => {
          const data: HeadendDto = e.data;
          setLineupToPreview(data.lineup);
        }}
        selectedItemsKey="selectSelectedItems"
        style={{ height: 'calc(100vh - 120px)' }}
      />
    </div>
  );
};

SchedulesDirectHeadendDataSelector.displayName = 'SchedulesDirectHeadendDataSelector';

export default memo(SchedulesDirectHeadendDataSelector);
