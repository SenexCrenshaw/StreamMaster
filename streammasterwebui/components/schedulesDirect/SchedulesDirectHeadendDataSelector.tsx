import { HeadendDto, SchedulesDirectGetHeadendsApiArg, useSchedulesDirectGetHeadendsQuery } from '@lib/iptvApi';
import { memo, useCallback, useMemo } from 'react';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

import { useSelectedCountry } from '@lib/redux/slices/selectedCountrySlice';
import { useSelectedPostalCode } from '@lib/redux/slices/selectedPostalCodeSlice';
import { skipToken } from '@reduxjs/toolkit/query';
import DataSelector from '../dataSelector/DataSelector';
import SchedulesDirectAddHeadendDialog from './SchedulesDirectAddHeadendDialog';
import SchedulesDirectCountrySelector from './SchedulesDirectCountrySelector';

const SchedulesDirectHeadendDataSelector = () => {
  const { selectedCountry } = useSelectedCountry('Country');
  const { selectedPostalCode } = useSelectedPostalCode('ZipCode');

  const getHeadendsQuery = useSchedulesDirectGetHeadendsQuery(
    ({ country: selectedCountry ?? 'USA', postalCode: selectedPostalCode ?? '0000' } as SchedulesDirectGetHeadendsApiArg) ?? skipToken
  );

  const actionBodyTemplate = useCallback((data: HeadendDto) => {
    return (
      <div className="flex p-0 justify-content-end align-items-center">
        <SchedulesDirectAddHeadendDialog value={data} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'headendId', sortable: true },
      { field: 'lineup', sortable: true },
      { field: 'location', sortable: true },
      { field: 'name', sortable: true },
      { field: 'transport', sortable: true },
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
      <DataSelector
        columns={columns}
        defaultSortField="name"
        dataSource={getHeadendsQuery.data}
        emptyMessage="No Streams"
        id="StreamingServerStatusPanel"
        isLoading={getHeadendsQuery.isLoading}
        headerRightTemplate={rightHeaderTemplate}
        selectedItemsKey="selectSelectedItems"
        style={{ height: 'calc(100vh - 120px)' }}
      />
    </div>
  );
};

SchedulesDirectHeadendDataSelector.displayName = 'SchedulesDirectHeadendDataSelector';

export default memo(SchedulesDirectHeadendDataSelector);
