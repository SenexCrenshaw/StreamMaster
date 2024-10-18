import { DataTableHeaderProperties } from '../types/smDataTableInterfaces';

interface TableHeaderProperties {
  dataSelectorProps: DataTableHeaderProperties;
  enableExport: boolean;
  exportCSV: () => void;
  headerName?: string | React.ReactNode;
  headerClassName?: string;
  onMultiSelectClick?: (value: boolean) => void;
  rowClick: boolean;
  setRowClick: (value: boolean) => void;
}

const TableHeader: React.FC<TableHeaderProperties> = ({
  headerName,
  headerClassName = 'header-text',
  onMultiSelectClick,
  rowClick,
  setRowClick,
  enableExport,
  exportCSV,
  dataSelectorProps
}) => {
  return (
    <div className="flex flex-row align-items-center justify-content-between w-full px-1">
      <div className={`flex  ${!headerName && !onMultiSelectClick ? 'invisible' : ''} text-sm`}>
        {headerName || onMultiSelectClick ? <span className={headerClassName}>{headerName}</span> : null}
      </div>

      <div className={` flex justify-content-center items-center${!dataSelectorProps.headerCenterTemplate ? 'invisible' : ''}`}>
        {dataSelectorProps.headerCenterTemplate}
      </div>

      <div className={` flex justify-content-end items-center ${!dataSelectorProps.headerRightTemplate ? 'invisible' : ''}`}>
        {dataSelectorProps.headerRightTemplate}
      </div>
    </div>
  );
};

export default TableHeader;
