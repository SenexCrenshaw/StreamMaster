import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { isEmptyObject } from '@lib/common/common';
import useGetSubscribedLineup from '@lib/smAPI/SchedulesDirect/useGetSubscribedLineup';

import { StationPreview, SubscribedLineup } from '@lib/smAPI/smapiTypes';
import { type ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, type MultiSelectChangeEvent } from 'primereact/multiselect';

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
    const { data, isLoading, isFetching, isError } = useGetSubscribedLineup();

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

    const filterTemplate = (options: ColumnFilterElementTemplateOptions) => (
      <>
        <MultiSelect
          className="p-column-filter text-xs"
          filter
          itemTemplate={itemTemplate}
          maxSelectedLabels={1}
          onChange={(e: MultiSelectChangeEvent) => {
            if (isEmptyObject(e.value)) {
              options.filterApplyCallback();
            } else {
              options.filterApplyCallback([e.value[e.value.length - 1]]);
            }
          }}
          options={data}
          optionLabel="lineup"
          optionValue="lineup"
          placeholder="LineUp"
          value={options.value}
        />
      </>
    );

    const columnConfig: ColumnMeta = {
      align: 'left',
      bodyTemplate,
      field: 'lineup',
      filter: true,
      filterField: 'lineup',
      header: headerTitle,
      sortable: true,
      width: width
      // style: {
      //   maxWidth: maxWidth === undefined ? (width === undefined ? undefined : `${width}rem`) : `${maxWidth}rem`,
      //   minWidth: minWidth === undefined ? (width === undefined ? undefined : `${width}rem`) : `${minWidth}rem`,
      //   width: width === undefined ? (minWidth === undefined ? undefined : `${minWidth}rem`) : `${width}rem`
      // }
      // {...props}
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
