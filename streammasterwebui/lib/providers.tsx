'use client'

import { SideBar } from '@/app/SideBar';
import { useLocalStorage } from 'primereact/hooks';
import react, { useEffect } from 'react';
import { IntlProvider } from 'react-intl';
import { ProSidebarProvider } from 'react-pro-sidebar';
import { Provider } from 'react-redux';
import { persistStore } from 'redux-persist';
import { PersistGate } from 'redux-persist/integration/react';
import messages_en from './locales/messages_en';
import { useStore } from './redux/store';
import { SignalRConnection } from './signalr/SignalRConnection';
import { baseHostURL } from '@/lib/settings';

export const Providers = (props: React.PropsWithChildren) => {
  const [locale,] = useLocalStorage('en', 'locale');
  const messages = locale === 'en' ? messages_en : messages_en;
  const store = useStore();
  
useEffect(() => {
    const script = document.createElement('script');
    script.src = baseHostURL+'/initialize.js';
    script.onload = () => {
    
    };
    document.body.appendChild(script);
  }, []);
  
  if (store) {
    const persistor = persistStore(store, {}, function () {
      persistor.persist();
    });
    return (
      <react.StrictMode>
        <IntlProvider locale={locale} messages={messages}>
          <Provider store={store}>
            <PersistGate persistor={persistor} />
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
