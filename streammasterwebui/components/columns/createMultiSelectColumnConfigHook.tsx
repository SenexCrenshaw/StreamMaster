import { isEmptyObject, type QueryHook } from '@lib/common/common';
import { type ChannelGroupIdName, type VideoStreamDto } from '@lib/iptvApi';
import { type ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, type MultiSelectChangeEvent } from 'primereact/multiselect';
import { type ColumnFieldType, type ColumnMeta } from '../dataSelector/DataSelectorTypes';

type DataField = keyof VideoStreamDto;
type EditorComponent = React.ComponentType<{ data: VideoStreamDto }>;

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

const createMultiSelectColumnConfigHook = ({
  dataField,
  fieldType,
  headerTitle,
  maxWidth,
  minWidth,
  width,
  EditorComponent,
  queryHook
}: ColumnConfigInputs) => ({ enableEdit = false, useFilter = true, values }: { enableEdit?: boolean; useFilter?: boolean; values?: string[] | undefined }) => {
  const { data, isLoading, isFetching, isError } = queryHook ? queryHook() : { data: undefined, isError: false, isFetching: false, isLoading: false };

  const bodyTemplate = (bodyData: VideoStreamDto) => {
    const value = bodyData[dataField];

    if (value === undefined) {
      return <span />;
    }

    if (!enableEdit) {
      return <span>{value.toString()}</span>;
    }

    if (EditorComponent) {
      // if (headerTitle === 'Group') {
      //   console.log('EditorComponent', bodyData)
      // }

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
      className="p-column-filter text-xs"
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

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate,
    field: dataField,
    fieldType,
    filter: useFilter,
    filterField: dataField,
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

export default createMultiSelectColumnConfigHook;
