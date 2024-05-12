import { GetM3UFiles } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetM3UFiles = createAsyncThunk('cache/getGetM3UFiles', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetM3UFiles');
    const response = await GetM3UFiles();
    console.log('Fetched GetM3UFiles ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


