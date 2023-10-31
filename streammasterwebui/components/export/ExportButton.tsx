import { getTopToolOptions } from '@lib/common/common';
import { Button } from 'primereact/button';
import { memo } from 'react';

interface ExportButtonProperties {
  exportCSV: () => void;
}

const ExportButton: React.FC<ExportButtonProperties> = ({ exportCSV }) => (
  <Button
    className="p-button-text justify-content-end"
    icon="pi pi-file-export"
    onClick={() => exportCSV()}
    rounded
    text
    tooltip="Export CSV"
    tooltipOptions={getTopToolOptions}
    type="button"
  />
);

export default memo(ExportButton);
