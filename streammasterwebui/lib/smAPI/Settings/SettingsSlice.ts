import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData,PagedResponse, removeKeyFromData, SDSystemStatus } from '@lib/apiDefs';
import { fetchGetSystemStatus } from '@lib/smAPI/Settings/SettingsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: Record<string, PagedResponse<SDSystemStatus> | undefined>;
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
const SettingsSlice = createSlice({
  name: 'Settings',
  initialState,
  reducers: {
    updateSettings: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
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
      console.log('updateSettings executed');
    },
    clearSettings: (state) => {
      for (const key in state.data) {
        const updatedData = removeKeyFromData(state.data, key);
        state.data = updatedData;
      }
      console.log('clearSettings executed');
    },
    intSetSettingsIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       for (const key in state.data) { state.isLoading[key] = action.payload.isLoading; }
      console.log('setSettingsIsLoading executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetSystemStatus.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false; // Reset isError state on new fetch
      })
      .addCase(fetchGetSystemStatus.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
        }
      })
      .addCase(fetchGetSystemStatus.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
      });

  }
});

export const { intSetSettingsIsLoading, clearSettings, updateSettings } = SettingsSlice.actions;
export default SettingsSlice.reducer;
