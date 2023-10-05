import EPGFilesEditor from '@/components/epg/EPGFilesEditor'
import M3UFilesEditor from '@/components/m3u/M3UFilesEditor'
import { FilesEditorIcon } from '@/lib/common/icons'
import React from 'react'

const FilesEditor = () => {
  return (
    <div className="playListEditor">
      <div className="grid grid-nogutter flex justify-content-between align-items-center">
        <div className="flex w-full text-left ml-1 font-bold text-white-500 surface-overlay justify-content-start align-items-center">
          <FilesEditorIcon className="p-0 mr-2" />
          FILES
        </div>
        <M3UFilesEditor />

        <EPGFilesEditor />
      </div>
    </div>
  )
}

FilesEditor.displayName = 'FilesEditor'

export default React.memo(FilesEditor)
