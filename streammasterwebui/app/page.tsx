import { redirect } from 'next/navigation';

export default function IndexPage() {
  redirect('/editor/playlist');
}

export const metadata = {
  title: 'Stream Master',
};
