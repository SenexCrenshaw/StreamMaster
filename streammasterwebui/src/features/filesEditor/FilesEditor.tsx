
import React from 'react';
import EPGFilesEditor from '../../components/epg/EPGFilesEditor';
import M3UFilesEditor from '../../components/m3u/M3UFilesEditor';

const FilesEditor = () => {

  return (
    <>
      <M3UFilesEditor />
      <div className="my-1" />
      <EPGFilesEditor />
    </>
  );
}

FilesEditor.displayName = 'FilesEditor';
FilesEditor.defaultProps = {
  onChange: null,
  value: null,
};


export default React.memo(FilesEditor);
