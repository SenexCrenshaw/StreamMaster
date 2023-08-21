import { type ColumnFilterElementTemplateOptions } from "primereact/column";
import { type MultiSelectChangeEvent } from "primereact/multiselect";
import { MultiSelect } from "primereact/multiselect";
import { type VideoStreamDto } from "../../store/iptvApi";
import { type ColumnFieldType } from "../dataSelector/DataSelectorTypes";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";

type QueryHookResult = {
  data?: string[];
  isError: boolean;
  isLoading: boolean;
};

type QueryHook = () => QueryHookResult;

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

    let names: string[] = values ?? [];

    if (!values && queryHook !== undefined) {
      const { data, isLoading, isError } = queryHook();

      if (isLoading || isError) {
        return { field: '' };
      }

      if (data) {
        names = data;
      }
    }

    const bodyTemplate = (data: VideoStreamDto) => {
      const value = data[dataField];

      if (value === undefined) {
        return <span />;
      }

      if (!enableEditMode) {
        return <span>{value.toString()}</span>;
      }

      if (EditorComponent) {
        return <EditorComponent data={data} />;
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
          onChange={(e: MultiSelectChangeEvent) => options.filterApplyCallback(e.value)}
          options={names}
          placeholder="Any"
          value={options.value}
        />
      );
    };

    if (dataField === undefined) {
      console.log('dataField is undefined');
    }

    const columnConfig: ColumnMeta = {
      align: 'left',
      bodyTemplate,
      field: dataField,
      fieldType: fieldType,
      filter: useFilter,
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

    return columnConfig;
  };
}

export default createMultiSelectColumnConfigHook;
