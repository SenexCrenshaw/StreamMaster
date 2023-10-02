import { Providers } from "@/lib/providers";

import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

import '@/lib/styles/theme.css'; // theme
import 'primeflex/primeflex.css'; // css utility
import 'primeicons/primeicons.css'; // icons
import 'primereact/resources/primereact.css'; // core css
import 'primereact/resources/themes/viva-dark/theme.css'; // theme

import '@/lib/styles/index.css';

export default function RootLayout(props: React.PropsWithChildren) {
    return (
        <html lang="en">
            <body>
                <Providers>
                    {props.children}
                </Providers>
            </body>
        </html>
    );
}