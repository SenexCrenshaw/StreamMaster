import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import {FieldData, SMTask } from '@lib/smAPI/smapiTypes';
import { fetchGetSMTasks } from '@lib/smAPI/SMChannels/GetSMTasksFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: SMTask[] | undefined;
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

const getSMTasksSlice = createSlice({
  initialState,
  name: 'GetSMTasks',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetSMTasks clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      Logger.debug('GetSMTasks clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      Logger.debug('GetSMTasks setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      state.data = undefined;
      Logger.debug('GetSMTasks  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      Logger.debug('GetSMTasks setIsLoading ', action.payload.isLoading);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetSMTasks.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetSMTasks.fulfilled, (state, action) => {
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
      .addCase(fetchGetSMTasks.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getSMTasksSlice.actions;
export default getSMTasksSlice.reducer;
