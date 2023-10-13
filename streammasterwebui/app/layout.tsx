import { Providers } from '@/lib/providers';
import '@lib/styles/dataSelector.css';
import '@lib/styles/index.css';
import '@lib/styles/theme.css'; // theme
import { Albert_Sans } from 'next/font/google';
import 'primeflex/primeflex.css'; // css utility
import 'primeicons/primeicons.css'; // icons
import 'primereact/resources/primereact.css'; // core css
import 'primereact/resources/themes/viva-dark/theme.css'; // theme

const albert_sans = Albert_Sans({
  subsets: ['latin'],
  display: 'swap',
});

export default function RootLayout(props: React.PropsWithChildren) {
  return (
    <html lang="en" className={albert_sans.className}>
      <body>
        <Providers>{props.children}</Providers>
      </body>
    </html>
  );
}
