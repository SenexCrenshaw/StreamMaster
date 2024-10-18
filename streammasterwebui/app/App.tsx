import { useLocalStorage } from 'primereact/hooks';

import MessagesEn from '@lib/locales/MessagesEn';

import { SMProvider } from '@lib/context/SMProvider';
import { MessageProcessor } from '@lib/signalr/MessageProcessor';
import { SignalRProvider } from '@lib/signalr/SignalRProvider';
import { IntlProvider } from 'react-intl';
import Router from './Router';

const App = (): JSX.Element => {
  const [locale] = useLocalStorage('en', 'locale');
  const messages = locale === 'en' ? MessagesEn : MessagesEn;
  return (
    <SMProvider>
      <IntlProvider locale={locale} messages={messages}>
        <SignalRProvider>
          <MessageProcessor>
            <Router />
          </MessageProcessor>
        </SignalRProvider>
      </IntlProvider>
    </SMProvider>
  );
};

export default App;
