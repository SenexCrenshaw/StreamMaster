import { SMMessage } from '@lib/smAPI/smapiTypes';
import { PayloadAction, createSlice } from '@reduxjs/toolkit';

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
    addError: (state, action: PayloadAction<string>) => {
      if (action.payload) {
        state.push({ Summary: action.payload, Severity: 'error' } as SMMessage);
      }
    },
    addErrorWithDetail: (state, action: PayloadAction<SMMessage>) => {
      if (action.payload) {
        state.push({ Summary: action.payload.Summary, Detail: action.payload.Detail, Severity: 'error' } as SMMessage);
      }
    },
    clearMessages: (state) => {
      return initialState;
    }
  }
});

export const { addMessage, addError, addErrorWithDetail, clearMessages } = messagesSlice.actions;

export default messagesSlice.reducer;
