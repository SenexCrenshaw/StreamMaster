import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData,  } from '@lib/smAPI/smapiTypes';
import { fetchGetIsSystemReady } from '@lib/smAPI/Settings/GetIsSystemReadyFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


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
      console.log('GetIsSystemReady clear');
    },
    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      console.log('GetIsSystemReady setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetIsSystemReady  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      console.log('GetIsSystemReady setIsLoading ', action.payload.isLoading);
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
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetIsSystemReady.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getIsSystemReadySlice.actions;
export default getIsSystemReadySlice.reducer;
