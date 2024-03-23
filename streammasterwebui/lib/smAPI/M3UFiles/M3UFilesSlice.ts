import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData,PagedResponse, removeKeyFromData, M3UFileDto } from '@lib/apiDefs';
import { fetchGetPagedM3UFiles } from '@lib/smAPI/M3UFiles/M3UFilesFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: Record<string, PagedResponse<M3UFileDto> | undefined>;
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
const M3UFilesSlice = createSlice({
  name: 'M3UFiles',
  initialState,
  reducers: {
    updateM3UFiles: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
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
      console.log('updateM3UFiles executed');
    },
    clearM3UFiles: (state) => {
      for (const key in state.data) {
        const updatedData = removeKeyFromData(state.data, key);
        state.data = updatedData;
      }
      console.log('clearM3UFiles executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedM3UFiles.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false; // Reset isError state on new fetch
      })
      .addCase(fetchGetPagedM3UFiles.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
        }
      })
      .addCase(fetchGetPagedM3UFiles.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
      });

  }
});

export const { clearM3UFiles, updateM3UFiles } = M3UFilesSlice.actions;
export default M3UFilesSlice.reducer;
