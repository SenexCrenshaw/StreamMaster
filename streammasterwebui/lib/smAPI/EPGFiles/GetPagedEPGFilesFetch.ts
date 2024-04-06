import { GetPagedEPGFiles } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { GetPagedEPGFilesRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetPagedEPGFiles = createAsyncThunk('cache/getGetPagedEPGFiles', async (query: string, thunkAPI) => {
  try {
    if (query === undefined) return;
    console.log('Fetching GetPagedEPGFiles');
    const params = JSON.parse(query);
    const response = await GetPagedEPGFiles(params);
    console.log('Fetched GetPagedEPGFiles ',response?.data.length);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});


