import StandardHeader from '@components/StandardHeader';
import EPGFilesEditor from '@components/epgFiles/EPGFilesEditor';
import { FilesEditorIcon } from '@lib/common/icons';
import React from 'react';

const FilesEditor = () => (
  <StandardHeader className="filesEditor flex flex-column h-full" displayName="FILES" icon={<FilesEditorIcon />}>
    <EPGFilesEditor />
  </StandardHeader>
);

FilesEditor.displayName = 'FilesEditor';

export default React.memo(FilesEditor);
