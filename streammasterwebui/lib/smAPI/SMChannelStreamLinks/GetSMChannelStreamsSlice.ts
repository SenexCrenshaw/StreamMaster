import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { fetchGetSMChannelStreams } from '@lib/smAPI/SMChannelStreamLinks/GetSMChannelStreamsFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: Record<string, SMStreamDto[] | undefined>;
  error: Record<string, string | undefined>;
  isError: Record<string, boolean>;
  isForced: boolean;
  isLoading: Record<string, boolean>;
}

const initialState: QueryState = {
  data: {},
  error: {},
  isError: {},
  isForced: false,
  isLoading: {}
};

const getSMChannelStreamsSlice = createSlice({
  name: 'GetSMChannelStreams',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ param?: string | undefined; fieldData: FieldData }>) => {
      const { param , fieldData } = action.payload;

      if (param !== undefined) {
        const paramString = JSON.stringify(param);
        if (state.data[paramString]) {
          state.data[paramString] = updateFieldInData(state.data[paramString], fieldData);
        }
        return;
      }

      for (const key in state.data) {
        if (state.data[key]) {
          state.data[key] = updateFieldInData(state.data[key], fieldData);
        }
      }
      console.log('GetSMChannelStreams setField');
    },
    clear: (state) => {
       state = initialState;
       console.log('GetSMChannelStreams clear');
    },
    setIsLoading: (state, action: PayloadAction<{ param: string; isLoading: boolean }>) => {
      const { param, isLoading } = action.payload;
      if (param !== undefined) {
        const paramString = JSON.stringify(param);
        state.isLoading[paramString] = isLoading;
      } else {
        for (const key in state.data) {
          state.isLoading[key] = action.payload.isLoading;
        }
      }
      console.log('GetSMChannelStreams setIsLoading ', action.payload.isLoading);
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetSMChannelStreams  setIsForced ', force);
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetSMChannelStreams.pending, (state, action) => {
        const paramString = JSON.stringify(action.meta.arg);
        state.isLoading[paramString] = true;
        state.isError[paramString] = false;
        state.isForced = false;
        state.error[paramString] = undefined;
      })
      .addCase(fetchGetSMChannelStreams.fulfilled, (state, action) => {
        if (action.payload) {
          const { param, value } = action.payload;
          const paramString = JSON.stringify(param);
          state.data[paramString] = value;
          state.isLoading[paramString] = false;
          state.isError[paramString] = false;
          state.error[paramString] = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetSMChannelStreams.rejected, (state, action) => {
        const paramString = JSON.stringify(action.meta.arg);
        state.error[paramString] = action.error.message || 'Failed to fetch';
        state.isError[paramString] = true;
        state.isLoading[paramString] = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getSMChannelStreamsSlice.actions;
export default getSMChannelStreamsSlice.reducer;
