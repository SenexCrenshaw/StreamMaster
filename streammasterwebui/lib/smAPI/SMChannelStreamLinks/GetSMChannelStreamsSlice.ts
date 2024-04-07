import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { fetchGetSMChannelStreams } from '@lib/smAPI/SMChannelStreamLinks/GetSMChannelStreamsFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: SMStreamDto[] | undefined;
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
const getSMChannelStreamsSlice = createSlice({
  name: 'GetSMChannelStreams',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      console.log('GetSMChannelStreams setField');
    },
    clear: (state) => {
       state = initialState;
      console.log('GetSMChannelStreams clear');
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
       state.isLoading = action.payload.isLoading;
      console.log('GetSMChannelStreams setIsLoading ', action.payload.isLoading);
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetSMChannelStreams  setIsForced ', force);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetSMChannelStreams.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetSMChannelStreams.fulfilled, (state, action) => {
        if (action.payload) {
          const { value } = action.payload;
          state.data = value ?? undefined;;
          state.isLoading = false;
          state.isError = false;
          state.error = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetSMChannelStreams.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, setIsLoading, setIsForced, setField } = getSMChannelStreamsSlice.actions;
export default getSMChannelStreamsSlice.reducer;
