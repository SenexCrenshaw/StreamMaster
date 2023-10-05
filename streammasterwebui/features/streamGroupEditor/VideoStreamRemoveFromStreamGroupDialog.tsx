import XButton from '@/components/buttons/XButton'
import {
  StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiArg,
  VideoStreamDto,
  useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteMutation,
} from '@/lib/iptvApi'
import { useSelectedStreamGroup } from '@/lib/redux/slices/useSelectedStreamGroup'
import { memo } from 'react'

type VideoStreamRemoveFromStreamGroupDialogProps = {
  readonly id: string
  readonly value?: VideoStreamDto | undefined
}

const VideoStreamRemoveFromStreamGroupDialog = ({
  id,
  value,
}: VideoStreamRemoveFromStreamGroupDialogProps) => {
  const [streamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation] =
    useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteMutation()
  const { selectedStreamGroup } = useSelectedStreamGroup(id)

  const removeVideoStream = async () => {
    if (!value || !selectedStreamGroup) {
      return
    }

    const toSend =
      {} as StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiArg

    toSend.streamGroupId = selectedStreamGroup.id
    toSend.videoStreamId = value.id

    await streamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation(
      toSend,
    )
      .then(() => {})
      .catch((error) => {
        console.error('Remove Stream Error: ' + error.message)
      })
  }

  return (
    <div className="flex">
      <XButton
        iconFilled={false}
        onClick={async () => await removeVideoStream()}
      />
    </div>
  )
}

VideoStreamRemoveFromStreamGroupDialog.displayName =
  'VideoStreamRemoveFromStreamGroupDialog'

export default memo(VideoStreamRemoveFromStreamGroupDialog)
