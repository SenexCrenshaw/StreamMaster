import StandardHeader from '@components/StandardHeader';
import EPGFilesEditor from '@components/epg/EPGFilesEditor';
import M3UFilesEditor from '@components/m3u/M3UFilesEditor';
import { FilesEditorIcon } from '@lib/common/icons';
import React from 'react';

const FilesEditor = () => (
  <StandardHeader className="filesEditor flex flex-column h-full" displayName="FILES" icon={<FilesEditorIcon />}>
    <M3UFilesEditor />

    <div style={{ backgroundColor: 'var(--surface-ground)', height: '4rem' }} />

    <EPGFilesEditor />
  </StandardHeader>
);

FilesEditor.displayName = 'FilesEditor';

export default React.memo(FilesEditor);
