import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import {FieldData, ChannelMetric } from '@lib/smAPI/smapiTypes';
import { fetchGetChannelMetrics } from '@lib/smAPI/Statistics/GetChannelMetricsFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: ChannelMetric[] | undefined;
  error: string | undefined;
  isError: boolean;
  isForced: boolean;
  isLoading: boolean;
}

const initialState: QueryState = {
  data: undefined,
  error: undefined,
  isError: false,
  isForced: false,
  isLoading: false
};

const getChannelMetricsSlice = createSlice({
  initialState,
  name: 'GetChannelMetrics',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetChannelMetrics clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      Logger.debug('GetChannelMetrics clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      Logger.debug('GetChannelMetrics setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      Logger.debug('GetChannelMetrics  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      Logger.debug('GetChannelMetrics setIsLoading ', action.payload.isLoading);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetChannelMetrics.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetChannelMetrics.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;
          setIsLoading({ isLoading: false });
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetChannelMetrics.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getChannelMetricsSlice.actions;
export default getChannelMetricsSlice.reducer;
