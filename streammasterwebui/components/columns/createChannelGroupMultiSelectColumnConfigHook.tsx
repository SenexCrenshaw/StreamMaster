import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { ColumnFieldType } from '@components/smDataTable/types/smDataTableTypes';
import { QueryHook } from '@lib/apiDefs';
import { isEmptyObject } from '@lib/common/common';

import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { type ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, type MultiSelectChangeEvent } from 'primereact/multiselect';

type DataField = keyof ChannelGroupDto;
type EditorComponent = React.ComponentType<{ data: ChannelGroupDto }>;

interface ColumnConfigInputs {
  EditorComponent?: EditorComponent;
  dataField: DataField;
  fieldType?: ColumnFieldType;
  headerTitle: string;
  maxWidth?: number;
  minWidth?: number;
  queryHook?: QueryHook<string[]>;
  sortable?: boolean;
  useFilter?: boolean;
  width?: number;
}

const createChannelGroupMultiSelectColumnConfigHook =
  ({
    dataField,
    fieldType,
    headerTitle,
    maxWidth,
    minWidth,
    width,
    EditorComponent,
    queryHook,
    sortable = false,
    useFilter: configUseFilter
  }: ColumnConfigInputs) =>
  ({ enableEdit = false, useFilter = configUseFilter, values }: { enableEdit?: boolean; useFilter?: boolean; values?: string[] | undefined }) => {
    const { data, isLoading, isFetching, isError } = queryHook ? queryHook() : { data: undefined, isError: false, isFetching: false, isLoading: false };

    const bodyTemplate = (bodyData: ChannelGroupDto | string) => {
      if (typeof bodyData === 'string') {
        return <span>{bodyData}</span>;
      }

      const value = bodyData[dataField];

      if (value === undefined) {
        return <span />;
      }

      if (!enableEdit) {
        return <span>{value.toString()}</span>;
      }

      if (EditorComponent) {
        return <EditorComponent data={bodyData} />;
      }

      return <span>{value.toString()}</span>;
    };

    const itemTemplate = (option: string) => (
      <div className="align-items-center gap-1">
        <span>{option}</span>
      </div>
    );

    const filterTemplate = (options: ColumnFilterElementTemplateOptions) => (
      <MultiSelect
        className="p-column-filter text-xs border-1"
        filter
        itemTemplate={itemTemplate}
        maxSelectedLabels={1}
        onChange={(e: MultiSelectChangeEvent) => {
          if (isEmptyObject(e.value)) {
            options.filterApplyCallback();
          } else {
            options.filterApplyCallback(e.value);
          }
        }}
        options={values && values.length > 0 ? values : data}
        placeholder="Any"
        value={options.value}
      />
    );

    const columnConfig: ColumnMeta = {
      align: 'left',
      bodyTemplate,
      field: dataField as string,
      fieldType,
      filter: useFilter === undefined ? true : useFilter,
      filterField: dataField as string,
      header: headerTitle,
      maxWidth: maxWidth === undefined ? undefined : `${maxWidth}rem`,
      minWidth: minWidth === undefined ? undefined : `${minWidth}rem`,
      sortable: sortable,
      width: width === undefined ? undefined : `${width}rem`
    };

    if (queryHook !== undefined) {
      columnConfig.filterElement = filterTemplate;
    }

    return {
      columnConfig,
      isError,
      isFetching,
      isLoading
    };
  };

export default createChannelGroupMultiSelectColumnConfigHook;
