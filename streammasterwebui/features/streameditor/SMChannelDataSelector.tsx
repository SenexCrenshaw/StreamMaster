// import DataSelector2 from '@components/dataSelector/DataSelector2';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import ChannelDeleteDialog from '@components/smchannels/ChannelDeleteDialog';

import { SMChannelDto } from '@lib/apiDefs';

import { GetMessage, arraysContainSameStrings } from '@lib/common/common';
import { ChannelGroupDto } from '@lib/iptvApi';
import { useQueryAdditionalFilters } from '@lib/redux/slices/useQueryAdditionalFilters';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import useSMChannels from '@lib/smAPI/SMChannels/useSMChannels';

import { Suspense, lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';
const DataSelector2 = lazy(() => import('@components/dataSelector/DataSelector2'));
const StreamCopyLinkDialog = lazy(() => import('@components/smstreams/StreamCopyLinkDialog'));
// const StreamVisibleDialog = lazy(() => import('@components/streams/StreamVisibleDialog'));

interface SMChannelDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly reorderable?: boolean;
}

const SMChannelDataSelector = ({ enableEdit: propsEnableEdit, id, reorderable }: SMChannelDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelDataSelector`;

  const { selectSelectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');
  const [enableEdit, setEnableEdit] = useState<boolean>(true);

  const channelGroupNames = useMemo(() => selectSelectedItems.map((channelGroup) => channelGroup.name), [selectSelectedItems]);

  const { queryAdditionalFilter, setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);
  const { isLoading } = useSMChannels();

  useEffect(() => {
    if (!arraysContainSameStrings(queryAdditionalFilter?.values, channelGroupNames)) {
      setQueryAdditionalFilter({
        field: 'Group',
        matchMode: 'equals',
        values: channelGroupNames
      });
    }
  }, [channelGroupNames, dataKey, queryAdditionalFilter, setQueryAdditionalFilter]);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [propsEnableEdit]);

  const targetActionBodyTemplate = useCallback(
    (data: SMChannelDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <Suspense>
          <StreamCopyLinkDialog realUrl={data?.realUrl} />
          <ChannelDeleteDialog iconFilled={false} id={dataKey} skipOverLayer value={data} />
        </Suspense>
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
      { field: 'channelNumber', width: '4rem' },
      { field: 'logo', fieldType: 'image', width: '4rem' },
      { field: 'name', filter: true, sortable: true },
      { field: 'group', filter: true, sortable: true, width: '5rem' },
      {
        align: 'right',
        bodyTemplate: targetActionBodyTemplate,
        field: 'isHidden',
        fieldType: 'actions',
        header: 'Actions',
        width: '5rem'
      }
    ],
    [targetActionBodyTemplate]
  );

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex justify-content-end align-items-center w-full gap-1">
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
    <Suspense fallback={<div>Loading...</div>}>
      <DataSelector2
        addOrRemove
        columns={columns}
        defaultSortField="name"
        defaultSortOrder={1}
        emptyMessage="No Channels"
        headerName={GetMessage('channels').toUpperCase()}
        headerRightTemplate={rightHeaderTemplate}
        isLoading={isLoading}
        id={dataKey}
        // onSelectionChange={(value, selectAll) => {
        //   if (selectAll !== true) {
        //     setSelectedSMStreams(value as SMChannelDto[]);
        //   }
        // }}

        queryFilter={useSMChannels}
        selectedItemsKey="selectSelectedSMChannelDtoItems"
        // selectionMode="multiple"
        style={{ height: 'calc(100vh - 40px)' }}
      />
    </Suspense>
  );
};

export default memo(SMChannelDataSelector);
