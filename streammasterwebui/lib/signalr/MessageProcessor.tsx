import { useEffect, useRef } from 'react';
import { Toast } from 'primereact/toast';
import { useAppSelector } from '@lib/redux/hooks';
import { useSMMessages } from '@lib/redux/slices/messagesSlice';

export const MessageProcessor = ({ children }: React.PropsWithChildren): JSX.Element => {
  const toast = useRef<Toast>(null);
  const smMessages = useAppSelector((state) => state.messages);

  const { ClearMessages } = useSMMessages();
  useEffect(() => {
    if (smMessages.length === 0) return;

    smMessages.forEach((message) => {
      toast?.current?.show({ severity: message.severity, summary: message.summary, detail: message.detail, life: message.life });
    });

    ClearMessages();
  }, [smMessages]);

  return (
    <>
      <Toast ref={toast} position="bottom-right" />
      {children}
    </>
  );
};
