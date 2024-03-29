import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, SDSystemStatus } from '@lib/smAPI/smapiTypes';
import { fetchGetSystemStatus } from '@lib/smAPI/Settings/SettingsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: SDSystemStatus | undefined;
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
const getSystemStatusSlice = createSlice({
  name: 'GetSystemStatus',
  initialState,
  reducers: {
    updateGetSystemStatus: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updatePagedResponseFieldInData(state.data, fieldData);
      console.log('updateGetSystemStatus executed');
    },
    clearGetSystemStatus: (state) => {
      state.data = undefined;
      state.error = undefined;
      state.isError = false;
      state.isLoading = false;
      console.log('clearGetSystemStatus executed');
    },
    intSetGetSystemStatusIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('setGetSystemStatusIsLoading executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetSystemStatus.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
      })
      .addCase(fetchGetSystemStatus.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
        }
      })
      .addCase(fetchGetSystemStatus.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
      });

  }
});

export const { clearGetSystemStatus, intSetGetSystemStatusIsLoading, updateGetSystemStatus } = getSystemStatusSlice.actions;
export default getSystemStatusSlice.reducer;
