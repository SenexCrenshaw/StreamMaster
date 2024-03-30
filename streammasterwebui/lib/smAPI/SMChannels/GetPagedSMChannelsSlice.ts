import { updatePagedResponseFieldInData } from '@lib/redux/updatePagedResponseFieldInData';
import { fetchGetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsFetch';
import { FieldData, PagedResponse, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { PayloadAction, createSlice } from '@reduxjs/toolkit';

interface QueryState {
  data: Record<string, PagedResponse<SMChannelDto> | undefined>;
  isLoading: Record<string, boolean>;
  isForced: boolean;
  isError: Record<string, boolean>;
  error: Record<string, string | undefined>;
}

const initialState: QueryState = {
  data: {},
  isLoading: {},
  isError: {},
  isForced: false,
  error: {}
};

const getPagedSMChannelsSlice = createSlice({
  name: 'GetPagedSMChannels',
  initialState,
  reducers: {
    setField: (state, action: PayloadAction<{ query?: string | undefined; fieldData: FieldData }>) => {
      const { query, fieldData } = action.payload;

      if (query !== undefined) {
        if (state.data[query]) {
          state.data[query] = updatePagedResponseFieldInData(state.data[query], fieldData);
        }
        return;
      }

      for (const key in state.data) {
        if (state.data[key]) {
          state.data[key] = updatePagedResponseFieldInData(state.data[key], fieldData);
        }
      }
      console.log('setField');
    },
    clear: (state) => {
      state = initialState;
      console.log('clear');
    },
    setIsLoading: (state, action: PayloadAction<{ query?: string; isLoading: boolean }>) => {
      const { query, isLoading } = action.payload;
      if (query !== undefined) {
        state.isLoading[query] = isLoading;
      } else {
        for (const key in state.data) {
          state.isLoading[key] = action.payload.isLoading;
        }
      }
      console.log('setIsLoading ', action.payload.isLoading);
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      console.log('setIsForced ', force);
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchGetPagedSMChannels.pending, (state, action) => {
        const query = action.meta.arg;
        state.isLoading[query] = true;
        state.isError[query] = false;
        state.error[query] = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetPagedSMChannels.fulfilled, (state, action) => {
        if (action.payload) {
          const { query, value } = action.payload;
          state.data[query] = value;
          state.isLoading[query] = false;
          state.isError[query] = false;
          state.error[query] = undefined;
          state.isForced = false;
        }
      })
      .addCase(fetchGetPagedSMChannels.rejected, (state, action) => {
        const query = action.meta.arg;
        state.error[query] = action.error.message || 'Failed to fetch';
        state.isError[query] = true;
        state.isLoading[query] = false;
        state.isForced = false;
      });
  }
});

export const { clear, setIsLoading, setIsForced, setField } = getPagedSMChannelsSlice.actions;
export default getPagedSMChannelsSlice.reducer;
