import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, SDSystemStatus } from '@lib/smAPI/smapiTypes';
import { fetchGetSystemStatus } from '@lib/smAPI/Settings/SettingsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: SDSystemStatus | undefined;
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
const getSystemStatusSlice = createSlice({
  name: 'GetSystemStatus',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updatePagedResponseFieldInData(state.data, fieldData);
      console.log('updateGetSystemStatus executed');
    },
    clear: (state) => {
       state = initialState;
      console.log('clearGetSystemStatus executed');
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('setGetSystemStatusIsLoading executed');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('setIsForced ', force);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetSystemStatus.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetSystemStatus.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetSystemStatus.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getSystemStatusSlice.actions;
export default getSystemStatusSlice.reducer;
