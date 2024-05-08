import AddButton from '@components/buttons/AddButton';
import MinusButton from '@components/buttons/MinusButton';
import M3UFilesButton from '@components/m3u/M3UFilesButton';

import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import StreamVisibleDialog from '@components/smstreams/StreamVisibleDialog';
import { GetMessage } from '@lib/common/common';
import { useSelectSMStreams } from '@lib/redux/slices/selectedSMStreamsSlice';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { AddSMStreamToSMChannel, RemoveSMStreamFromSMChannel } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';

import { useSMStreamGroupColumnConfig } from '@components/columns/SMStreams/useSMChannelGroupColumnConfig';
import { useSMStreamM3UColumnConfig } from '@components/columns/SMStreams/useSMStreamM3UColumnConfig';
import SMButton from '@components/sm/SMButton';
import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import CreateSMChannelsDialog from '@components/smchannels/CreateSMChannelsDialog';
import StreamMultiVisibleDialog from '@components/smstreams/StreamMultiVisibleDialog';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import { CreateSMChannelFromStream } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import {
  CreateSMChannelFromStreamRequest,
  GetSMChannelStreamsRequest,
  RemoveSMStreamFromSMChannelRequest,
  SMChannelDto,
  SMStreamDto
} from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent, DataTableRowEvent, DataTableValue } from 'primereact/datatable';
import { Suspense, lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';
import useSelectedSMItems from './useSelectedSMItems';

const SMDataTable = lazy(() => import('@components/smDataTable/SMDataTable'));

interface SMStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly height?: string;
  readonly showSelections?: boolean;
  readonly simple?: boolean;
}

