import { GetEPGFiles } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


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


