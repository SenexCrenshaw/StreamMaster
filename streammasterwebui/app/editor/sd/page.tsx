'use client'

import dynamic from 'next/dynamic'

const SDEditor = dynamic(() => import('@/features/sdEditor/SDEditor'), {
  ssr: false,
})

export default function SDEditorLayout() {
  return <SDEditor />
}
