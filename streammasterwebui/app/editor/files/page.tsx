'use client'

import dynamic from 'next/dynamic'

const FilesEditor = dynamic(
  () => import('@/features/filesEditor/FilesEditor'),
  { ssr: false },
)

export default function FilesEditorLayout() {
  return <FilesEditor />
}
