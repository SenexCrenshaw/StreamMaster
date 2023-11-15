import { ExportComponent, HeaderLeft, MultiSelectCheckbox } from '@lib/common/common';
import { SMTextColor } from '../SMTextColor';
import { type DataSelectorProps as DataSelectorProperties } from './DataSelector';

interface TableHeaderProperties {
  dataSelectorProps: DataSelectorProperties;
  enableExport: boolean;
  exportCSV: () => void;
  headerName?: string;
  onMultiSelectClick?: (value: boolean) => void;
  rowClick: boolean;
  setRowClick: (value: boolean) => void;
}

const TableHeader: React.FC<TableHeaderProperties> = ({
  headerName,
  onMultiSelectClick,
  rowClick,
  setRowClick,
  enableExport,
  exportCSV,
  dataSelectorProps
}) => (
  <div className="flex grid flex-row w-full flex-wrap grid align-items-center w-full col-12 h-full p-0 debug">
    <div className="flex col-2 h-full text-sm align-items-center p-0 debug">
      <SMTextColor text={headerName} />
      <div hidden={dataSelectorProps.selectionMode !== 'selectable'}>
        <MultiSelectCheckbox onMultiSelectClick={onMultiSelectClick} rowClick={rowClick} setRowClick={setRowClick} />
      </div>
    </div>
    <div className="flex col-10 h-full align-items-center p-0 px-2 m-0 debug">
      <div className="grid mt-2 flex flex-nowrap flex-row justify-content-between align-items-center col-12 px-0 border-1">
        <HeaderLeft props={dataSelectorProps} />

        {dataSelectorProps.headerRightTemplate}
        {enableExport && <ExportComponent exportCSV={exportCSV} />}
      </div>
    </div>
  </div>
);

export default TableHeader;
