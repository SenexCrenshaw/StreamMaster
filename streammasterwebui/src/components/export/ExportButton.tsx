import { Button } from "primereact/button";
import { memo } from "react";
import { getTopToolOptions } from "../../common/common";

type ExportButtonProps = {
  exportCSV: () => void;
}

const ExportButton: React.FC<ExportButtonProps> = ({ exportCSV }) => {
  return (
    <Button
      className="p-button-text justify-content-end"
      data-pr-tooltip="CSV"
      icon="pi pi-file-export"
      onClick={() => exportCSV()}
      rounded
      text
      tooltip="Clear Filter"
      tooltipOptions={getTopToolOptions}
      type="button"
    />
  );
}

export default memo(ExportButton);
