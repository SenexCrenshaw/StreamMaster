import { GetPagedEPGFiles } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetPagedEPGFiles = createAsyncThunk('cache/getGetPagedEPGFiles', async (query: string, thunkAPI) => {
  try {
    if (query === undefined) return;
    const params = JSON.parse(query);
    const response = await GetPagedEPGFiles(params);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});


