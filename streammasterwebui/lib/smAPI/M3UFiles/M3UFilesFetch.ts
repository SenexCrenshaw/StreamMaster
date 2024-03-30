import { GetPagedM3UFiles } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetPagedM3UFiles = createAsyncThunk('cache/getGetPagedM3UFiles', async (query: string, thunkAPI) => {
  try {
    if (query === undefined) return;
    console.log('Fetching GetPagedM3UFiles');
    const params = JSON.parse(query);
    const response = await GetPagedM3UFiles(params);
    console.log('Fetched GetPagedM3UFiles ',response?.data.length);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});

