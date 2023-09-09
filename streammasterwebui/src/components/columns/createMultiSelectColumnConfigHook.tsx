import { type ColumnFilterElementTemplateOptions } from "primereact/column";
import { MultiSelect, type MultiSelectChangeEvent } from "primereact/multiselect";
import { isEmptyObject, type QueryHook } from "../../common/common";
import { type ChannelGroupIdName, type VideoStreamDto } from "../../store/iptvApi";
import { type ColumnFieldType, type ColumnMeta } from "../dataSelector/DataSelectorTypes";


type DataField = keyof VideoStreamDto;
type EditorComponent = React.ComponentType<{ data: VideoStreamDto }>;

type ColumnConfigInputs = {
  EditorComponent?: EditorComponent,
  dataField: DataField,
  fieldType?: ColumnFieldType,
  headerTitle: string,
  maxWidth?: number,
  minWidth?: number,
  queryHook?: QueryHook<ChannelGroupIdName[] | string[]>,
  useFilter?: boolean,
  width?: number
};

const createMultiSelectColumnConfigHook = ({
  dataField,
  fieldType,
  headerTitle,
  maxWidth = undefined,
  minWidth = undefined,
  width = undefined,
  EditorComponent,
  queryHook
}: ColumnConfigInputs) => {

  return ({ enableEdit = false, useFilter = true, values = undefined }: { enableEdit?: boolean; useFilter?: boolean; values?: string[] | undefined; }) => {

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
        return <EditorComponent data={bodyData} />;
      }

      return <span>{value.toString()}</span>;
    };

    const itemTemplate = (option: string) => {
      return (
        <div className="align-items-center gap-2">
          <span>{option}</span>
        </div>
      );
    };

    const filterTemplate = (options: ColumnFilterElementTemplateOptions) => {
      return (
        <MultiSelect
          className="p-column-filter text-xs"
          filter
          itemTemplate={itemTemplate}
          maxSelectedLabels={1}
          onChange={(e: MultiSelectChangeEvent) => {
            if (isEmptyObject(e.value)) {
              options.filterApplyCallback(undefined);
            } else {
              options.filterApplyCallback(e.value);
            }
          }}
          options={values && values.length > 0 ? values : data}
          placeholder="Any"
          value={options.value}
        />
      );
    }

    if (dataField === undefined) {
      console.error('dataField is undefined');
    }

    const columnConfig: ColumnMeta = {
      align: 'left',
      bodyTemplate,
      field: dataField,
      fieldType: fieldType,
      filter: useFilter,
      filterField: dataField,
      header: headerTitle,
      sortable: true,

      style: {
        maxWidth: maxWidth !== undefined ? `${maxWidth}rem` : (width !== undefined ? `${width}rem` : undefined),
        minWidth: minWidth !== undefined ? `${minWidth}rem` : (width !== undefined ? `${width}rem` : undefined),
        width: width !== undefined ? `${width}rem` : (minWidth !== undefined ? `${minWidth}rem` : undefined),
      },
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
}

export default createMultiSelectColumnConfigHook;
