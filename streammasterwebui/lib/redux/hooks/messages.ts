import { SMMessage } from '@lib/smAPI/smapiTypes';
import { PayloadAction, createSlice } from '@reduxjs/toolkit';

const initialState: SMMessage[] = [];

const messagesSlice = createSlice({
  initialState,
  name: 'messages',
  reducers: {
    addError: (state, action: PayloadAction<string>) => {
      if (action.payload) {
        state.push({ Severity: 'error', Summary: action.payload } as SMMessage);
      }
    },
    addErrorWithDetail: (state, action: PayloadAction<SMMessage>) => {
      if (action.payload) {
        state.push({ Detail: action.payload.Detail, Severity: 'error', Summary: action.payload.Summary } as SMMessage);
      }
    },
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

export const { addMessage, addError, addErrorWithDetail, clearMessages } = messagesSlice.actions;

export default messagesSlice.reducer;
