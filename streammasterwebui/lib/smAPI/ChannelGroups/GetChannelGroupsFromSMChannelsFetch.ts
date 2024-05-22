import { GetChannelGroupsFromSMChannels } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetChannelGroupsFromSMChannels = createAsyncThunk('cache/getGetChannelGroupsFromSMChannels', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetChannelGroupsFromSMChannels');
    const response = await GetChannelGroupsFromSMChannels();
    console.log('Fetched GetChannelGroupsFromSMChannels ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


