import { GetChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelGroups = createAsyncThunk('cache/getGetChannelGroups', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetChannelGroups');
    const response = await GetChannelGroups();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


