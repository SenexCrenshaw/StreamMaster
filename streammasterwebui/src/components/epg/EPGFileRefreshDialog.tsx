import { memo, useState } from 'react'

import {
  useEpgFilesRefreshEpgFileMutation,
  type EpgFileDto,
  type EpgFilesRefreshEpgFileApiArg,
} from '@/lib/iptvApi'
import FileRefreshDialog from '../sharedEPGM3U/FileRefreshDialog'

type EPGFileRefreshDialogProps = {
  readonly selectedFile: EpgFileDto
}

const EPGFileRefreshDialog = ({ selectedFile }: EPGFileRefreshDialogProps) => {
  const [infoMessage, setInfoMessage] = useState('')
  const [epgFilesRefreshEpgFileMutation] = useEpgFilesRefreshEpgFileMutation()

  const refreshFile = async () => {
    if (!selectedFile?.id) {
      return
    }

    const toSend = {} as EpgFilesRefreshEpgFileApiArg
    toSend.id = selectedFile.id

    epgFilesRefreshEpgFileMutation(toSend)
      .then(() => {
        setInfoMessage('EPG Refresh Successfully')
      })
      .catch((e) => {
        setInfoMessage('EPG Refresh Error: ' + e.message)
      })
  }

  return (
    <FileRefreshDialog
      fileType="epg"
      inputInfoMessage={infoMessage}
      onRefreshFile={refreshFile}
    />
  )
}

EPGFileRefreshDialog.displayName = 'EPGFileRefreshDialog'

export default memo(EPGFileRefreshDialog)
