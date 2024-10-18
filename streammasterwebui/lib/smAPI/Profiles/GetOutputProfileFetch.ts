import { GetOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { GetOutputProfileRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetOutputProfile = createAsyncThunk('cache/getGetOutputProfile', async (param: GetOutputProfileRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    Logger.debug('Fetching GetOutputProfile');
    const fetchDebug = localStorage.getItem('fetchDebug');
    const start = performance.now();
    const response = await GetOutputProfile(param);
    if (fetchDebug) {
      const duration = performance.now() - start;
      Logger.debug(`Fetch GetOutputProfile completed in ${duration.toFixed(2)}ms`);
    }

    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


