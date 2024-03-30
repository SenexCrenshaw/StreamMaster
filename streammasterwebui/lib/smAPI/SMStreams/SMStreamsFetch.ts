import { GetPagedSMStreams } from '@lib/smAPI/SMStreams/SMStreamsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetPagedSMStreams = createAsyncThunk('cache/getGetPagedSMStreams', async (query: string, thunkAPI) => {
  try {
    if (query === undefined) return;
    console.log('Fetching GetPagedSMStreams');
    const params = JSON.parse(query);
    const response = await GetPagedSMStreams(params);
    console.log('Fetched GetPagedSMStreams ',response?.data.length);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});

