import { useAppSelector } from '@lib/redux/hooks';
import { useSMMessages } from '@lib/redux/hooks/useSMMessages';
import { Toast } from 'primereact/toast';
import { useEffect, useRef } from 'react';

export const MessageProcessor = ({ children }: React.PropsWithChildren): JSX.Element => {
  const toast = useRef<Toast>(null);
  const smMessages = useAppSelector((state) => state.messages);

  const { ClearMessages } = useSMMessages();

  useEffect(() => {
    if (smMessages.length === 0) return;

    smMessages.forEach((message) => {
      toast?.current?.show({ severity: message.Severity, summary: message.Summary, detail: message.Detail });
    });

    ClearMessages();
  }, [ClearMessages, smMessages]);

  return (
    <>
      <Toast ref={toast} position="bottom-right" />
      {children}
    </>
  );
};
