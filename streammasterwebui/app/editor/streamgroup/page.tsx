'use client'

import dynamic from 'next/dynamic'

const StreamGroupEditor = dynamic(
  () => import('@/features/streamGroupEditor/StreamGroupEditor'),
  { ssr: false },
)

export default function StreamGroupEditorLayout() {
  return <StreamGroupEditor />
}
