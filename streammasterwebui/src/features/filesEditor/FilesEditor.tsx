
import React from 'react';

import EPGFilesEditor from '../../components/EPGFilesEditor';
import M3UFilesEditor from '../../components/M3UFilesEditor';

const FilesEditor = () => {

  return (
    <>
      <M3UFilesEditor
        onClick={() => { }}
      />
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
