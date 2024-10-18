import { PayloadAction, createSlice } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';
import {FieldData, ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { fetchGetChannelGroupsFromSMChannels } from '@lib/smAPI/ChannelGroups/GetChannelGroupsFromSMChannelsFetch';
import { updateFieldInData } from '@lib/redux/updateFieldInData';


interface QueryState {
  data: ChannelGroupDto[] | undefined;
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

const getChannelGroupsFromSMChannelsSlice = createSlice({
  initialState,
  name: 'GetChannelGroupsFromSMChannels',
  reducers: {
    clear: (state) => {
      state = initialState;
      Logger.debug('GetChannelGroupsFromSMChannels clear');
    },

    clearByTag: (state, action: PayloadAction<{ tag: string }>) => {
      state.data = undefined;
      Logger.debug('GetChannelGroupsFromSMChannels clearByTag');
    },

    setField: (state, action: PayloadAction<{ fieldData: FieldData }>) => {
      const { fieldData } = action.payload;
      state.data = updateFieldInData(state.data, fieldData);
      Logger.debug('GetChannelGroupsFromSMChannels setField');
    },
    setIsForced: (state, action: PayloadAction<{ force: boolean }>) => {
      const { force } = action.payload;
      state.isForced = force;
      Logger.debug('GetChannelGroupsFromSMChannels  setIsForced ', force);
    },
    setIsLoading: (state, action: PayloadAction<{isLoading: boolean }>) => {
      state.isLoading = action.payload.isLoading;
      Logger.debug('GetChannelGroupsFromSMChannels setIsLoading ', action.payload.isLoading);
    }
},

  extraReducers: (builder) => {
    builder
      .addCase(fetchGetChannelGroupsFromSMChannels.pending, (state, action) => {
        state.isLoading = true;
        state.isError = false;
        state.error = undefined;
        state.isForced = false;
      })
      .addCase(fetchGetChannelGroupsFromSMChannels.fulfilled, (state, action) => {
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
      .addCase(fetchGetChannelGroupsFromSMChannels.rejected, (state, action) => {
        state.error = action.error.message || 'Failed to fetch';
        state.isError = true;
        setIsLoading({ isLoading: false });
        state.isLoading = false;
        state.isForced = false;
      });

  }
});

export const { clear, clearByTag, setIsLoading, setIsForced, setField } = getChannelGroupsFromSMChannelsSlice.actions;
export default getChannelGroupsFromSMChannelsSlice.reducer;
