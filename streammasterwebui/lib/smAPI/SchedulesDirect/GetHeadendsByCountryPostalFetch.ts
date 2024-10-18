import { GetHeadendsByCountryPostal } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { GetHeadendsByCountryPostalRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetHeadendsByCountryPostal = createAsyncThunk('cache/getGetHeadendsByCountryPostal', async (param: GetHeadendsByCountryPostalRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    Logger.debug('Fetching GetHeadendsByCountryPostal');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetHeadendsByCountryPostal(param);
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetHeadendsByCountryPostal completed in ${duration.toFixed(2)}ms`);
    }

    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


