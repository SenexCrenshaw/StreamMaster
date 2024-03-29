import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData,  } from '@lib/smAPI/smapiTypes';
import { fetchGetIsSystemReady } from '@lib/smAPI/Settings/SettingsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: boolean | undefined;
  isLoading: boolean;
  isError: boolean;
  error: string | undefined;
}

const initialState: QueryState = {
  data: undefined,
  isLoading: false,
  isError: false,
  error: undefined
};
const getIsSystemReadySlice = createSlice({
  name: 'GetIsSystemReady',
  initialState,
  reducers: {
    updateGetIsSystemReady: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updatePagedResponseFieldInData(state.data, fieldData);
      console.log('updateGetIsSystemReady executed');
    },
    clearGetIsSystemReady: (state) => {
      state.data = undefined;
      state.error = undefined;
      state.isError = false;
      state.isLoading = false;
      console.log('clearGetIsSystemReady executed');
    },
    intSetGetIsSystemReadyIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('setGetIsSystemReadyIsLoading executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetIsSystemReady.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
      })
      .addCase(fetchGetIsSystemReady.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
        }
      })
      .addCase(fetchGetIsSystemReady.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
      });

  }
});

export const { clearGetIsSystemReady, intSetGetIsSystemReadyIsLoading, updateGetIsSystemReady } = getIsSystemReadySlice.actions;
export default getIsSystemReadySlice.reducer;
