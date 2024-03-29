import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, SettingDto } from '@lib/smAPI/smapiTypes';
import { fetchGetSettings } from '@lib/smAPI/Settings/SettingsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: SettingDto | undefined;
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
const getSettingsSlice = createSlice({
  name: 'GetSettings',
  initialState,
  reducers: {
    updateGetSettings: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updatePagedResponseFieldInData(state.data, fieldData);
      console.log('updateGetSettings executed');
    },
    clearGetSettings: (state) => {
      state.data = undefined;
      state.error = undefined;
      state.isError = false;
      state.isLoading = false;
      console.log('clearGetSettings executed');
    },
    intSetGetSettingsIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('setGetSettingsIsLoading executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetSettings.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
      })
      .addCase(fetchGetSettings.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
        }
      })
      .addCase(fetchGetSettings.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
      });

  }
});

export const { clearGetSettings, intSetGetSettingsIsLoading, updateGetSettings } = getSettingsSlice.actions;
export default getSettingsSlice.reducer;
