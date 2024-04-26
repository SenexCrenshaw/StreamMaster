import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, EPGFileDto,PagedResponse } from '@lib/smAPI/smapiTypes';
import { fetchGetPagedEPGFiles } from '@lib/smAPI/EPGFiles/GetPagedEPGFilesFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: Record<string, PagedResponse<EPGFileDto> | undefined>;
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

const getPagedEPGFilesSlice = createSlice({
  initialState,
  name: 'GetPagedEPGFiles',
  reducers: {
    clear: (state) => {
      state = initialState;
      console.log('GetPagedEPGFiles clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      const tag = action.payload.tag;
      for (const key in state.data) {
        if (key.includes(tag)) {
          state.data[key] = undefined;
        }
      }
      console.log('GetPagedEPGFiles clearByTag');
    },

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
      console.log('GetPagedEPGFiles setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetPagedEPGFiles  setIsForced ', force);
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
      console.log('GetPagedEPGFiles setIsLoading ', action.payload.isLoading);
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedEPGFiles.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false;
        state.isForced = false;
        state.error[query] = undefined;
      })
      .addCase(fetchGetPagedEPGFiles.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          setIsLoading({ isLoading: false, query: query });
          state.isLoading[query] = false;
          state.isError[query] = false;
          state.error[query] = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetPagedEPGFiles.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
         setIsLoading({ isLoading: false, query: query });;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getPagedEPGFilesSlice.actions;
export default getPagedEPGFilesSlice.reducer;
