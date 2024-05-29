import { HeaderLeft, MultiSelectCheckbox } from '@lib/common/common';
import { useMemo } from 'react';
import { DataTableHeaderProperties } from '../types/smDataTableInterfaces';

interface TableHeaderProperties {
  smTableIsSimple: boolean;
  dataSelectorProps: DataTableHeaderProperties;
  enableExport: boolean;
  exportCSV: () => void;
  headerName?: string;
  headerClassName?: string;
  onMultiSelectClick?: (value: boolean) => void;
  rowClick: boolean;
  setRowClick: (value: boolean) => void;
}

const TableHeader: React.FC<TableHeaderProperties> = ({
  smTableIsSimple,
  headerName,
  headerClassName = 'header-text',
  onMultiSelectClick,
  rowClick,
  setRowClick,
  enableExport,
  exportCSV,
  dataSelectorProps
}) => {
  const col = useMemo(() => {
    if (smTableIsSimple) {
      return 'col-8';
    }
    if (dataSelectorProps.headerRightTemplate) {
      return 'col-3';
    }
    return 'col-12';
  }, [dataSelectorProps.headerRightTemplate, smTableIsSimple]);

  const colRight = useMemo(() => {
    if (smTableIsSimple) {
      return 'col-4';
    }

    return 'col-8';
  }, [smTableIsSimple]);

  return (
    <div className="flex flex-row align-items-center justify-content-between w-full">
      {(headerName || onMultiSelectClick) && (
        <div className={`${col} text-sm`}>
          <span className={headerClassName}>{headerName}</span>

          {onMultiSelectClick && (
            <div hidden={dataSelectorProps.selectionMode !== 'selectable'}>
              <MultiSelectCheckbox onMultiSelectClick={onMultiSelectClick} rowClick={rowClick} setRowClick={setRowClick} />
            </div>
          )}
        </div>
      )}
      {(dataSelectorProps.headerRightTemplate || enableExport || dataSelectorProps.headerLeftTemplate) && (
        <div className={`${colRight} p-0`}>
          <div className="flex flex-nowrap flex-row justify-content-between">
            {dataSelectorProps.headerLeftTemplate && <HeaderLeft props={dataSelectorProps} />}
            {dataSelectorProps.headerRightTemplate && dataSelectorProps.headerRightTemplate}
            {/* {enableExport && <ExportComponent exportCSV={exportCSV} />} */}
          </div>
        </div>
      )}
    </div>
  );
};

export default TableHeader;
