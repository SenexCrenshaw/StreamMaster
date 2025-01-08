import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import {FieldData, StreamGroupDto,PagedResponse } from '@lib/smAPI/smapiTypes';
import { fetchGetPagedStreamGroups } from '@lib/smAPI/StreamGroups/GetPagedStreamGroupsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: Record<string, PagedResponse<StreamGroupDto> | undefined>;
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

const getPagedStreamGroupsSlice = createSlice({
  initialState,
  name: 'GetPagedStreamGroups',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetPagedStreamGroups clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      const tag = action.payload.tag;
      for (const key in state.data) {
        if (key.includes(tag)) {
          state.data[key] = undefined;
        }
      }
      Logger.debug('GetPagedStreamGroups clearByTag');
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
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;

      const updatedData = { ...state.data };
      for (const key in updatedData) {
        if (updatedData[key]) {
          updatedData[key] = undefined;
        }
      }
      state.data = updatedData;
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
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedStreamGroups.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false;
        state.isForced = false;
        state.error[query] = undefined;
      })
      .addCase(fetchGetPagedStreamGroups.fulfilled, (state, action) => {
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
      .addCase(fetchGetPagedStreamGroups.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
         setIsLoading({ isLoading: false, query: query });;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getPagedStreamGroupsSlice.actions;
export default getPagedStreamGroupsSlice.reducer;
