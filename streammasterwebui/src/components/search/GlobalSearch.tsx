import { getTopToolOptions } from "@/lib/common/common";
import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import React, { type ChangeEvent } from "react";
import { type ColumnMeta } from "../dataSelector/DataSelectorTypes";

type GlobalSearchProps = {
  readonly clearSourceFilter: () => void;
  readonly columns?: ColumnMeta[];
  readonly globalSearchName: string;
  readonly globalSourceFilterValue?: string;
  readonly onGlobalSourceFilterChange: (e: ChangeEvent<HTMLInputElement>) => void; // Define proper type here
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
