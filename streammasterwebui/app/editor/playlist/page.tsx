import React from 'react';

const PlayListEditor = React.lazy(() => import('@/features/playListEditor/PlayListEditor'));

export default function PlayListEditorLayout() {
  return <PlayListEditor />;
}
