import { GetPagedStreamGroups } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetPagedStreamGroups = createAsyncThunk('cache/getGetPagedStreamGroups', async (query: string, thunkAPI) => {
  try {
    if (query === undefined) return;
    console.log('Fetching GetPagedStreamGroups');
    const params = JSON.parse(query);
    const response = await GetPagedStreamGroups(params);
    console.log('Fetched GetPagedStreamGroups ',response?.Data.length);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});


