import { useSMMessages } from '@lib/redux/hooks/useSMMessages';
import { useSMContext } from '@lib/signalr/SMProvider';
import { useEffect, useState } from 'react';
import './ball-fussion.css';
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
    <div className="absolute sm-loader flex-column">
      {/* <div className="absolute la-ball-grid-beat top-50 left-50">
        <div />
        <div />
        <div />
        <div />
        <div />
        <div />
        <div />
        <div />
        <div />
      </div> */}
      <div className="absolute la-ball-fussion la-lg top-50 left-50">
        <div></div>
        <div></div>
        <div></div>
        <div></div>
      </div>
      <h2 className=" p-0 m-0">{message}</h2>
    </div>
  );
};

export default SMLoader;
