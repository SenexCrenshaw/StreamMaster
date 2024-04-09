import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { fetchGetSMChannelStreams } from '@lib/smAPI/SMChannelStreamLinks/GetSMChannelStreamsFetch';


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
  initialState,
  name: 'GetSMChannelStreams',
  reducers: {
    clear: (state) => {
      state = initialState;
      console.log('GetSMChannelStreams clear');
    },
    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;

      if (fieldData.Entity !== undefined && state.data[fieldData.Id]) {
        state.data[fieldData.Id] = fieldData.Value;
        return;
      }
      console.log('GetSMChannelStreams setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetSMChannelStreams  setIsForced ', force);
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
