import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import { Logger } from '@lib/common/logger';
import useGetSubscribedLineups from '@lib/smAPI/SchedulesDirect/useGetSubscribedLineups';
import { StationPreview, SubscribedLineup } from '@lib/smAPI/smapiTypes';
import { type ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, type MultiSelectChangeEvent } from 'primereact/multiselect';
import { useCallback, useState } from 'react';

interface ColumnConfigInputs {
  headerTitle: string;
  maxWidth?: number;
  minWidth?: number;
  useFilter?: boolean;
  width?: number;
}

const createMultiSelectLineUpColumnConfigHook =
  ({ headerTitle, maxWidth, minWidth, width }: ColumnConfigInputs) =>
  () => {
    const { data, isLoading, isFetching, isError } = useGetSubscribedLineups();
    const [selectedValues, setSelectedValues] = useState<SubscribedLineup[]>([]);

    const bodyTemplate = (option: StationPreview) => {
      return <span>{option.Lineup}</span>;
    };

    const itemTemplate = (option: SubscribedLineup) => (
      <div className="align-items-center gap-1">
        <span>
          {option.Lineup} - {option.Name}
        </span>
      </div>
    );

    const filterTemplate = useCallback(
      (options: ColumnFilterElementTemplateOptions) => {
        return (
          <MultiSelect
            filter
            filterBy="Lineup"
            itemTemplate={itemTemplate}
            maxSelectedLabels={1}
            onChange={(e: MultiSelectChangeEvent) => {
              //  const b = e.value[e.value.length - 1];
              // options.filterApplyCallback([e.value.length - 1]);
              if (isEmptyObject(e.value)) {
                options.filterApplyCallback();
              } else {
                // const lineups = e.value.map((v: SubscribedLineup) => v.Lineup);
                Logger.debug('options', e.value, options, data);
                options.filterApplyCallback(e.value);
              }
            }}
            options={data}
            optionLabel="Lineup"
            optionValue="Lineup"
            placeholder="Lineup"
            value={options.value}
          />
        );
      },
      [data]
    );

    const columnConfig: ColumnMeta = {
      align: 'left',
      bodyTemplate,
      field: 'Lineup',
      filter: true,
      filterField: 'Lineup',
      header: headerTitle,
      sortable: true,
      width: width
    };

    columnConfig.filterElement = filterTemplate;

    return {
      columnConfig,
      isError,
      isFetching,
      isLoading
    };
  };

export default createMultiSelectLineUpColumnConfigHook;
