import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, IconFileDto } from '@lib/smAPI/smapiTypes';
import { fetchGetIcons } from '@lib/smAPI/Icons/IconsFetch';
import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';


interface QueryState {
  data: IconFileDto[] | undefined;
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
const getIconsSlice = createSlice({
  name: 'GetIcons',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updatePagedResponseFieldInData(state.data, fieldData);
      console.log('updateGetIcons executed');
    },
    clear: (state) => {
       state = initialState;
      console.log('clearGetIcons executed');
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('setGetIconsIsLoading executed');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('setIsForced ', force);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetIcons.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetIcons.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetIcons.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getIconsSlice.actions;
export default getIconsSlice.reducer;
