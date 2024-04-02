import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import M3UFilesEditor2 from '@components/m3u/M3UFilesEditor';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import StreamVisibleDialog from '@components/smstreams/StreamVisibleDialog';
import { GetMessage } from '@lib/common/common';
import { useSelectSMStreams } from '@lib/redux/slices/selectedSMStreams';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { AddSMStreamToSMChannel, CreateSMChannelFromStream } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import { AddSMStreamToSMChannelRequest, CreateSMChannelFromStreamRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';

import { lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';
const DataSelector2 = lazy(() => import('@components/dataSelector/DataSelector2'));

interface SMStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
}

const SMStreamDataSelector = ({ enableEdit: propsEnableEdit, id }: SMStreamDataSelectorProperties) => {
  const dataKey = `${id}-SMStreamDataSelector`;
  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { setSelectedSMStreams } = useSelectSMStreams(dataKey);

  const { queryFilter } = useQueryFilter(dataKey);
  const { isLoading } = useGetPagedSMStreams(queryFilter);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [propsEnableEdit]);

  const actionBodyTemplate = useCallback(
    (data: SMStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <StreamCopyLinkDialog realUrl={data.realUrl} />
        <StreamVisibleDialog iconFilled={false} id={dataKey} skipOverLayer values={[data]} />

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
    [dataKey]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'logo', fieldType: 'image', width: '4rem' },
      { field: 'name', filter: true, sortable: true },
      { field: 'group', filter: true, sortable: true, width: '5rem' },
      { field: 'm3UFileName', filter: true, header: 'M3U', sortable: true, width: '5rem' },
      {
        align: 'right',
        bodyTemplate: actionBodyTemplate,
        field: 'isHidden',
        fieldType: 'actions',
        header: 'Actions',
        width: '5rem'
      }
    ],
    [actionBodyTemplate]
  );

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <M3UFilesEditor2 />
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
    []
  );

  return (
    <DataSelector2
      columns={columns}
      defaultSortField="name"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      headerName={GetMessage('m3ustreams').toUpperCase()}
      headerRightTemplate={rightHeaderTemplate}
      isLoading={isLoading}
      id={dataKey}
      onChannelAdd={(e) => {
        CreateSMChannelFromStream({ streamId: e.id } as CreateSMChannelFromStreamRequest)
          .then((response) => {})
          .catch((error) => {
            console.error(error.message);
            throw error;
          });
      }}
      onStreamAdd={(e: AddSMStreamToSMChannelRequest) => {
        AddSMStreamToSMChannel(e)
          .then((response) => {})
          .catch((error) => {
            console.error(error.message);
            throw error;
          });
      }}
      onDelete={(e) => {
        console.log('Delete', e);
      }}
      onSelectionChange={(value, selectAll) => {
        if (selectAll !== true) {
          setSelectedSMStreams(value as SMStreamDto[]);
        }
      }}
      queryFilter={useGetPagedSMStreams}
      selectedSMStreamKey="SMChannelDataSelector"
      selectedSMChannelKey="SMChannelDataSelector"
      selectedItemsKey="selectSelectedSMStreamDtoItems"
      style={{ height: 'calc(100vh - 10px)' }}
    />
  );
};

export default memo(SMStreamDataSelector);
