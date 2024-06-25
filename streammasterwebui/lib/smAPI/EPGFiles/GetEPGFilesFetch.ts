import { GetEPGFiles } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetEPGFiles = createAsyncThunk('cache/getGetEPGFiles', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetEPGFiles');
    const response = await GetEPGFiles();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


