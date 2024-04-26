import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import {FieldData, StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { fetchGetStreamGroups } from '@lib/smAPI/StreamGroups/GetStreamGroupsFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: StreamGroupDto[] | undefined;
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

const getStreamGroupsSlice = createSlice({
  initialState,
  name: 'GetStreamGroups',
  reducers: {
    clear: (state) => {
      state = initialState;
      console.log('GetStreamGroups clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      console.log('GetStreamGroups clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      console.log('GetStreamGroups setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('GetStreamGroups  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      console.log('GetStreamGroups setIsLoading ', action.payload.isLoading);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetStreamGroups.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetStreamGroups.fulfilled, (state, action) => {
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
      .addCase(fetchGetStreamGroups.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getStreamGroupsSlice.actions;
export default getStreamGroupsSlice.reducer;
