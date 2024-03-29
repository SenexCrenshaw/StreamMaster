import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, IconFileDto } from '@lib/smAPI/smapiTypes';
import { fetchGetIcons } from '@lib/smAPI/Icons/IconsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: IconFileDto[] | undefined;
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
const getIconsSlice = createSlice({
  name: 'GetIcons',
  initialState,
  reducers: {
    updateGetIcons: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updatePagedResponseFieldInData(state.data, fieldData);
      console.log('updateGetIcons executed');
    },
    clearGetIcons: (state) => {
      state.data = undefined;
      state.error = undefined;
      state.isError = false;
      state.isLoading = false;
      console.log('clearGetIcons executed');
    },
    intSetGetIconsIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('setGetIconsIsLoading executed');
    },

  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetIcons.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
      })
      .addCase(fetchGetIcons.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
        }
      })
      .addCase(fetchGetIcons.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
      });

  }
});

export const { clearGetIcons, intSetGetIconsIsLoading, updateGetIcons } = getIconsSlice.actions;
export default getIconsSlice.reducer;
