import { isClient } from '@/lib/settings'
import { redirect } from 'next/navigation'

export default function IndexPage() {
  if (isClient) {
    console.log(window.location.pathname)
  } else {
    console.log('Server')
  }

  redirect('/editor/playlist')
}

export const metadata = {
  title: 'Stream Master',
}
