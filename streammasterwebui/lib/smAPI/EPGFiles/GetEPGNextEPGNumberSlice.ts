import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData,  } from '@lib/smAPI/smapiTypes';
import { fetchGetEPGNextEPGNumber } from '@lib/smAPI/EPGFiles/GetEPGNextEPGNumberFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: number | undefined;
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

const getEPGNextEPGNumberSlice = createSlice({
  name: 'GetEPGNextEPGNumber',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      console.log('GetEPGNextEPGNumber setField');
    },
    clear: (state) => {
       state = initialState;
      console.log('GetEPGNextEPGNumber clear');
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('GetEPGNextEPGNumber setIsLoading ', action.payload.isLoading);
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetEPGNextEPGNumber  setIsForced ', force);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetEPGNextEPGNumber.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetEPGNextEPGNumber.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetEPGNextEPGNumber.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getEPGNextEPGNumberSlice.actions;
export default getEPGNextEPGNumberSlice.reducer;
