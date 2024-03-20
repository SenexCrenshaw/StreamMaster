import { isEmptyObject } from '@lib/common/common';
import { StationPreview, useSchedulesDirectGetLineupsQuery } from '@lib/iptvApi';
import { type ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, type MultiSelectChangeEvent } from 'primereact/multiselect';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';

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
    const { data, isLoading, isFetching, isError } = useSchedulesDirectGetLineupsQuery();

    const bodyTemplate = (option: StationPreview) => {
      return <span>{option.lineup}</span>;
    };

    const itemTemplate = (option: Lineup) => (
      <div className="align-items-center gap-2">
        <span>
          {option.lineup} - {option.name}
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
          placeholder="All"
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

      style: {
        maxWidth: maxWidth === undefined ? (width === undefined ? undefined : `${width}rem`) : `${maxWidth}rem`,
        minWidth: minWidth === undefined ? (width === undefined ? undefined : `${width}rem`) : `${minWidth}rem`,
        width: width === undefined ? (minWidth === undefined ? undefined : `${minWidth}rem`) : `${width}rem`
      }
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
