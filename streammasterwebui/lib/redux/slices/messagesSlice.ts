import { SMMessage } from '@lib/signalr/SMMessage';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { useAppDispatch, useAppSelector } from '../hooks';

const initialState: SMMessage[] = [];

const messagesSlice = createSlice({
  name: 'messages',
  initialState,
  reducers: {
    addMessage: (state, action: PayloadAction<SMMessage>) => {
      if (action.payload) {
        state.push(action.payload);
      }
    },
    clearMessages: (state) => {
      return initialState;
    }
  }
});

export const { addMessage, clearMessages } = messagesSlice.actions;

interface SMMessageResult extends SMMessage {
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

  return { messages, AddMessage, ClearMessages };
};

export default messagesSlice.reducer;
