import StandardHeader from '@components/StandardHeader';
import EPGFilesEditor from '@components/epg/EPGFilesEditor';
import M3UFilesEditor2 from '@components/m3u/M3UFilesEditor2';
import { FilesEditorIcon } from '@lib/common/icons';
import React from 'react';

const FilesEditor = () => (
  <StandardHeader className="filesEditor flex flex-column h-full" displayName="FILES" icon={<FilesEditorIcon />}>
    <M3UFilesEditor2 />

    <div style={{ backgroundColor: 'var(--surface-ground)', height: '4rem' }} />

    <EPGFilesEditor />
  </StandardHeader>
);

FilesEditor.displayName = 'FilesEditor';

export default React.memo(FilesEditor);
