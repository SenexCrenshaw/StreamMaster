import AddButton from '@components/buttons/AddButton';
import M3UFilesButton from '@components/m3u/M3UFilesButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import StreamVisibleDialog from '@components/smstreams/StreamVisibleDialog';
import { GetMessage } from '@lib/common/common';
import { useSelectSMStreams } from '@lib/redux/slices/selectedSMStreamsSlice';

import BaseButton from '@components/buttons/BaseButton';
import { useSMStreamGroupColumnConfig } from '@components/columns/SMStreams/useSMChannelGroupColumnConfig';
import { useSMStreamM3UColumnConfig } from '@components/columns/SMStreams/useSMStreamM3UColumnConfig';
import { TriSelectShowHidden } from '@components/selectors/TriSelectShowHidden';
import StreamMultiVisibleDialog from '@components/smstreams/StreamMultiVisibleDialog';
import selectedSMChannel from '@lib/redux/slices/selectedSMChannel';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import { GetSMChannelStreamsRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent, DataTableRowEvent } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';

interface SSMChannelSMStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id?: string;
  readonly showSelections?: boolean;
  readonly smChannelId?: number;
}

const SMChannelSMStreamDataSelector = ({ enableEdit: propsEnableEdit, id, smChannelId, showSelections }: SSMChannelSMStreamDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelSMStreamDataSelector`;

  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { setSelectedSMStreams } = useSelectSMStreams(dataKey);
  const groupColumnConfig = useSMStreamGroupColumnConfig();
  const smStreamM3UColumnConfig = useSMStreamM3UColumnConfig();

  const { data, isLoading } = useGetSMChannelStreams({ SMChannelId: smChannelId } as GetSMChannelStreamsRequest);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const actionBodyTemplate = useCallback(
    (data: SMStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <StreamCopyLinkDialog realUrl={data.RealUrl} />
        <StreamVisibleDialog iconFilled={false} value={data} />
      </div>
    ),
    []
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      // { field: 'Logo', fieldType: 'image' },
      { field: 'Name', filter: true, sortable: true, width: '18rem' },
      groupColumnConfig,
      smStreamM3UColumnConfig,
      // { field: 'Group', filter: true, sortable: true },
      // { field: 'M3UFileName', filter: true, header: 'M3U', sortable: true },
      { align: 'right', bodyTemplate: actionBodyTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: '4rem' }
    ],
    [actionBodyTemplate, groupColumnConfig, smStreamM3UColumnConfig]
  );

  const addOrRemoveTemplate = useCallback(
    (data: any) => {
      return <AddButton iconFilled={false} onClick={() => console.log('AddButton')} />;
      // const found = smChannelStreamsData?.some((item) => item.Id === data.Id) ?? false;

      // let toolTip = 'Add Channel';
      // if (selectedSMChannel !== undefined) {
      //   toolTip = 'Remove Stream From "' + selectedSMChannel.Name + '"';
      //   if (found)
      //     return (
      //       <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
      //         <MinusButton
      //           iconFilled={false}
      //           onClick={() => {
      //             if (!data.Id || selectedSMChannel === undefined) {
      //               return;
      //             }
      //             const request: RemoveSMStreamFromSMChannelRequest = { SMChannelId: selectedSMChannel.Id, SMStreamId: data.Id };
      //             RemoveSMStreamFromSMChannel(request)
      //               .then((response) => {
      //                 console.log('Remove Stream', response);
      //               })
      //               .catch((error) => {
      //                 console.error('Remove Stream', error.message);
      //               });
      //           }}
      //           tooltip={toolTip}
      //         />
      //       </div>
      //     );

      //   toolTip = 'Add Stream To "' + selectedSMChannel.Name + '"';
      //   return (
      //     <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
      //       <AddButton
      //         iconFilled={false}
      //         onClick={() => {
      //           AddSMStreamToSMChannel({ SMChannelId: selectedSMChannel?.Id ?? 0, SMStreamId: data.Id })
      //             .then((response) => {})
      //             .catch((error) => {
      //               console.error(error.message);
      //               throw error;
      //             });
      //         }}
      //         tooltip={toolTip}
      //       />

      //       {/* {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />} */}
      //     </div>
      //   );
      // }

      // return (
      //   <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
      //     <AddButton
      //       iconFilled={false}
      //       onClick={() => {
      //         CreateSMChannelFromStream({ StreamId: data.Id } as CreateSMChannelFromStreamRequest)
      //           .then((response) => {
      //             // if (response?.IsError) {
      //             //   smMessages.AddMessage({ Summary: response.ErrorMessage, Severity: 'error' } as SMMessage);
      //             //   return;
      //             // }
      //           })
      //           .catch((error) => {
      //             // console.error(error.message);
      //             // throw error;
      //           });
      //       }}
      //       tooltip={toolTip}
      //     />
      //     {/* {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />} */}
      //   </div>
      // );
    },
    [selectedSMChannel]
  );

  function addOrRemoveHeaderTemplate() {
    return <TriSelectShowHidden dataKey={dataKey} />;
    // const isSelected = false;

    // if (!isSelected) {
    //   return (
    //     <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
    //       {/* <AddButton iconFilled={false} onClick={() => console.log('AddButton')} tooltip="Add All Channels" /> */}
    //       {/* {showSelection && <Checkbox checked={state.selectAll} className="pl-1" onChange={() => toggleAllSelection()} />} */}
    //     </div>
    //   );
    // }

    // return (
    //   <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
    //     <AddButton iconFilled={false} onClick={() => console.log('AddButton')} />
    //     {/* {showSelection && <Checkbox checked={state.selectAll} className="pl-1" onChange={() => toggleAllSelection()} />} */}
    //   </div>
    // );
  }

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex flex-row justify-content-end align-items-center w-full gap-2 pr-2">
        <div>
          <M3UFilesButton />
        </div>
        <BaseButton className="button-red" icon="pi pi-times" rounded onClick={() => {}} />
        <BaseButton className="button-yellow" icon="pi-plus" rounded onClick={() => {}} />
        <BaseButton className="button-orange" icon="pi pi-bars" rounded onClick={() => {}} />

        <StreamMultiVisibleDialog iconFilled selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} skipOverLayer />
      </div>
    ),
    [dataKey]
  );

  return (
    <SMDataTable
      columns={columns}
      defaultSortField="Name"
      defaultSortOrder={1}
      // addOrRemoveTemplate={addOrRemoveTemplate}
      // addOrRemoveHeaderTemplate={addOrRemoveHeaderTemplate}
      dataSource={data}
      enablePaginator
      emptyMessage="No Streams"
      headerName={GetMessage('m3ustreams').toUpperCase()}
      headerRightTemplate={rightHeaderTemplate}
      isLoading={isLoading}
      id={dataKey}
      onSelectionChange={(value, selectAll) => {
        if (selectAll !== true) {
          setSelectedSMStreams(value as SMStreamDto[]);
        }
      }}
      onClick={(e: any) => {
        if (e.target.className && e.target.className === 'p-datatable-wrapper') {
          // setSelectedSMChannel(undefined);
        }
      }}
      onRowExpand={(e: DataTableRowEvent) => {
        // 1setSelectedSMEntity(e.data);
      }}
      onRowClick={(e: DataTableRowClickEvent) => {
        // setSelectedSMEntity(e.data, true);
        // props.onRowClick?.(e);
      }}
      // rowClass={rowClass}

      selectedItemsKey="selectSelectedSMStreamDtoItems"
      selectionMode="multiple"
      style={{ height: 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelSMStreamDataSelector);
