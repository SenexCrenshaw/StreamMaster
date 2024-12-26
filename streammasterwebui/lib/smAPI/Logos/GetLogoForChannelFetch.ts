import { GetLogoForChannel } from '@lib/smAPI/Logos/LogosCommands';
import { GetLogoForChannelRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetLogoForChannel = createAsyncThunk('cache/getGetLogoForChannel', async (param: GetLogoForChannelRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetLogoForChannel');
        return undefined;
    }
    Logger.debug('Fetching GetLogoForChannel');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetLogoForChannel(param);
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetLogoForChannel completed in ${duration.toFixed(2)}ms`);
    }

    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


