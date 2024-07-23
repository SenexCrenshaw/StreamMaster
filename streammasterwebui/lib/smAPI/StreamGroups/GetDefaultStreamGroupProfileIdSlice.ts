import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import {FieldData, StreamGroupProfile } from '@lib/smAPI/smapiTypes';
import { fetchGetDefaultStreamGroupProfileId } from '@lib/smAPI/StreamGroups/GetDefaultStreamGroupProfileIdFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: StreamGroupProfile | undefined;
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

const getDefaultStreamGroupProfileIdSlice = createSlice({
  initialState,
  name: 'GetDefaultStreamGroupProfileId',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetDefaultStreamGroupProfileId clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      Logger.debug('GetDefaultStreamGroupProfileId clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      Logger.debug('GetDefaultStreamGroupProfileId setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      state.data = undefined;
      Logger.debug('GetDefaultStreamGroupProfileId  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      Logger.debug('GetDefaultStreamGroupProfileId setIsLoading ', action.payload.isLoading);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetDefaultStreamGroupProfileId.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetDefaultStreamGroupProfileId.fulfilled, (state, action) => {
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
      .addCase(fetchGetDefaultStreamGroupProfileId.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getDefaultStreamGroupProfileIdSlice.actions;
export default getDefaultStreamGroupProfileIdSlice.reducer;
