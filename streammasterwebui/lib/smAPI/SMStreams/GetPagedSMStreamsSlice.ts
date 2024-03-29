import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, SMStreamDto,PagedResponse } from '@lib/smAPI/smapiTypes';
import {removeKeyFromData} from '@lib/apiDefs';
import { fetchGetPagedSMStreams } from '@lib/smAPI/SMStreams/SMStreamsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: Record<string, PagedResponse<SMStreamDto> | undefined>;
  isLoading: Record<string, boolean>;
  isError: Record<string, boolean>;
  error: Record<string, string | undefined>;
}

const initialState: QueryState = {
  data: {},
  isLoading: {},
  isError: {},
  error: {}
};
const getPagedSMStreamsSlice = createSlice({
  name: 'GetPagedSMStreams',
  initialState,
  reducers: {
    updateGetPagedSMStreams: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
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
      console.log('updateGetPagedSMStreams executed');
    },
    clearGetPagedSMStreams: (state) => {
      for (const key in state.data) {
        const updatedData = removeKeyFromData(state.data, key);
        state.data = updatedData;
      }
      console.log('clearGetPagedSMStreams executed');
    },
    intSetGetPagedSMStreamsIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       for (const key in state.data) { state.isLoading[key] = action.payload.isLoading; }
      console.log('setGetPagedSMStreamsIsLoading executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedSMStreams.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false;
        state.error[query] = undefined;
      })
      .addCase(fetchGetPagedSMStreams.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
          state.error[query] = undefined;
        }
      })
      .addCase(fetchGetPagedSMStreams.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
      });

  }
});

export const { clearGetPagedSMStreams, intSetGetPagedSMStreamsIsLoading, updateGetPagedSMStreams } = getPagedSMStreamsSlice.actions;
export default getPagedSMStreamsSlice.reducer;
