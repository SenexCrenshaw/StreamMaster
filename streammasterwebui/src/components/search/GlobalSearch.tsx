import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import { type ChangeEvent } from "react";
import React from "react";
import { getTopToolOptions } from "../../common/common";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";

type GlobalSearchProps = {
  clearSourceFilter: () => void;
  columns?: ColumnMeta[];
  globalSearchName: string;
  globalSourceFilterValue?: string;
  onGlobalSourceFilterChange: (e: ChangeEvent<HTMLInputElement>) => void; // Define proper type here
}

const GlobalSearch: React.FC<GlobalSearchProps> = ({
  columns = [],
  clearSourceFilter,
  onGlobalSourceFilterChange,
  globalSearchName,
  globalSourceFilterValue
}) => {
  const isColumnsEmpty = !columns.length;

  return (
    <>
      <Button
        className="p-button-text"
        hidden={isColumnsEmpty}
        icon="pi pi-filter-slash"
        onClick={clearSourceFilter}
        rounded
        text
        tooltip="Clear Filter"
        tooltipOptions={getTopToolOptions}
        type="button"
      />
      <InputText
        className="withpadding flex w-full"
        disabled={isColumnsEmpty}
        hidden={isColumnsEmpty}
        onChange={onGlobalSourceFilterChange}
        placeholder={globalSearchName}
        value={globalSourceFilterValue ?? ''}
      />
    </>
  );
}

export default React.memo(GlobalSearch);
