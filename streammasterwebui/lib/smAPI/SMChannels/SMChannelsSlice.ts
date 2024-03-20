import { FieldData, PagedResponse, SMChannelDto, removeKeyFromData } from '@lib/apiDefs';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';
import { fetchGetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsFetch';
import { PayloadAction, createSlice } from '@reduxjs/toolkit';

interface QueryState {
  data: Record<string, PagedResponse<SMChannelDto> | undefined>;
  isLoading: Record<string, boolean>;
  isError: Record<string, boolean>;
  error: Record<string, string>;
}

const initialState: QueryState = {
  data: {},
  isLoading: {},
  isError: {},
  error: {}
};
const SMChannelsSlice = createSlice({
  name: 'SMChannels',
  initialState,
  reducers: {
    updateSMChannels: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
      const { query, fieldData } = action.payload;

      if (query !== undefined) {
        // Update a specific query's data if it exists
        if (state.data[query]) {
          state.data[query] = updatePagedResponseFieldInData(state.data[query], fieldData);
        }
        return;
      }

      // Fallback: update all queries' data if query is undefined
      for (const key in state.data) {
        if (state.data[key]) {
          state.data[key] = updatePagedResponseFieldInData(state.data[key], fieldData);
        }
      }
      console.log('updateSMChannels executed');
    },
    clearSMChannels: (state) => {
      for (const key in state.data) {
        const updatedData = removeKeyFromData(state.data, key);
        state.data = updatedData;
      }
      console.log('clearSMChannels executed');
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedSMChannels.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false; // Reset isError state on new fetch
      })
      .addCase(fetchGetPagedSMChannels.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
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

export const { clearSMChannels, updateSMChannels } = SMChannelsSlice.actions;
export default SMChannelsSlice.reducer;
