import { useSMMessages } from '@lib/redux/hooks/useSMMessages';
import { useSMContext } from '@lib/signalr/SMProvider';
import { useEffect, useState } from 'react';
import './ball-grid-beat.css';

const SMLoader = () => {
  const { isSystemReady } = useSMContext();

  const [message, setMessage] = useState<string>(''); // [ messages, setMessages
  const { messages, ClearMessages } = useSMMessages();

  useEffect(() => {
    if (messages.length === 0) return;

    if (isSystemReady === true) {
      return;
    }

    // console.log(messages[messages.length - 1].Detail);
    setMessage(messages[messages.length - 1].Detail ?? '');

    ClearMessages();
  }, [ClearMessages, isSystemReady, messages]);

  return (
    <div className="sm-loader flex-column">
      <div className="la-ball-grid-beat">
        <div />
        <div />
        <div />
        <div />
        <div />
        <div />
        <div />
        <div />
        <div />
      </div>
      <h2 className=" p-0 m-0">{message}</h2>
    </div>
  );
};

export default SMLoader;
