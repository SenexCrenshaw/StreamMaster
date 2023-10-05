'use client'

import dynamic from 'next/dynamic'

const PlayListEditor = dynamic(
  () => import('@/features/playListEditor/PlayListEditor'),
  { ssr: false },
)

export default function PlayListEditorLayout() {
  return <PlayListEditor />
}
