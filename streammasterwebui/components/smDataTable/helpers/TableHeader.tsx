import { SMTextColor } from '@components/sm/SMTextColor';
import { DataTableHeaderProperties } from '../types/smDataTableInterfaces';
import { HeaderLeft, MultiSelectCheckbox } from '@lib/common/common';

interface TableHeaderProperties {
  dataSelectorProps: DataTableHeaderProperties;
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
  <div className="flex grid flex-row align-items-center justify-content-between debug">
    {(headerName || onMultiSelectClick) && (
      <div className="col-4 text-sm debug">
        <SMTextColor text={headerName} />
        {onMultiSelectClick && (
          <div hidden={dataSelectorProps.selectionMode !== 'selectable'}>
            <MultiSelectCheckbox onMultiSelectClick={onMultiSelectClick} rowClick={rowClick} setRowClick={setRowClick} />
          </div>
        )}
      </div>
    )}
    {(dataSelectorProps.headerRightTemplate || enableExport || dataSelectorProps.headerLeftTemplate) && (
      <div className="col-8 debug p-0">
        <div className="flex flex-nowrap flex-row justify-content-between">
          {dataSelectorProps.headerLeftTemplate && <HeaderLeft props={dataSelectorProps} />}
          {dataSelectorProps.headerRightTemplate && dataSelectorProps.headerRightTemplate}
          {/* {enableExport && <ExportComponent exportCSV={exportCSV} />} */}
        </div>
      </div>
    )}
  </div>
);

export default TableHeader;
