import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, EPGFilePreviewDto } from '@lib/smAPI/smapiTypes';
import { fetchGetEPGFilePreviewById } from '@lib/smAPI/EPGFiles/GetEPGFilePreviewByIdFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: Record<string, EPGFilePreviewDto[] | undefined>;
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

const getEPGFilePreviewByIdSlice = createSlice({
  name: 'GetEPGFilePreviewById',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ param?: string | undefined; fieldData: FieldData }>) => {
      const { param , fieldData } = action.payload;

      if (param !== undefined) {
        const paramString = JSON.stringify(param);
        if (state.data[paramString]) {
          state.data[paramString] = updateFieldInData(state.data[paramString], fieldData);
        }
        return;
      }

      for (const key in state.data) {
        if (state.data[key]) {
          state.data[key] = updateFieldInData(state.data[key], fieldData);
        }
      }
      console.log('GetEPGFilePreviewById setField');
    },
    clear: (state) => {
       state = initialState;
       console.log('GetEPGFilePreviewById clear');
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
      console.log('GetEPGFilePreviewById setIsLoading ', action.payload.isLoading);
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetEPGFilePreviewById  setIsForced ', force);
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetEPGFilePreviewById.pending, (state, action) => {
        const paramString = JSON.stringify(action.meta.arg);
        state.isLoading[paramString] = true;
        state.isError[paramString] = false;
        state.isForced = false;
        state.error[paramString] = undefined;
      })
      .addCase(fetchGetEPGFilePreviewById.fulfilled, (state, action) => {
        if (action.payload) {
          const { param, value } = action.payload;
          const paramString = JSON.stringify(param);
          state.data[paramString] = value;
          state.isLoading[paramString] = false;
          state.isError[paramString] = false;
          state.error[paramString] = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetEPGFilePreviewById.rejected, (state, action) => {
        const paramString = JSON.stringify(action.meta.arg);
        state.error[paramString] = action.error.message || 'Failed to fetch';
        state.isError[paramString] = true;
        state.isLoading[paramString] = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getEPGFilePreviewByIdSlice.actions;
export default getEPGFilePreviewByIdSlice.reducer;
