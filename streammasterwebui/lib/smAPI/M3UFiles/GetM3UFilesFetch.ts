import { GetM3UFiles } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetM3UFiles = createAsyncThunk('cache/getGetM3UFiles', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetM3UFiles');
    const response = await GetM3UFiles();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


