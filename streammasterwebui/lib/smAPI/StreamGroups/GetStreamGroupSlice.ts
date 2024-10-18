import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import {FieldData, StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { fetchGetStreamGroup } from '@lib/smAPI/StreamGroups/GetStreamGroupFetch';


interface QueryState {
  data: Record<string, StreamGroupDto | undefined>;
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

const getStreamGroupSlice = createSlice({
  initialState,
  name: 'GetStreamGroup',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetStreamGroup clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      const tag = action.payload.tag;
      for (const key in state.data) {
        if (key.includes(tag)) {
          state.data[key] = undefined;
        }
      }
      Logger.debug('GetStreamGroup clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;

      if (fieldData.Entity !== undefined && state.data[fieldData.Id]) {
        state.data[fieldData.Id] = fieldData.Value;
        return;
      }
      Logger.debug('GetStreamGroup setField');
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
      Logger.debug('GetStreamGroup  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{ param: string; isLoading: boolean }>) => {
      const { param, isLoading } = action.payload;
      if (param !== undefined) {
        const paramString = JSON.stringify(param);
        state.isLoading[paramString] = isLoading;
      } else {
        for (const key in state.data) {
          state.isLoading[key] = action.payload.isLoading;
        }
      }
      Logger.debug('GetStreamGroup setIsLoading ', action.payload.isLoading);
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetStreamGroup.pending, (state, action) => {
        const paramString = JSON.stringify(action.meta.arg);
        state.isLoading[paramString] = true;
        state.isError[paramString] = false;
        state.isForced = false;
        state.error[paramString] = undefined;
      })
      .addCase(fetchGetStreamGroup.fulfilled, (state, action) => {
        if (action.payload) {
          const { param, value } = action.payload;
          const paramString = JSON.stringify(param);
          state.data[paramString] = value;
          setIsLoading({ isLoading: false, param: paramString });
          state.isLoading[paramString] = false;
          state.isError[paramString] = false;
          state.error[paramString] = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetStreamGroup.rejected, (state, action) => {
        const paramString = JSON.stringify(action.meta.arg);
        state.error[paramString] = action.error.message || 'Failed to fetch';
        state.isError[paramString] = true;
        setIsLoading({ isLoading: false, param: paramString });
        state.isLoading[paramString] = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getStreamGroupSlice.actions;
export default getStreamGroupSlice.reducer;
