import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import { FieldData } from '@lib/smAPI/smapiTypes';
import { fetchGetIsSystemReady } from '@lib/smAPI/General/GetIsSystemReadyFetch';

interface QueryState {
  data: boolean | undefined;
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

const getIsSystemReadySlice = createSlice({
  initialState,
  name: 'GetIsSystemReady',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetIsSystemReady clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      Logger.debug('GetIsSystemReady clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = fieldData.Value;
      Logger.debug('GetIsSystemReady setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      state.data = undefined;
      Logger.debug('GetIsSystemReady  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{ isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      Logger.debug('GetIsSystemReady setIsLoading ', action.payload.isLoading);
    }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetIsSystemReady.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetIsSystemReady.fulfilled, (state, action) => {
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
      .addCase(fetchGetIsSystemReady.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });
  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getIsSystemReadySlice.actions;
export default getIsSystemReadySlice.reducer;
