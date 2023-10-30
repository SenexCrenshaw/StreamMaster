import React from 'react';

const FilesEditor = React.lazy(() => import('@/features/filesEditor/FilesEditor'));

export default function FilesEditorLayout() {
  return <FilesEditor />;
}
