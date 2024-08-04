import SMDropDown from '@components/sm/SMDropDown';
import { arraysEqual } from '@components/smDataTable/helpers/arraysEqual';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { useFilters } from '@lib/redux/hooks/filters';

import useGetM3UFiles from '@lib/smAPI/M3UFiles/useGetM3UFiles';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { DataTableFilterMetaData } from 'primereact/datatable';
import { ReactNode, useCallback, useMemo } from 'react';

interface SMStreamM3UColumnConfigProperties {
  readonly className?: string;
  readonly dataKey: string;
  readonly width?: string;
}

export const useSMStreamM3UColumnConfig = ({ className = 'sm-w-7rem', dataKey }: SMStreamM3UColumnConfigProperties) => {
  const { data } = useGetM3UFiles();

  const { filters, setFilters } = useFilters(dataKey);
  const { selectedItems } = useSelectedAndQ('useSMStreamM3UColumnConfig');

  const updateFilters = () => {
    if (selectedItems.length === 0) {
      const newFilter = { ...filters };
      const a = newFilter['M3UFileId'] as DataTableFilterMetaData;
      if (a && a.value !== undefined) {
        newFilter['M3UFileId'] = { matchMode: 'contains', value: undefined };
        setFilters(newFilter);
      }
    } else {
      const names = selectedItems.map((x) => x.Id);
      const newFilter = { ...filters };
      const a = newFilter['M3UFileId'] as DataTableFilterMetaData;
      if (!filters['M3UFileId'] || !arraysEqual(a.value, names)) {
        newFilter['M3UFileId'] = { matchMode: 'contains', value: names };
        setFilters(newFilter);
      }
    }
  };

  updateFilters();

  const itemTemplate = useCallback((option: M3UFileDto) => {
    if (option?.Name === undefined) {
      return null;
    }

    return <div className="text-container">{option.Name}</div>;
  }, []);

  const buttonTemplate = useCallback(
    (options: any): ReactNode => {
      if (Array.isArray(selectedItems)) {
        if (selectedItems.length > 0) {
          const names = selectedItems.map((x: any) => (x.Id === -1 ? '*' : x.Name));
          const sortedInput = [...names].sort();
          return <div className="text-container">{sortedInput.join(', ')}</div>;
        }
      }

      return <div className="text-container pl-1">M3U</div>;
    },
    [selectedItems]
  );

  const dataSource = useMemo(() => {
    if (data) {
      return [{ Id: -1, Name: '* Custom Only' }, ...data];
    }
    return [{ Id: -1, Name: '* Custom Only' }];
  }, [data]);

  function filterTemplate(options: ColumnFilterElementTemplateOptions): ReactNode {
    return (
      <div className={className}>
        <SMDropDown
          buttonDarkBackground
          buttonTemplate={buttonTemplate(options)}
          data={dataSource}
          dataKey="Id"
          itemTemplate={itemTemplate}
          filter
          info=""
          filterBy="Name"
          select
          selectedItemsKey="useSMStreamM3UColumnConfig"
          title="M3U"
          contentWidthSize="2"
        />
      </div>
    );
  }

  const columnConfig: ColumnMeta = {
    align: 'right',
    field: 'M3UFileName',
    filter: true,
    filterElement: filterTemplate,
    header: 'M3UFileName',
    sortable: true,
    width: 125
  };

  return columnConfig;
};
