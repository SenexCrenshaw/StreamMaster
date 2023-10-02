import { redirect } from 'next/navigation';

export default function IndexPage() {
    redirect("/editor/playlist");
    return (<div />);
}

export const metadata = {
    title: 'Stream Master',
}
