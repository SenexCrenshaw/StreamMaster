import { GetMessage } from '@/lib/common/common'
import {
  StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiArg,
  useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery,
  useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostMutation,
  useVideoStreamsGetPagedVideoStreamsQuery,
} from '@/lib/iptvApi'
import { useSelectedStreamGroup } from '@/lib/redux/slices/useSelectedStreamGroup'
import {
  useChannelGroupColumnConfig,
  useChannelNameColumnConfig,
  useChannelNumberColumnConfig,
  useM3UFileNameColumnConfig,
} from '@/src/components/columns/columnConfigHooks'
import DataSelector from '@/src/components/dataSelector/DataSelector'
import { ColumnMeta } from '@/src/components/dataSelector/DataSelectorTypes'
import { TriSelect } from '@/src/components/selectors/TriSelect'
import { skipToken } from '@reduxjs/toolkit/dist/query'
import { type DataTableRowClickEvent } from 'primereact/datatable'
import { memo, useMemo } from 'react'

type StreamGroupVideoStreamDataSelectorProps = {
  readonly id: string
}

const StreamGroupVideoStreamDataSelector = ({
  id,
}: StreamGroupVideoStreamDataSelectorProps) => {
  const dataKey = id + '-StreamGroupVideoStreamDataSelector'

  const { columnConfig: m3uFileNameColumnConfig } = useM3UFileNameColumnConfig({
    enableEdit: false,
  })
  const { columnConfig: channelNumberColumnConfig } =
    useChannelNumberColumnConfig({ enableEdit: false })
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({
    enableEdit: false,
  })
  const { columnConfig: channelGroupConfig } = useChannelGroupColumnConfig({
    enableEdit: false,
  })
  const { selectedStreamGroup } = useSelectedStreamGroup(id)
  const streamGroupsGetStreamGroupVideoStreamIdsQuery =
    useStreamGroupVideoStreamsGetStreamGroupVideoStreamIdsQuery(
      selectedStreamGroup?.id ?? skipToken,
    )
  const [streamGroupVideoStreamsSyncVideoStreamToStreamGroupMutation] =
    useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostMutation()

  const SyncVideoStream = async (videoId: string) => {
    if (!videoId || !selectedStreamGroup) {
      return
    }

    const toSend =
      {} as StreamGroupVideoStreamsSyncVideoStreamToStreamGroupPostApiArg

    toSend.streamGroupId = selectedStreamGroup.id
    toSend.videoStreamId = videoId

    await streamGroupVideoStreamsSyncVideoStreamToStreamGroupMutation(toSend)
      .then(() => {})
      .catch((error) => {
        console.error('Add Stream Error: ' + error.message)
      })
  }

  const targetColumns = useMemo((): ColumnMeta[] => {
    return [
      channelNumberColumnConfig,
      channelNameColumnConfig,
      channelGroupConfig,
      m3uFileNameColumnConfig,
    ]
  }, [
    channelNumberColumnConfig,
    channelNameColumnConfig,
    channelGroupConfig,
    m3uFileNameColumnConfig,
  ])

  const rightHeaderTemplate = useMemo(() => {
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1">
        <TriSelect dataKey={dataKey} />
      </div>
    )
  }, [dataKey])

  const onRowClick = async (event: DataTableRowClickEvent) => {
    await SyncVideoStream(event.data.id)
      .then(() => {})
      .catch((error) => {
        console.error('Add Stream Error: ' + error.message)
      })
  }

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      defaultSortOrder={1}
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      isLoading={
        streamGroupsGetStreamGroupVideoStreamIdsQuery.isLoading ||
        streamGroupsGetStreamGroupVideoStreamIdsQuery.isFetching
      }
      onRowClick={async (e) => await onRowClick(e)}
      queryFilter={useVideoStreamsGetPagedVideoStreamsQuery}
      selectedItemsKey="selectSelectedStreamGroupDtoItems"
      selectionMode="single"
      style={{ height: 'calc(100vh - 40px)' }}
      videoStreamIdsIsReadOnly={(
        streamGroupsGetStreamGroupVideoStreamIdsQuery.data ?? []
      ).map((x) => x.videoStreamId ?? '')}
    />
  )
}

StreamGroupVideoStreamDataSelector.displayName = 'Stream Editor'

export default memo(StreamGroupVideoStreamDataSelector)
