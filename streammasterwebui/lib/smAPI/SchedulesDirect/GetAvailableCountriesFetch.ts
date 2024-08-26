import { GetAvailableCountries } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetAvailableCountries = createAsyncThunk('cache/getGetAvailableCountries', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetAvailableCountries');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetAvailableCountries();
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetAvailableCountries completed in ${duration.toFixed(2)}ms`);
    }

    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


