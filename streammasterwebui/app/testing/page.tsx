'use client';

import dynamic from 'next/dynamic';
const StreamGroupSelectedVideoStreamDataSelector = dynamic(() => import('@/features/streamGroupEditor/StreamGroupSelectedVideoStreamDataSelector'), {
  ssr: false,
});

export default function StreamGroupSelectedVideoStreamDataSelectorLayout() {
  const id = 'streamgroupeditor';
  return <StreamGroupSelectedVideoStreamDataSelector id={id} />;
}
