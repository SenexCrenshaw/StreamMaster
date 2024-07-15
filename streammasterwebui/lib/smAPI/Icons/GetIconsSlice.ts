import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import {FieldData, IconFileDto } from '@lib/smAPI/smapiTypes';
import { fetchGetIcons } from '@lib/smAPI/Icons/GetIconsFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


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
  initialState,
  name: 'GetIcons',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetIcons clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      Logger.debug('GetIcons clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      Logger.debug('GetIcons setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      state.data = undefined;
      Logger.debug('GetIcons  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      Logger.debug('GetIcons setIsLoading ', action.payload.isLoading);
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
          state.data = value ?? undefined;
          setIsLoading({ isLoading: false });
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetIcons.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getIconsSlice.actions;
export default getIconsSlice.reducer;