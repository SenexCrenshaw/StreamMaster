import { QueryHook } from '@lib/apiDefs';
import { isEmptyObject } from '@lib/common/common';

import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { type ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, type MultiSelectChangeEvent } from 'primereact/multiselect';
import { type ColumnFieldType, type ColumnMeta } from '../dataSelector/DataSelectorTypes';

type DataField = keyof SMChannelDto;
type EditorComponent = React.ComponentType<{ data: SMChannelDto }>;

interface ColumnConfigInputs {
  EditorComponent?: EditorComponent;
  dataField: DataField;
  fieldType?: ColumnFieldType;
  headerTitle: string;
  maxWidth?: number;
  minWidth?: number;
  queryHook?: QueryHook<ChannelGroupIdName[] | string[]>;
  useFilter?: boolean;
  width?: number;
}

const createSMChannelMultiSelectColumnConfigHook =
  ({ dataField, fieldType, headerTitle, maxWidth, minWidth, width, EditorComponent, queryHook, useFilter }: ColumnConfigInputs) =>
  ({ enableEdit = false, values }: { enableEdit?: boolean; values?: string[] | undefined }) => {
    const { data, isLoading, isFetching, isError } = queryHook ? queryHook() : { data: undefined, isError: false, isFetching: false, isLoading: false };

    const bodyTemplate = (bodyData: SMChannelDto | string) => {
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
      <div className="align-items-center gap-2">
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

    if (dataField === undefined) {
      console.error('dataField is undefined');
    }

    console.log('dataField', dataField, 'useFilter', useFilter);
    const columnConfig: ColumnMeta = {
      align: 'left',
      bodyTemplate,
      field: dataField as string,
      fieldType,
      filter: useFilter === undefined ? true : useFilter,
      filterField: dataField as string,
      header: headerTitle,
      sortable: true,

      style: {
        maxWidth: maxWidth === undefined ? (width === undefined ? undefined : `${width}rem`) : `${maxWidth}rem`,
        minWidth: minWidth === undefined ? (width === undefined ? undefined : `${width}rem`) : `${minWidth}rem`,
        width: width === undefined ? (minWidth === undefined ? undefined : `${minWidth}rem`) : `${width}rem`
      }
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

export default createSMChannelMultiSelectColumnConfigHook;
