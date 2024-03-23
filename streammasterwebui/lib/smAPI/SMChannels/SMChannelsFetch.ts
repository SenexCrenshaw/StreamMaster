import { GetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetPagedSMChannels = createAsyncThunk('cache/getGetPagedSMChannels', async (query: string, thunkAPI) => {
  try {
    const params = JSON.parse(query);
    const response = await GetPagedSMChannels(params);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});
