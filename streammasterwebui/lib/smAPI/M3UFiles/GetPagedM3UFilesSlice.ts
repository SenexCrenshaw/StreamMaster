import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, M3UFileDto,PagedResponse } from '@lib/smAPI/smapiTypes';
import {removeKeyFromData} from '@lib/apiDefs';
import { fetchGetPagedM3UFiles } from '@lib/smAPI/M3UFiles/M3UFilesFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: Record<string, PagedResponse<M3UFileDto> | undefined>;
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
const getPagedM3UFilesSlice = createSlice({
  name: 'GetPagedM3UFiles',
  initialState,
  reducers: {
    updateGetPagedM3UFiles: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
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
      console.log('updateGetPagedM3UFiles executed');
    },
    clearGetPagedM3UFiles: (state) => {
        state.data = {};
        state.error = {};
        state.isError = {};
        state.isLoading = {};
      console.log('clearGetPagedM3UFiles executed');
    },
    intSetGetPagedM3UFilesIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       for (const key in state.data) { state.isLoading[key] = action.payload.isLoading; }
      console.log('setGetPagedM3UFilesIsLoading executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedM3UFiles.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false;
        state.error[query] = undefined;
      })
      .addCase(fetchGetPagedM3UFiles.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
          state.error[query] = undefined;
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

export const { clearGetPagedM3UFiles, intSetGetPagedM3UFilesIsLoading, updateGetPagedM3UFiles } = getPagedM3UFilesSlice.actions;
export default getPagedM3UFilesSlice.reducer;
