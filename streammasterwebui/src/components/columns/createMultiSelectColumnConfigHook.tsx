import { type ColumnFilterElementTemplateOptions } from "primereact/column";
import { type MultiSelectChangeEvent } from "primereact/multiselect";
import { MultiSelect } from "primereact/multiselect";
import { type VideoStreamDto } from "../../store/iptvApi";
import { type ColumnFieldType } from "../dataSelector/DataSelectorTypes";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";
import { type QueryHook } from "../../common/common";
import { isEmptyObject } from "../../common/common";


type DataField = keyof VideoStreamDto;
type EditorComponent = React.ComponentType<{ data: VideoStreamDto }>;

type ColumnConfigInputs = {
  EditorComponent?: EditorComponent,
  dataField: DataField,
  fieldType?: ColumnFieldType,
  headerTitle: string,
  queryHook?: QueryHook,
  useFilter?: boolean,
  width?: number
};

const createMultiSelectColumnConfigHook = ({
  dataField,
  fieldType,
  headerTitle,
  width = 2,
  EditorComponent,
  useFilter = true,
  queryHook
}: ColumnConfigInputs) => {

  return (enableEditMode = false, values: string[] | undefined = undefined) => {

    const { data, isLoading, isFetching, isError } = queryHook ? queryHook() : { data: undefined, isError: false, isFetching: false, isLoading: false };

    const bodyTemplate = (bodyData: VideoStreamDto) => {
      const value = bodyData[dataField];

      if (value === undefined) {
        return <span />;
      }

      if (!enableEditMode) {
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
        maxWidth: `${width}rem`,
        width: `${width}rem`,
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
