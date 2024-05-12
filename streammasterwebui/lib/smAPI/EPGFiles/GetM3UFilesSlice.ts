import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, M3UFileDto } from '@lib/smAPI/smapiTypes';
import { fetchGetM3UFiles } from '@lib/smAPI/EPGFiles/GetM3UFilesFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: M3UFileDto[] | undefined;
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

const getM3UFilesSlice = createSlice({
  initialState,
  name: 'GetM3UFiles',
  reducers: {
    clear: (state) => {
      state = initialState;
      console.log('GetM3UFiles clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      console.log('GetM3UFiles clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      console.log('GetM3UFiles setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetM3UFiles  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      console.log('GetM3UFiles setIsLoading ', action.payload.isLoading);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetM3UFiles.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetM3UFiles.fulfilled, (state, action) => {
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
      .addCase(fetchGetM3UFiles.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getM3UFilesSlice.actions;
export default getM3UFilesSlice.reducer;
