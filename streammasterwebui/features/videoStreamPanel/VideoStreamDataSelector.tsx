import {
  useChannelNameColumnConfig,
  useChannelNumberColumnConfig,
} from '@/components/columns/columnConfigHooks'
import DataSelector from '@/components/dataSelector/DataSelector'
import { ColumnMeta } from '@/components/dataSelector/DataSelectorTypes'
import { GetMessage } from '@/lib/common/common'
import {
  VideoStreamDto,
  VideoStreamLinksAddVideoStreamToVideoStreamApiArg,
  useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery,
  useVideoStreamsGetPagedVideoStreamsQuery,
} from '@/lib/iptvApi'
import { AddVideoStreamToVideoStream } from '@/lib/smAPI/VideoStreamLinks/VideoStreamLinksMutateAPI'
import { skipToken } from '@reduxjs/toolkit/dist/query'
import { memo, useEffect, useMemo, useState } from 'react'

type VideoStreamDataSelectorProps = {
  readonly id: string
  readonly videoStreamId?: string
}

const VideoStreamDataSelector = ({
  id,
  videoStreamId,
}: VideoStreamDataSelectorProps) => {
  const dataKey = id + '-VideoStreamDataSelector'

  const [videoStreamIds, setVideoStreamIds] = useState<string[]>([] as string[])

  const { columnConfig: channelNumberColumnConfig } =
    useChannelNumberColumnConfig({ enableEdit: false })
  const { columnConfig: channelNameColumnConfig } = useChannelNameColumnConfig({
    enableEdit: false,
  })
  const videoStreamLinksGetVideoStreamVideoStreamIdsQuery =
    useVideoStreamLinksGetVideoStreamVideoStreamIdsQuery(
      videoStreamId ?? skipToken,
    )

  useEffect(() => {
    if (videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data !== undefined) {
      setVideoStreamIds(videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data)
    }
  }, [videoStreamLinksGetVideoStreamVideoStreamIdsQuery.data])

  const targetColumns = useMemo((): ColumnMeta[] => {
    let columnConfigs = [channelNumberColumnConfig, channelNameColumnConfig]

    return columnConfigs
  }, [channelNameColumnConfig, channelNumberColumnConfig])

  const rightHeaderTemplate = useMemo(() => {
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1" />
    )
  }, [])

  return (
    <DataSelector
      columns={targetColumns}
      defaultSortField="user_tvg_name"
      emptyMessage="No Streams"
      headerName={GetMessage('streams')}
      headerRightTemplate={rightHeaderTemplate}
      id={dataKey}
      isLoading={
        videoStreamLinksGetVideoStreamVideoStreamIdsQuery.isLoading ||
        videoStreamLinksGetVideoStreamVideoStreamIdsQuery.isFetching
      }
      onRowClick={async (e) => {
        if (e.data === undefined || videoStreamId === undefined) {
          return
        }

        let stream = {} as VideoStreamDto

        if (Array.isArray(e.data)) {
          stream = e.data[0] as VideoStreamDto
        } else {
          stream = e.data as VideoStreamDto
        }

        const toSend = {} as VideoStreamLinksAddVideoStreamToVideoStreamApiArg

        toSend.parentVideoStreamId = videoStreamId
        toSend.childVideoStreamId = stream.id

        await AddVideoStreamToVideoStream(toSend)
          .then()
          .catch((error) => {
            console.error('Add Stream Error: ' + error.message)
          })
      }}
      queryFilter={useVideoStreamsGetPagedVideoStreamsQuery}
      selectedItemsKey={`selectSelected` + videoStreamId}
      selectionMode="single"
      style={{ height: 'calc(100vh - 480px)' }}
      videoStreamIdsIsReadOnly={videoStreamIds || []}
    />
  )
}

VideoStreamDataSelector.displayName = 'Stream Editor'

export default memo(VideoStreamDataSelector)
