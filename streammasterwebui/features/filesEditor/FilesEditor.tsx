
import EPGFilesEditor from '@/src/components/epg/EPGFilesEditor';
import M3UFilesEditor from '@/src/components/m3u/M3UFilesEditor';
import React from 'react';

const FilesEditor = () => {

  return (
    <>
      <M3UFilesEditor />

      <EPGFilesEditor />
    </>
  );
}

FilesEditor.displayName = 'FilesEditor';

export default React.memo(FilesEditor);
