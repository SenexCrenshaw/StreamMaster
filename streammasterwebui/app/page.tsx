import { permanentRedirect } from 'next/navigation'

export default function IndexPage() {
  permanentRedirect('/editor/playlist')
}

export const metadata = {
  title: 'Stream Master',
}
