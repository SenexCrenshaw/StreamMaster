import { Providers } from "@/lib/providers";
import { Albert_Sans, } from 'next/font/google';

import '@/lib/styles/theme.css'; // theme
import 'primeflex/primeflex.css'; // css utility
import 'primeicons/primeicons.css'; // icons
import 'primereact/resources/primereact.css'; // core css
import 'primereact/resources/themes/viva-dark/theme.css'; // theme

import '@/lib/styles/index.css';

const albert_sans = Albert_Sans({
    subsets: ['latin'],
    display: 'swap',
})


export default function RootLayout(props: React.PropsWithChildren) {
    return (
        <html lang="en" className={albert_sans.className}>
            <body>
                <Providers>
                    {props.children}
                </Providers>
            </body>
        </html>
    );
}