import { GetChannelGroupsFromSMChannels } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetChannelGroupsFromSMChannels = createAsyncThunk('cache/getGetChannelGroupsFromSMChannels', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetChannelGroupsFromSMChannels');
    const response = await GetChannelGroupsFromSMChannels();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


