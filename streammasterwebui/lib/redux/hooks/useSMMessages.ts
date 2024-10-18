import { SMMessage } from '@lib/smAPI/smapiTypes';
import { useAppDispatch, useAppSelector } from '../hooks';
import { addMessage, clearMessages } from './messages';

interface SMMessageResult {
  messages: SMMessage[];
  AddMessage: (message: SMMessage) => void;
  ClearMessages: () => void;
}

export const useSMMessages = (): SMMessageResult => {
  const dispatch = useAppDispatch();

  const messages = useAppSelector((state) => state.messages);

  const AddMessage = (message: SMMessage): void => {
    dispatch(addMessage(message));
  };

  const ClearMessages = (): void => {
    dispatch(clearMessages());
  };

  return { AddMessage, ClearMessages, messages };
};
