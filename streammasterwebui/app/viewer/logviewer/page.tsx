'use client';

import dynamic from 'next/dynamic';

const LogViewer = dynamic(() => import('@/features/logViewer/LogViewer'), { ssr: false });

export default function LogViewerLayout() {
  return <LogViewer />;
}
