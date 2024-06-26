import { useSMMessages } from '@lib/redux/hooks/useSMMessages';
import { Toast } from 'primereact/toast';
import { useEffect, useRef } from 'react';
import { useSMContext } from './SMProvider';

export const MessageProcessor = ({ children }: React.PropsWithChildren): JSX.Element => {
  const { isSystemReady } = useSMContext();

  const toast = useRef<Toast>(null);
  const { messages, ClearMessages } = useSMMessages();

  useEffect(() => {
    if (messages === undefined || !Array.isArray(messages) || messages.length === 0) return;
    if (isSystemReady === undefined || isSystemReady === false) {
      return;
      //ClearMessages();
    }
    messages.forEach((message) => {
      toast?.current?.show({ detail: message.Detail, severity: message.Severity, summary: message.Summary });
    });

    ClearMessages();
  }, [ClearMessages, isSystemReady, messages]);

  return (
    <>
      <Toast ref={toast} position="bottom-right" />
      {children}
    </>
  );
};
