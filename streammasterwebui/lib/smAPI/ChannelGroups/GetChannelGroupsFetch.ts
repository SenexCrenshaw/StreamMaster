import { GetChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelGroups = createAsyncThunk('cache/getGetChannelGroups', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetChannelGroups');
    const response = await GetChannelGroups();
    console.log('Fetched GetChannelGroups ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


