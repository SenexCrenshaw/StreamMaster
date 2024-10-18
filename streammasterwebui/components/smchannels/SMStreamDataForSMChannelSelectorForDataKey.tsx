import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { AdditionalFilterProperties, isEmptyObject } from '@lib/common/common';

import SMButton from '@components/sm/SMButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { useQueryAdditionalFilters } from '@lib/redux/hooks/queryAdditionalFilters';
import useGetM3UFiles from '@lib/smAPI/M3UFiles/useGetM3UFiles';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import { M3UFileDto, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
import { ReactNode, memo, useCallback, useMemo, useState } from 'react';

interface SMStreamDataForSMChannelSelectorForDataKeyProperties {
  readonly dataKey: string;
  readonly height?: string;
  readonly name: string | undefined;
  readonly selectedItems: SMStreamDto[];
  onChange: (e: SMStreamDto[]) => void;
}

const SMStreamDataForSMChannelSelectorForDataKey = ({
  height,
  dataKey,
  onChange,
  selectedItems,
  name
}: SMStreamDataForSMChannelSelectorForDataKeyProperties) => {
  const { setQueryAdditionalFilters } = useQueryAdditionalFilters(dataKey);
  const [selectedM3UFiles, setSelectedM3UFiles] = useState<M3UFileDto[]>([]);
  const { data: m3uFiles } = useGetM3UFiles();
  // const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);

  const itemTemplate = useCallback(
    (option: M3UFileDto) => {
      if (!m3uFiles || !option) return null;
      return (
        <div className="flex align-items-center gap-1">
          <span>{option.Name}</span>
        </div>
      );
    },
    [m3uFiles]
  );

  const selectedItemTemplate = useCallback(
    (option: M3UFileDto) => {
      if (selectedM3UFiles !== undefined && selectedM3UFiles.length > 0) {
        return <div className="flex align-content-center justify-content-start w-2rem max-w-2rem pi pi-file icon-yellow"></div>;
      }
      return <></>;
    },
    [selectedM3UFiles]
  );

  const m3uFilter = useCallback(
    (options: ColumnFilterElementTemplateOptions): ReactNode => {
      return (
        <MultiSelect
          className="w-12 input-height-with-no-borders flex justify-content-center align-items-center"
          filter
          itemTemplate={itemTemplate}
          maxSelectedLabels={1}
          showClear
          filterBy="Name"
          onChange={(e: MultiSelectChangeEvent) => {
            if (isEmptyObject(e.value)) {
              setQueryAdditionalFilters(undefined);
              setSelectedM3UFiles([]);
            } else {
              const ids = e.value.map((v: M3UFileDto) => v.Id);
              const newFilter = { field: 'M3UFileId', matchMode: 'in', values: ids } as AdditionalFilterProperties;
              setQueryAdditionalFilters(newFilter);
              setSelectedM3UFiles(e.value);
            }
          }}
          onShow={() => {}}
          options={m3uFiles}
          placeholder="M3U"
          value={selectedM3UFiles}
          selectedItemTemplate={selectedItemTemplate}
        />
      );
    },
    [itemTemplate, m3uFiles, selectedItemTemplate, selectedM3UFiles, setQueryAdditionalFilters]
  );

  const w = '9rem';
  const z = '5rem';
  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, maxWidth: w, minWidth: w, sortable: true, width: w },
      { field: 'M3UFileName', filter: true, filterElement: m3uFilter, header: 'M3U', maxWidth: z, minWidth: z, sortable: true, width: z }
    ],
    [m3uFilter]
  );

  const addOrRemoveTemplate = useCallback(
    (data: any) => {
      const found = selectedItems?.some((item) => item.Id === data.Id) ?? false;

      let toolTip = '';

      if (found) {
        if (name !== undefined) {
          toolTip = 'Remove Stream From "' + name + '"';
        } else {
          toolTip = 'Remove Stream';
        }
        return (
          <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
            <div className="flex align-content-center justify-content-center">
              <SMButton
                icon="pi-minus"
                buttonClassName="border-noround borderread icon-red"
                iconFilled={false}
                onClick={() => {
                  if (!data.Id) {
                    return;
                  }

                  const newData = selectedItems?.filter((item) => item.Id !== data.Id);
                  onChange(newData);
                }}
                tooltip={toolTip}
              />
            </div>
          </div>
        );
      }

      if (name !== undefined) {
        toolTip = 'Add Stream To "' + name + '"';
      } else {
        toolTip = 'Add Stream';
      }

      toolTip = 'Add Stream To "' + name + '"';

      return (
        <div className="flex align-content-center justify-content-center">
          <SMButton
            icon="pi-plus"
            buttonClassName="w-2rem border-noround borderread icon-green"
            iconFilled={false}
            onClick={() => {
              onChange([...(selectedItems ?? []), data]);
            }}
            tooltip={toolTip}
          />
        </div>
      );
    },
    [name, onChange, selectedItems]
  );

  function addOrRemoveHeaderTemplate() {
    return <SMTriSelectShowHidden dataKey={dataKey} />;
  }

  const rowClass = useCallback(
    (data: unknown): string => {
      const id = getRecord({ data, fieldName: 'Id' });

      const found = selectedItems?.some((item) => item.Id === id) ?? false;

      if (found) {
        return 'p-hidden';
      }

      const isHidden = getRecord({ data, fieldName: 'IsHidden' });

      if (isHidden === true) {
        return 'bg-red-900';
      }

      return '';
    },
    [selectedItems]
  );

  return (
    <SMDataTable
      addOrRemoveHeaderTemplate={addOrRemoveHeaderTemplate}
      addOrRemoveTemplate={addOrRemoveTemplate}
      columns={columns}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      enablePaginator
      headerClassName="header-text-channels"
      headerName="STREAMS"
      id={dataKey}
      queryFilter={useGetPagedSMStreams}
      rowClass={rowClass}
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMStreamDataForSMChannelSelectorForDataKey);
