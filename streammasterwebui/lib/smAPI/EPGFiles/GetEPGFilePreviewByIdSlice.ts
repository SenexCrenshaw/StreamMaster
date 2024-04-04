import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, EPGFilePreviewDto } from '@lib/smAPI/smapiTypes';
import { fetchGetEPGFilePreviewById } from '@lib/smAPI/EPGFiles/GetEPGFilePreviewByIdFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: EPGFilePreviewDto[] | undefined;
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
const getEPGFilePreviewByIdSlice = createSlice({
  name: 'GetEPGFilePreviewById',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      console.log('GetEPGFilePreviewById setField');
    },
    clear: (state) => {
       state = initialState;
      console.log('GetEPGFilePreviewById clear');
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('GetEPGFilePreviewById setIsLoading ', action.payload.isLoading);
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetEPGFilePreviewById  setIsForced ', force);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetEPGFilePreviewById.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetEPGFilePreviewById.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetEPGFilePreviewById.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getEPGFilePreviewByIdSlice.actions;
export default getEPGFilePreviewByIdSlice.reducer;
