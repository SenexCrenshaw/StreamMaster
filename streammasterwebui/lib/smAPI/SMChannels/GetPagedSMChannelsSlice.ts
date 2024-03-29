import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { FieldData, SMChannelDto, PagedResponse } from '@lib/smAPI/smapiTypes';
import { removeKeyFromData } from '@lib/apiDefs';
import { fetchGetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';

interface QueryState {
  data: Record<string, PagedResponse<SMChannelDto> | undefined>;
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
const getPagedSMChannelsSlice = createSlice({
  name: 'GetPagedSMChannels',
  initialState,
  reducers: {
    updateGetPagedSMChannels: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
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
      console.log('updateGetPagedSMChannels executed');
    },
    clearGetPagedSMChannels: (state) => {
      for (const key in state.data) {
        state.data[key] = undefined;
        state.error[key] = undefined;
        state.isError[key] = false;
        state.isLoading[key] = false;
      }
      console.log('clearGetPagedSMChannels executed');
    },
    intSetGetPagedSMChannelsIsLoading: (state, action: PayloadAction<{ isLoading: boolean }>) => {
      for (const key in state.data) {
        state.isLoading[key] = action.payload.isLoading;
      }
      console.log('setGetPagedSMChannelsIsLoading executed');
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedSMChannels.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false;
        state.error[query] = undefined;
      })
      .addCase(fetchGetPagedSMChannels.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
          state.error[query] = undefined;
        }
      })
      .addCase(fetchGetPagedSMChannels.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
      });
  }
});

export const { clearGetPagedSMChannels, intSetGetPagedSMChannelsIsLoading, updateGetPagedSMChannels } = getPagedSMChannelsSlice.actions;
export default getPagedSMChannelsSlice.reducer;
