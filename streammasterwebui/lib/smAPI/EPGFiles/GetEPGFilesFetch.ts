import { GetEPGFiles } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGFiles = createAsyncThunk('cache/getGetEPGFiles', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetEPGFiles');
    const response = await GetEPGFiles();
    console.log('Fetched GetEPGFiles ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


