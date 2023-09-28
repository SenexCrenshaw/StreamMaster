'use client'
import { SideBar } from '@/app/sideBar';
import { useLocalStorage } from 'primereact/hooks';
import react from 'react';
import { IntlProvider } from 'react-intl';
import { ProSidebarProvider } from 'react-pro-sidebar';
import { Provider } from 'react-redux';
import messages_en from './locales/messages_en';
import { store } from './redux/store';
import { SignalRConnection } from './signalr/SignalRConnection';

export const Providers = (props: React.PropsWithChildren) => {
  const [locale,] = useLocalStorage('en', 'locale');
  const messages = locale === 'en' ? messages_en : messages_en;

  return (
    <react.StrictMode>
      <IntlProvider locale={locale} messages={messages}>
        <Provider store={store}>
          <SignalRConnection>
            <ProSidebarProvider>
              <SideBar>
                {props.children}
              </SideBar>
            </ProSidebarProvider>
          </SignalRConnection>
        </Provider>
      </IntlProvider>
    </react.StrictMode>
  );
}