const SMStreamDataSelector = ({ enableEdit: propsEnableEdit, height, id, simple = false, showSelections }: SMStreamDataSelectorProperties) => {
  const dataKey = `${id}-SMStreamDataSelector`;
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMItems();
  const { data: smChannelStreamsData } = useGetSMChannelStreams({ SMChannelId: selectedSMChannel?.Id } as GetSMChannelStreamsRequest);

  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { setSelectedSMStreams } = useSelectSMStreams(dataKey);
  const groupColumnConfig = useSMStreamGroupColumnConfig();
  const smStreamM3UColumnConfig = useSMStreamM3UColumnConfig();
  const { queryFilter } = useQueryFilter(dataKey);
  const { isLoading } = useGetPagedSMStreams(queryFilter);

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

        {/* <VideoStreamSetAutoSetEPGDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} /> */}
        {/* <VideoStreamDeleteDialog iconFilled={false} id={dataKey} values={[data]} /> */}
        {/* <VideoStreamEditDialog value={data} /> */}
        {/* <VideoStreamCopyLinkDialog value={data} />
        <VideoStreamSetTimeShiftDialog iconFilled={false} value={data} />
        <VideoStreamResetLogoDialog value={data} />
        <VideoStreamSetLogoFromEPGDialog value={data} />
        <VideoStreamVisibleDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} />
        <VideoStreamSetAutoSetEPGDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} />
        <VideoStreamDeleteDialog iconFilled={false} id={dataKey} values={[data]} />
        <VideoStreamEditDialog value={data} /> */}
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
      const found = smChannelStreamsData?.some((item) => item.Id === data.Id) ?? false;

      let toolTip = 'Add Channel';
      if (selectedSMChannel !== undefined) {
        toolTip = 'Remove Stream From "' + selectedSMChannel.Name + '"';
        if (found)
          return (
            <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
              <MinusButton
                iconFilled={false}
                onClick={() => {
                  if (!data.Id || selectedSMChannel === undefined) {
                    return;
                  }
                  const request: RemoveSMStreamFromSMChannelRequest = { SMChannelId: selectedSMChannel.Id, SMStreamId: data.Id };
                  RemoveSMStreamFromSMChannel(request)
                    .then((response) => {
                      console.log('Remove Stream', response);
                    })
                    .catch((error) => {
                      console.error('Remove Stream', error.message);
                    });
                }}
                tooltip={toolTip}
              />
            </div>
          );

        toolTip = 'Add Stream To "' + selectedSMChannel.Name + '"';
        return (
          <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
            <AddButton
              iconFilled={false}
              onClick={() => {
                AddSMStreamToSMChannel({ SMChannelId: selectedSMChannel?.Id ?? 0, SMStreamId: data.Id })
                  .then((response) => {})
                  .catch((error) => {
                    console.error(error.message);
                    throw error;
                  });
              }}
              tooltip={toolTip}
            />

            {/* {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />} */}
          </div>
        );
      }

      return (
        <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
          <AddButton
            iconFilled={false}
            onClick={() => {
              CreateSMChannelFromStream({ StreamId: data.Id } as CreateSMChannelFromStreamRequest)
                .then((response) => {
                  // if (response?.IsError) {
                  //   smMessages.AddMessage({ Summary: response.ErrorMessage, Severity: 'error' } as SMMessage);
                  //   return;
                  // }
                })
                .catch((error) => {
                  // console.error(error.message);
                  // throw error;
                });
            }}
            tooltip={toolTip}
          />
          {/* {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />} */}
        </div>
      );
    },
    [selectedSMChannel, smChannelStreamsData]
  );

  function addOrRemoveHeaderTemplate() {
    return <SMTriSelectShowHidden dataKey={dataKey} />;
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
      <div className="flex flex-row justify-content-end align-items-center w-full gap-1  pr-1">
        <div>
          <M3UFilesButton />
        </div>
        <SMButton className="icon-red-filled" icon="pi-times" rounded onClick={() => {}} />
        {/* <SMButton className="icon-green-filled" icon="pi-plus" rounded onClick={() => {}} /> */}
        <CreateSMChannelsDialog selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} />
        <SMButton className="icon-orange-filled" icon="pi pi-bars" rounded onClick={() => {}} />
        <StreamMultiVisibleDialog iconFilled selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} skipOverLayer />
        {/* <TriSelectShowHidden dataKey={dataKey} /> */}
        {/* <TriSelectShowHidden dataKey={dataKey} />
        <VideoStreamSetTimeShiftsDialog id={dataKey} />
        <VideoStreamResetLogosDialog id={dataKey} />
        <VideoStreamSetLogosFromEPGDialog id={dataKey} />
        <AutoSetChannelNumbers id={dataKey} />
        <VideoStreamVisibleDialog id={dataKey} />
        <VideoStreamSetAutoSetEPGDialog iconFilled id={dataKey} />
        <VideoStreamDeleteDialog iconFilled id={dataKey} />
        <VideoStreamAddDialog group={channelGroupNames?.[0]} /> */}
      </div>
    ),
    [dataKey]
  );

  const setSelectedSMEntity = useCallback(
    (data: DataTableValue, toggle?: boolean) => {
      if (toggle === true && selectedSMChannel !== undefined && data !== undefined && data.id === selectedSMChannel.Id) {
        setSelectedSMChannel(undefined);
      } else {
        setSelectedSMChannel(data as SMChannelDto);
      }
    },
    [selectedSMChannel, setSelectedSMChannel]
  );

  const rowClass = useCallback(
    (data: unknown): string => {
      const isHidden = getRecord(data, 'IsHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (data && smChannelStreamsData && smChannelStreamsData !== undefined && Array.isArray(smChannelStreamsData)) {
        const id = getRecord(data, 'Id');
        if (smChannelStreamsData.some((stream) => stream.Id === id)) {
          return 'bg-blue-900';
        }
      }

      return '';
    },
    [smChannelStreamsData]
  );

  return (
    <Suspense>
      <SMDataTable
        columns={columns}
        defaultSortField="Name"
        defaultSortOrder={1}
        addOrRemoveTemplate={addOrRemoveTemplate}
        addOrRemoveHeaderTemplate={addOrRemoveHeaderTemplate}
        enablePaginator
        emptyMessage="No Streams"
        headerName={GetMessage('m3ustreams').toUpperCase()}
        headerRightTemplate={simple === true ? undefined : rightHeaderTemplate}
        isLoading={isLoading}
        id={dataKey}
        onSelectionChange={(value, selectAll) => {
          if (selectAll !== true) {
            setSelectedSMStreams(value as SMStreamDto[]);
          }
        }}
        onClick={(e: any) => {
          if (e.target.className && e.target.className === 'p-datatable-wrapper') {
            setSelectedSMChannel(undefined);
          }
        }}
        onRowExpand={(e: DataTableRowEvent) => {
          setSelectedSMEntity(e.data);
        }}
        onRowClick={(e: DataTableRowClickEvent) => {
          setSelectedSMEntity(e.data, true);
          // props.onRowClick?.(e);
        }}
        rowClass={rowClass}
        queryFilter={useGetPagedSMStreams}
        selectedItemsKey="selectSelectedSMStreamDtoItems"
        selectionMode="multiple"
        style={{ height: height ?? 'calc(100vh - 100px)' }}
      />
    </Suspense>
  );
};

export default memo(SMStreamDataSelector);
