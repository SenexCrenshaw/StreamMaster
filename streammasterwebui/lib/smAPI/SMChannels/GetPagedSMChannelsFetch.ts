import { GetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { GetPagedSMChannelsRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetPagedSMChannels = createAsyncThunk('cache/getGetPagedSMChannels', async (query: string, thunkAPI) => {
  try {
    if (query === undefined) return;
    console.log('Fetching GetPagedSMChannels');
    const params = JSON.parse(query);
    const response = await GetPagedSMChannels(params);
    console.log('Fetched GetPagedSMChannels ',response?.Data.length);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});


