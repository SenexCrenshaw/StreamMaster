import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, ChannelGroupDto,PagedResponse } from '@lib/smAPI/smapiTypes';
import {removeKeyFromData} from '@lib/apiDefs';
import { fetchGetPagedChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: Record<string, PagedResponse<ChannelGroupDto> | undefined>;
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
const getPagedChannelGroupsSlice = createSlice({
  name: 'GetPagedChannelGroups',
  initialState,
  reducers: {
    updateGetPagedChannelGroups: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
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
      console.log('updateGetPagedChannelGroups executed');
    },
    clearGetPagedChannelGroups: (state) => {
        state.data = {};
        state.error = {};
        state.isError = {};
        state.isLoading = {};
      console.log('clearGetPagedChannelGroups executed');
    },
    intSetGetPagedChannelGroupsIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       for (const key in state.data) { state.isLoading[key] = action.payload.isLoading; }
      console.log('setGetPagedChannelGroupsIsLoading executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedChannelGroups.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false;
        state.error[query] = undefined;
      })
      .addCase(fetchGetPagedChannelGroups.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
          state.error[query] = undefined;
        }
      })
      .addCase(fetchGetPagedChannelGroups.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
      });

  }
});

export const { clearGetPagedChannelGroups, intSetGetPagedChannelGroupsIsLoading, updateGetPagedChannelGroups } = getPagedChannelGroupsSlice.actions;
export default getPagedChannelGroupsSlice.reducer;
