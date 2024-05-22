import { useSMMessages } from '@lib/redux/hooks/useSMMessages';
import { useSMContext } from '@lib/signalr/SMProvider';
import { useEffect, useState } from 'react';
import './ball-spin-clockwise-fade.css';

const SMLoader = () => {
  const { isSystemReady } = useSMContext();

  const [message, setMessage] = useState<string>('Loading...'); // [ messages, setMessages
  const { messages, ClearMessages } = useSMMessages();

  useEffect(() => {
    if (messages === undefined || !Array.isArray(messages) || messages.length === 0) return;

    if (isSystemReady === true) {
      return;
    }

    setMessage(messages[messages.length - 1].Detail ?? '');

    ClearMessages();
  }, [ClearMessages, isSystemReady, messages]);

  return (
    <div className="sm-loader flex flex-column h-full justify-content-center ">
      <div className="">
        {/* <div className="la-ball-fussion la-lg">
          <div></div>
          <div></div>
          <div></div>
          <div></div>
        </div> */}
        <div className="la-ball-spin-clockwise-fade la-3x">
          <div></div>
          <div></div>
          <div></div>
          <div></div>
          <div></div>
          <div></div>
          <div></div>
          <div></div>
        </div>
      </div>
      <div className="pt-8 m-0 ">
        <div className="flex align-items-center bg-black-alpha-70 h-3rem px-4 border-round-md">
          <h3>{message}</h3>
        </div>
      </div>
    </div>
  );
};

export default SMLoader;
