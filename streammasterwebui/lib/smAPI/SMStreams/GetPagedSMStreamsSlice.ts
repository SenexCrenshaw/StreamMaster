import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, SMStreamDto,PagedResponse } from '@lib/smAPI/smapiTypes';
import { fetchGetPagedSMStreams } from '@lib/smAPI/SMStreams/GetPagedSMStreamsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: Record<string, PagedResponse<SMStreamDto> | undefined>;
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
const getPagedSMStreamsSlice = createSlice({
  name: 'GetPagedSMStreams',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
      const { query, fieldData } = action.payload;

      if (query !== undefined) {
        if (state.data[query]) {
          state.data[query] = updatePagedResponseFieldInData(state.data[query], fieldData);
        }
        return;
      }

      for (const key in state.data) {
        if (state.data[key]) {
          state.data[key] = updatePagedResponseFieldInData(state.data[key], fieldData);
        }
      }
      console.log('GetPagedSMStreams setField');
    },
    clear: (state) => {
       state = initialState;
       console.log('GetPagedSMStreams clear');
    },
    setIsLoading: (state, action: PayloadAction<{ query: string; isLoading: boolean }>) => {
      const { query, isLoading } = action.payload;
      if (query !== undefined) {
        state.isLoading[query] = isLoading;
      } else {
        for (const key in state.data) {
          state.isLoading[key] = action.payload.isLoading;
        }
      }
      console.log('GetPagedSMStreams setIsLoading ', action.payload.isLoading);
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetPagedSMStreams  setIsForced ', force);
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedSMStreams.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false;
        state.isForced = false;
        state.error[query] = undefined;
      })
      .addCase(fetchGetPagedSMStreams.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
          state.error[query] = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetPagedSMStreams.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getPagedSMStreamsSlice.actions;
export default getPagedSMStreamsSlice.reducer;
