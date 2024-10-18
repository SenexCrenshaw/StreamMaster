import { GetEPGFileNames } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGFileNames = createAsyncThunk('cache/getGetEPGFileNames', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetEPGFileNames');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetEPGFileNames();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetEPGFileNames completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


