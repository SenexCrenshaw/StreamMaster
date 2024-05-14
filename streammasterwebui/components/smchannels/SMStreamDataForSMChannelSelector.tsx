import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { AdditionalFilterProperties, GetMessage, isEmptyObject } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';

import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { AddSMStreamToSMChannel, RemoveSMStreamFromSMChannel } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import { GetSMChannelStreamsRequest, M3UFileDto, RemoveSMStreamFromSMChannelRequest, SMChannelDto, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { ReactNode, memo, useCallback, useEffect, useMemo, useState } from 'react';

import SMButton from '@components/sm/SMButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { useQueryAdditionalFilters } from '@lib/redux/hooks/queryAdditionalFilters';
import useGetM3UFiles from '@lib/smAPI/EPGFiles/useGetM3UFiles';
import { ColumnFilterElementTemplateOptions } from 'primereact/column';
import { MultiSelect, MultiSelectChangeEvent } from 'primereact/multiselect';
interface SMStreamDataForSMChannelSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly height?: string;

  readonly name: string | undefined;
  readonly smChannel?: SMChannelDto;
}

const SMStreamDataForSMChannelSelector = ({ enableEdit: propsEnableEdit, height, id, name, smChannel }: SMStreamDataForSMChannelSelectorProperties) => {
  const dataKey = `${id}-SMStreamDataForSMChannelSelector`;
  const { setQueryAdditionalFilters } = useQueryAdditionalFilters(dataKey);

  const [selectedM3UFiles, setSelectedM3UFiles] = useState<M3UFileDto[]>([]);

  const { data: m3uFiles } = useGetM3UFiles();
  const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(`${id}-SMStreamDataForSMChannelSelector`);
  const { data: smChannelData, isLoading: smChannelIsLoading } = useGetSMChannelStreams({ SMChannelId: smChannel?.Id } as GetSMChannelStreamsRequest);

  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { queryFilter } = useQueryFilter(dataKey);

  const { isLoading } = useGetPagedSMStreams(queryFilter);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

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
      // if (!m3uFiles || !option) return null;
      // return (
      //   <div className="flex align-items-center gap-1">
      //     <span>{option.Name}</span>
      //   </div>
      // );
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
      let found = false;
      if (smChannel) {
        found = smChannelData?.some((item) => item.Id === data.Id) ?? false;
      } else {
        found = selectedItems?.some((item) => item.Id === data.Id) ?? false;
      }

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
                className="border-noround borderread icon-red"
                iconFilled={false}
                onClick={() => {
                  if (!data.Id) {
                    return;
                  }

                  if (smChannel) {
                    const request = {} as RemoveSMStreamFromSMChannelRequest;
                    request.SMChannelId = smChannel.Id;
                    request.SMStreamId = data.Id;
                    RemoveSMStreamFromSMChannel(request)
                      .then((response) => {
                        console.log('Remove Stream', response);
                      })
                      .catch((error) => {
                        console.error('Remove Stream', error.message);
                      });
                  } else {
                    const newData = selectedItems?.filter((item) => item.Id !== data.Id);
                    setSelectedItems(newData);
                  }
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
            className="w-2rem border-noround borderread icon-green"
            iconFilled={false}
            onClick={() => {
              if (smChannel) {
                AddSMStreamToSMChannel({ SMChannelId: smChannel?.Id ?? 0, SMStreamId: data.Id })
                  .then((response) => {})
                  .catch((error) => {
                    console.error(error.message);
                    throw error;
                  });
              } else {
                setSelectedItems([...(selectedItems ?? []), data]);
              }
            }}
            tooltip={toolTip}
          />
        </div>
      );
    },
    [name, selectedItems, setSelectedItems, smChannel, smChannelData]
  );

  function addOrRemoveHeaderTemplate() {
    return <SMTriSelectShowHidden dataKey={dataKey} />;
  }

  const rowClass = useCallback(
    (data: unknown): string => {
      const id = getRecord(data, 'Id');
      let found = false;
      if (smChannel) {
        found = smChannelData?.some((item) => item.Id === id) ?? false;
      } else {
        found = selectedItems?.some((item) => item.Id === id) ?? false;
      }

      if (found) {
        return 'p-hidden';
      }

      const isHidden = getRecord(data, 'IsHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      return '';
    },
    [selectedItems, smChannel, smChannelData]
  );

  return (
    <SMDataTable
      addOrRemoveTemplate={addOrRemoveTemplate}
      addOrRemoveHeaderTemplate={addOrRemoveHeaderTemplate}
      columns={columns}
      defaultSortField="Name"
      defaultSortOrder={1}
      enablePaginator
      emptyMessage="No Streams"
      headerName={GetMessage('m3ustreams').toUpperCase()}
      headerClassName="header-text-channels"
      isLoading={isLoading || smChannelIsLoading}
      id={dataKey}
      rowClass={rowClass}
      queryFilter={useGetPagedSMStreams}
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMStreamDataForSMChannelSelector);
