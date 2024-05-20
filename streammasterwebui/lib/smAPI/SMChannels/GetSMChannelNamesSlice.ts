import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData,  } from '@lib/smAPI/smapiTypes';
import { fetchGetSMChannelNames } from '@lib/smAPI/SMChannels/GetSMChannelNamesFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: string[] | undefined;
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

const getSMChannelNamesSlice = createSlice({
  initialState,
  name: 'GetSMChannelNames',
  reducers: {
    clear: (state) => {
      state = initialState;
      console.log('GetSMChannelNames clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      console.log('GetSMChannelNames clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      console.log('GetSMChannelNames setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      state.data = undefined;
      console.log('GetSMChannelNames  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      console.log('GetSMChannelNames setIsLoading ', action.payload.isLoading);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetSMChannelNames.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetSMChannelNames.fulfilled, (state, action) => {
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
      .addCase(fetchGetSMChannelNames.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getSMChannelNamesSlice.actions;
export default getSMChannelNamesSlice.reducer;
