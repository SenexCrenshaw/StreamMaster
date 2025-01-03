import { GetSMChannelUniqueName } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { GetSMChannelUniqueNameRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetSMChannelUniqueName = createAsyncThunk('cache/getGetSMChannelUniqueName', async (param: GetSMChannelUniqueNameRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetSMChannelUniqueName');
        return undefined;
    }
    Logger.debug('Fetching GetSMChannelUniqueName');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetSMChannelUniqueName(param);
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetSMChannelUniqueName completed in ${duration.toFixed(2)}ms`);
    }

    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


