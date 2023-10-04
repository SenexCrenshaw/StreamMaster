import {
  type VideoStreamDto,
  type VideoStreamsSetVideoStreamsLogoFromEpgApiArg,
  type VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiArg,
} from '@/lib/iptvApi'
import { memo, useCallback, useMemo, useState } from 'react'
import { useQueryFilter } from '../../../lib/redux/slices/useQueryFilter'
import { useSelectAll } from '../../../lib/redux/slices/useSelectAll'
import { useSelectedVideoStreams } from '../../../lib/redux/slices/useSelectedVideoStreams'

import {
  SetVideoStreamsLogoFromEpg,
  SetVideoStreamsLogoFromEpgFromParameters,
} from '@/lib/smAPI/VideoStreams/VideoStreamsMutateAPI'
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog'
import ImageButton from '../buttons/ImageButton'
import OKButton from '../buttons/OKButton'

type VideoStreamSetLogosFromEPGDialogProps = {
  readonly id: string
}

const VideoStreamSetLogosFromEPGDialog = ({
  id,
}: VideoStreamSetLogosFromEPGDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false)
  const [infoMessage, setInfoMessage] = useState('')
  const [block, setBlock] = useState<boolean>(false)

  const { selectedVideoStreams } = useSelectedVideoStreams(id)
  const { selectAll } = useSelectAll(id)
  const { queryFilter } = useQueryFilter(id)

  const ReturnToParent = () => {
    setShowOverlay(false)
    setInfoMessage('')
    setBlock(false)
  }

  const onSetLogoSave = useCallback(async () => {
    setBlock(true)

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent()

        return
      }

      const toSendAll =
        {} as VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiArg

      toSendAll.parameters = queryFilter

      SetVideoStreamsLogoFromEpgFromParameters(toSendAll)
        .then(() => {
          setInfoMessage('Set Streams Successfully')
        })
        .catch((error) => {
          setInfoMessage('Set Streams Error: ' + error.message)
        })

      return
    }

    const ids = [
      ...new Set(selectedVideoStreams.map((item: VideoStreamDto) => item.id)),
    ] as string[]

    const toSend = {} as VideoStreamsSetVideoStreamsLogoFromEpgApiArg

    toSend.ids = ids

    const max = 500

    let count = 0

    const promises = []

    while (count < ids.length) {
      if (count + max < ids.length) {
        toSend.ids = ids.slice(count, count + max)
      } else {
        toSend.ids = ids.slice(count, ids.length)
      }

      count += max
      promises.push(
        SetVideoStreamsLogoFromEpg(toSend)
          .then(() => {})
          .catch(() => {}),
      )
    }

    const p = Promise.all(promises)

    await p
      .then(() => {
        setInfoMessage('Successful')
      })
      .catch((error) => {
        setInfoMessage('Error: ' + error.message)
      })
  }, [queryFilter, selectAll, selectedVideoStreams])

  const getTotalCount = useMemo(() => {
    return selectedVideoStreams.length
  }, [selectedVideoStreams])

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Set Logo From EPG"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent()
        }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full align-items-center h-full">
          <OKButton
            label="Set Logo From EPG"
            onClick={async () => await onSetLogoSave()}
          />
        </div>
      </InfoMessageOverLayDialog>

      <ImageButton
        disabled={getTotalCount === 0 && !selectAll}
        onClick={() => setShowOverlay(true)}
        tooltip="Set Logo from EPG Streams"
      />
    </>
  )
}

VideoStreamSetLogosFromEPGDialog.displayName =
  'VideoStreamSetLogosFromEPGDialog'

export default memo(VideoStreamSetLogosFromEPGDialog)
