import { GetSMChannelNames } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMChannelNames = createAsyncThunk('cache/getGetSMChannelNames', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetSMChannelNames');
    const response = await GetSMChannelNames();
    console.log('Fetched GetSMChannelNames ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


