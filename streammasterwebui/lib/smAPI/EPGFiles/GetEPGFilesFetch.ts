import { GetEPGFiles } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGFiles = createAsyncThunk('cache/getGetEPGFiles', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetEPGFiles');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetEPGFiles();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetEPGFiles completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


